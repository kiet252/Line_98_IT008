using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Line_98
{
    public class MainGamePanel : Panel
    {
        //Form tham chiếu từ Main_Form
        public Main_Form Main_Form;

        //Tạo 1 bảng gồm 9*9 Cells (button) độ lớn 9x9
        private Button[,] BoardCells = new Button[9, 9];

        //Tạo bảng CellBoard(Panel) chứa 9*9 Cells
        private Panel CellBoard;
        private int CellSize = 0;

        //Lưu màu quân cờ
        private int[,] BoardColor = new int[9, 9];
        private Color[] GameColor = {
            Color.LightGray, //Màu ô rỗng
            Color.Red, // 1
            Color.Blue,// 2
            Color.Green, // 3
            Color.DarkRed, // 4
            Color.Purple // 5
        };

        //Hằng quyết định số lượng banh mới mỗi lần tạo
        private const int MaxBallsPerGeneration = 5;

        //Hằng quyết định kích cỡ các quả banh
        private const int BallsSize = 30;

        //Biến theo dõi xem ô nào mới được chọn, vị trí ô mới được chọn
        private Button FirstSelectedCell = null;

        //Constructor
        public MainGamePanel(Main_Form mainForm)
        {
            this.Main_Form = mainForm;

            this.Location = new Point(10, 20);
            this.Size = new Size(700, 800);

            InitializePanel();

            //Tạo các banh khi mới vào game
            GenerateNewPieces();
        }

        //Khởi tạo bảng CellBoard
        public void InitializePanel()
        {
            CellBoard = new Panel();
            CellBoard.Size = new Size(630, 630);
            CellBoard.BackColor = Color.WhiteSmoke;
            CellBoard.Location = new Point(20, 100);

            //Thêm bảng CellBoard vào Form
            this.Controls.Add(CellBoard);

            //Vì ô CellBoard luôn có 9 x 9 ô, độ lớn mỗi phần tử cell là CellBoard.Width / 9
            CellSize = (CellBoard.Width / 9);

            //Khởi tạo từng Cell trong CellBoard
            InitializeBoard();
        }

        void InitializeBoard()
        {
            for (int Row = 0; Row < 9; Row++)
            {
                for (int Col = 0; Col < 9; Col++)
                {
                    //Khởi tạo từng Cell
                    Button Cell = new Button();
                    Cell.Size = new Size(CellSize, CellSize);
                    Cell.Location = new Point(Col * CellSize, Row * CellSize);

                    //Mỗi Cell sẽ lưu trữ vị trí hàng - cột của bản thân nó vào Tag
                    Cell.Tag = new Point(Row, Col);

                    //.Font quyết định kích thước của quả banh (Về sau nếu làm banh bằng control khác thì có thể bỏ đi)
                    Cell.Font = new Font("Arial", BallsSize, FontStyle.Regular);

                    //Chỉnh Cell (Button) hiện tại có hình phẳng thay vì nút 3D mặc định
                    Cell.FlatStyle = FlatStyle.Flat;

                    //Tham chiếu Cell mới vào Cell trong CellBoard, để có thể sử dụng trong các method sau
                    BoardCells[Row, Col] = Cell;

                    /*Khi Cell được nhấp vào, Cell sẽ thực hiện hàm CellClick 
                    -> hàm CellClick là thao tác mà Cell đó thực hiện khi được nhấp vào */
                    Cell.Click += CellClick;

                    //Thêm Cell vừa tạo vào CellBoard
                    CellBoard.Controls.Add(Cell);

                    //Cho màu ban đầu của các ô là 0 (Không có ball)
                    BoardColor[Row, Col] = 0;
                }
            }
        }

        private void CellClick(object sender, EventArgs e)
        {
            //Check xem cell vừa ấn vào có vị trí ở đâu thông qua Cell.Tag
            Button Clicked_Cell = (Button)sender;
            Point Clicked_Location = (Point)Clicked_Cell.Tag;

            //Lưu vị trí ô được chọn
            int XCellPosition = Clicked_Location.X;
            int YCellPosition = Clicked_Location.Y;

            //Nếu ô được chọn không có màu, không có gì xảy ra
            if (BoardColor[XCellPosition, YCellPosition] == 0) return;

            //Trường hợp 1: Lần đầu tiên chọn ô -> Hiển thị là ô đang được chọn
            if (FirstSelectedCell == null)
            {
                //FirstSelectedCell là biến tham chiếu đến Cell vừa được chọn
                FirstSelectedCell = Clicked_Cell;

                //Hiển thị ô đang được chọn có kích thước gấp 1.5 ban đầu (Về sau có thể đổi lại thành animation nảy nảy hoặc khác)
                Clicked_Cell.Font = new Font("Arial", (int)(BallsSize * 1.5), FontStyle.Bold);
            }
            else
            //Trường hợp 2: Chọn ô khác
            {
                //Nếu ô khác chọn trùng với ô đã được chọn, bỏ chọn
                if (FirstSelectedCell == Clicked_Cell)
                {
                    //Đổi kích thước banh về kích thước ban đầu
                    Clicked_Cell.Font = new Font("Arial", BallsSize, FontStyle.Regular);
                    FirstSelectedCell.FlatAppearance.BorderSize = 1;
                    FirstSelectedCell.FlatAppearance.BorderColor = Color.Black;
                    ResetSelection();
                } 
                //Không phải thì tiến hành di chuyển (Chưa code)
            }

        }

        private void ResetSelection()
        {
            // Bỏ đánh dấu ô cũ
            if (FirstSelectedCell != null)
            {
                FirstSelectedCell.FlatAppearance.BorderSize = 1;
                FirstSelectedCell.FlatAppearance.BorderColor = Color.Black;
                FirstSelectedCell = null;
            }
        }

        private void GenerateNewPieces()
        {
            Random random = new Random();
            //Kiểm tra xem ô nào vừa được tạo ball tại đó
            bool[,] Visited = new bool[9, 9];

            //Biến SuccessCount đếm số lượng ball được tạo, nếu đủ lượng MaxBallsPerGeneration thì dừng
            int SuccessCount = 0;

            while (SuccessCount < MaxBallsPerGeneration)
            {
                //Tìm vị trí ngẫu nhiên để tạo banh
                int RandomX = random.Next(0, 9);
                int RandomY = random.Next(0, 9);
                //Chọn màu ngẫu nhiên cho banh
                int RandomColor = random.Next(1, GameColor.Length);
                //Nếu ô đang xét đã thăm hay có một màu khác ở ô đang xét thì bỏ qua
                if (Visited[RandomX, RandomY] || BoardColor[RandomX, RandomY] != 0) continue;
                else
                {
                    ApplyColorToCell(BoardCells[RandomX, RandomY], RandomColor);
                    Visited[RandomX, RandomY] = true;
                    SuccessCount++;
                }
            }
        }

        private void ApplyColorToCell(Button Cell, int ColorType)
        {
            Point CellLocation = (Point)Cell.Tag;

            if (ColorType != 0)
            {
                //Nếu ô không rỗng, hiến thị ball là "●" (về sau có thể đổi lại)
                Cell.Text = "●";
                //Dùng border để tránh việc border của Cell tự ý đổi thành ball color
                Cell.FlatAppearance.BorderSize = 1;
                Cell.FlatAppearance.BorderColor = Color.Black;
                Cell.ForeColor = GameColor[ColorType];
                //Nhận vị trí cell và đặt màu mới cho BoardColor tại vị trí cell đó
                BoardColor[CellLocation.X, CellLocation.Y] = ColorType;
            }
        }
    }
}
