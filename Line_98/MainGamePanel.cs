using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
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
        private GameCell[,] BoardCells = new GameCell[9, 9];

        //Tạo bảng CellBoard(Panel) chứa 9*9 Cells
        private Panel CellBoard;
        private int CellSize = 0;

        //Lưu màu quân cờ
        private int[,] BoardColor = new int[9, 9];

        //Hằng quyết định số lượng banh mới mỗi lần tạo
        private const int MaxBallsPerGeneration = 5;

        //Hằng quyết định kích cỡ các quả banh
        private const int BallsSize = 30;

        //Biến theo dõi xem ô nào mới được chọn, vị trí ô mới được chọn
        private GameCell FirstSelectedCell = null;

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
                    GameCell Cell = new GameCell(CellSize, BallsSize, new Point(Col * CellSize, Row * CellSize), new Point(Row, Col));

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
            GameCell Clicked_Cell = (GameCell)sender;

            //Trường hợp 1: Lần đầu tiên chọn ô -> Hiển thị là ô đang được chọn
            if (FirstSelectedCell == null)
            {
                //Nếu ô được chọn không có màu, không có gì xảy ra
                if (!Clicked_Cell.HasBall()) return;

                //FirstSelectedCell là biến tham chiếu đến Cell vừa được chọn
                FirstSelectedCell = Clicked_Cell;

                //Hiển thị ô đang được chọn có border màu đỏ (Về sau có thể đổi lại thành animation nảy nảy hoặc khác)
                FirstSelectedCell.GetSelected();
            }
            else
            //Trường hợp 2: Chọn ô khác
            {
                //Nếu ô khác chọn trùng với ô đã được chọn, bỏ chọn
                if (FirstSelectedCell == Clicked_Cell)
                {
                    //Đổi kích thước banh về kích thước ban đầu
                    Clicked_Cell.GetUnselected();
                    ResetSelection();
                }
                //Không phải thì tiến hành di chuyển 
                else if (Clicked_Cell.Text == "")
                {
                    MoveBall(FirstSelectedCell, Clicked_Cell);
                    BallEnvol();
                    GenerateNewPieces();
                }
            }

        }

        // Phóng lớn banh 
        private void BallEnvol()
        {
            foreach (GameCell cell in BoardCells)
            {
                if (BoardColor[cell.X_Pos, cell.Y_Pos] != 0)
                {
                    cell.Font = new Font("Arial", (int)(BallsSize * 1.5), FontStyle.Bold);
                }
            }
        }

        //Kiểm tra xem có di chuyển được đến ô đã chọn
        private bool CanMoveBall(Point StartPoint, Point EndPoint)
        {
            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };
            bool[,] visited = new bool[9, 9];
            //Duyệt BFS xem có thể đến được đích từ điểm bắt đầu
            Queue<Point> CheckQueue = new Queue<Point>();
            CheckQueue.Enqueue(StartPoint);
            Console.WriteLine("New iteration");
            while (CheckQueue.Count > 0)
            {
                Point point = CheckQueue.First();
                CheckQueue.Dequeue();

                Console.WriteLine("x = " + point.X + ", y = " + point.Y);
                //Nếu đã đến được đích thì kết luận có thể đến
                if (point.X == EndPoint.X && point.Y == EndPoint.Y) return true;
                for (int k = 0; k < 4; k++)
                {
                    int New_X = point.X + dx[k];
                    int New_Y = point.Y + dy[k];
                    if (New_X >= 0 && New_Y >= 0 && New_X < 9 && New_Y < 9 && !visited[New_X, New_Y] && BoardColor[New_X, New_Y] == 0)
                    {
                        visited[New_X, New_Y] = true;
                        Point NextPoint = new Point(New_X, New_Y);
                        CheckQueue.Enqueue(NextPoint);
                    }
                }
            }
            //Không đến được đích
            return false;
        }

        // Di chuyển ball từ Selected Cell đến Clicked Cell 
        private void MoveBall(GameCell Src, GameCell Des)
        {
            //Lấy vị trí của ô nguồn và ô đích
            int Src_x = Src.X_Pos;
            int Src_y = Src.Y_Pos;

            int Des_x = Des.X_Pos;
            int Des_y = Des.Y_Pos;
            //Kiểm tra xem đến được đích không
            if (CanMoveBall(new Point(Src.X_Pos, Src.Y_Pos), new Point(Des.X_Pos, Des.Y_Pos)))
            {
                // Lấy màu của ball từ ô nguồn
                int color = BoardColor[Src_x, Src_y];

                //Xóa ball ở ô nguồn
                BoardColor[Src_x, Src_y] = 0;
                Src.RemoveBall();

                Des.ApplyColorToCell(color);

            }
            else
            {
                //Không đến được đích (Về sau có thể đổi thành âm thanh)
                MessageBox.Show("Không thể di chuyển đến ô đã chọn!");
            }

            this.Focus();

            // Reset chọn sau khi di chuyển
            ResetSelection();
        }

        private void ResetSelection()
        {
            // Bỏ đánh dấu ô cũ
            if (FirstSelectedCell != null)
            {
                FirstSelectedCell.GetUnselected();
                FirstSelectedCell = null;
            }
        }

        private void GenerateNewPieces()
        {
            Random random = new Random();
            //Kiểm tra xem ô nào vừa được tạo ball tại đó   

            //Biến SuccessCount đếm số lượng ball được tạo, nếu đủ lượng MaxBallsPerGeneration thì dừng
            int SuccessCount = 0;

            while (SuccessCount < MaxBallsPerGeneration)
            {
                // Thua
                if (Game_Lose())
                {
                    MessageBox.Show("You Lose");
                    Environment.Exit(0);
                }
                //Tìm vị trí ngẫu nhiên để tạo banh
                int RandomX = random.Next(0, 9);
                int RandomY = random.Next(0, 9);
                //Chọn màu ngẫu nhiên cho banh
                int RandomColor = random.Next(1, GameCell.GameColor.Length);
                //Nếu ô đang xét chưa có ball thì tạo ball tại ô đó
                if (BoardColor[RandomX, RandomY] == 0)
                {
                    BoardCells[RandomX, RandomY].ApplyColorToCell(RandomColor);
                    //Nhận vị trí cell và đặt màu mới cho BoardColor tại vị trí cell đó
                    BoardColor[RandomX, RandomY] = RandomColor;
                    SuccessCount++;
                }
            }
        }

        private bool Game_Lose()
        {

            foreach (GameCell cell in BoardCells)
            {
                if (!cell.HasBall())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
