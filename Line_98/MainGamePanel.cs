using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

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

        /// <summary>
        /// Lưu màu quân cờ. 
        /// </summary>
        /// <remarks>
        /// Mỗi ô có các trạng thái:
        /// <list type="bullet">
        /// <item><description>0: Ô trống, không có banh</description></item>
        /// <item><description>Giá trị âm: Ô chứa banh nhỏ</description></item>
        /// <item><description>Giá trị dương: Ô chứa banh lớn</description></item>
        /// </list>
        /// </remarks>
        private int[,] BoardColor = new int[9, 9];

        /// <summary>
        /// Mảng màu sắc cho các quả banh trong game
        /// </summary>
        /// <remarks>
        /// Hệ thống tự động nhận biết khi thêm/bớt màu
        /// Không xóa Color.LightGray (index 0, tượng trưng cho ô rỗng)
        /// </remarks>
        static internal Color[] GameColor = {
            Color.LightGray, //0 - Màu ô rỗng (Không xóa!)
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Gold,
            Color.DarkRed,
            Color.Magenta,
            Color.Cyan
        };

        // Điểm số
        private int gameScore;

        //Hằng quyết định số lượng banh mới mỗi lần tạo
        private const int MaxBallsPerGeneration = 3;
        private const int MaxBallsPerInitialization = 7;

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
            GenerateFirstPieces();

            //Tạo các banh chuẩn bị biến thành banh to
            GenerateNewPieces();
        }

        //Khởi tạo bảng CellBoard
        public void InitializePanel()
        {
            CellBoard = new Panel();
            CellBoard.Size = new Size(630, 630);
            CellBoard.BackColor = Main_Form.BackColor;
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

            // Bắt đầu tính điểm
            gameScore = 0;

            for (int Row = 0; Row < 9; Row++)
            {
                for (int Col = 0; Col < 9; Col++)
                {
                    //Khởi tạo từng Cell
                    GameCell Cell = new GameCell(CellSize, new Point(Col * CellSize, Row * CellSize), new Point(Row, Col));
                    //Tham chiếu Cell mới vào Cell trong CellBoard, để có thể sử dụng trong các method sau
                    BoardCells[Row, Col] = Cell;

                    //Khi Cell được nhấp vào, Cell sẽ thực hiện hàm CellClick 
                    Cell.Click += CellClick;

                    //Thêm Cell vừa tạo vào CellBoard
                    CellBoard.Controls.Add(Cell);

                    //Cho màu ban đầu của các ô là 0 (Không có ball)
                    BoardColor[Row, Col] = 0;
                }
            }
        }

        /// <summary>
        /// thao tác mà Cell đó thực hiện khi được nhấp vào
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellClick(object sender, EventArgs e)
        {
            //Check xem cell vừa ấn vào có vị trí ở đâu thông qua Cell.Tag
            GameCell Clicked_Cell = (GameCell)sender;

            //Trường hợp 1: Lần đầu tiên chọn ô -> Hiển thị là ô đang được chọn
            if (FirstSelectedCell == null)
            {
                //Nếu ô được chọn không có banh hoặc là banh bé, không có gì xảy ra
                if (BoardColor[Clicked_Cell.X_Pos, Clicked_Cell.Y_Pos] <= 0) return;

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
                else if (BoardColor[Clicked_Cell.X_Pos, Clicked_Cell.Y_Pos] <= 0)
                {
                    //Di chuyển thành công thì mới tạo banh mới
                    if (MoveBall(FirstSelectedCell, Clicked_Cell))
                    {
                        BallEnvol();
                        GenerateNewPieces();
                    }              
                }
            }

        }

        /// <summary>
        /// Di chuyển ball từ Selected Cell đến Clicked Cell 
        /// </summary>
        /// <param name="Src"></param>
        /// <param name="Des"></param>
        /// <returns></returns>
        private bool MoveBall(GameCell Src, GameCell Des)
        {
            //Lấy vị trí của ô nguồn và ô đích
            int Src_x = Src.X_Pos;
            int Src_y = Src.Y_Pos;

            int Des_x = Des.X_Pos;
            int Des_y = Des.Y_Pos;

            //Kiểm tra xem đến được đích không
            bool CanMoveToDes = CanMoveBall(new Point(Src_x, Src_y), new Point(Des_x, Des_y));
            
            if (CanMoveToDes)
            {
                // Lấy màu của ball từ ô nguồn
                int color = BoardColor[Src_x, Src_y];

                //Xóa ball ở ô nguồn
                BoardColor[Src_x, Src_y] = 0;
                Src.RemoveBall();

                BoardColor[Des_x, Des_y] = color;
                Des.ApplyColorToCell(color);

                int gamePoint = CheckAndRemoveBall(Des);
                if (gamePoint > 0) {
                    // Hiển thị điểm lên Main_Form
                    gameScore += ((gamePoint - 4) * (gamePoint - 4));
                    Main_Form.UpdateScore(gameScore);
                } else
                    Des.BallToEnlarged();
            }
            else
            {
                //Không đến được đích (Về sau có thể đổi thành âm thanh)
                MessageBox.Show("Không thể di chuyển đến ô đã chọn!");
            }

            this.Focus();
            // Reset chọn sau khi di chuyển
            ResetSelection();
            return CanMoveToDes;
        }

        /// <summary>
        /// Kiểm tra xem có di chuyển được đến ô đã chọn
        /// </summary>
        /// <param name="StartPoint"></param>
        /// <param name="EndPoint"></param>
        /// <returns></returns>
        private bool CanMoveBall(Point StartPoint, Point EndPoint)
        {
            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };
            bool[,] visited = new bool[9, 9];
            //Duyệt BFS xem có thể đến được đích từ điểm bắt đầu
            Queue<Point> CheckQueue = new Queue<Point>();
            CheckQueue.Enqueue(StartPoint);

            while (CheckQueue.Count > 0)
            {
                Point point = CheckQueue.First();
                CheckQueue.Dequeue();

                //Console.WriteLine("x = " + point.X + ", y = " + point.Y);

                //Nếu đã đến được đích thì kết luận có thể đến
                if (point.X == EndPoint.X && point.Y == EndPoint.Y) return true;
                for (int k = 0; k < 4; k++)
                {
                    int New_X = point.X + dx[k];
                    int New_Y = point.Y + dy[k];
                    if (New_X >= 0 && New_Y >= 0 && New_X < 9 && New_Y < 9 && !visited[New_X, New_Y] && BoardColor[New_X, New_Y] <= 0)
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
        /// <summary>
        /// Tạo những banh lớn đầu tiên khi trò chơi bắt đầu
        /// </summary>
        private void GenerateFirstPieces()
        {
            Random random = new Random();
            for (int i = 0; i < MaxBallsPerInitialization; i++)
            {
                //Tìm vị trí ngẫu nhiên để tạo banh
                int RandomX = random.Next(0, 9);
                int RandomY = random.Next(0, 9);
                //Chọn màu ngẫu nhiên cho banh
                int RandomColor = random.Next(1, GameColor.Length);
                //Nếu ô đang xét chưa có ball thì tạo ball tại ô đó
                if (BoardColor[RandomX, RandomY] == 0)
                {
                    BoardCells[RandomX, RandomY].ApplyColorToCell(RandomColor);
                    BoardCells[RandomX, RandomY].BallToEnlarged();
                    //Nhận vị trí cell và đặt màu mới cho BoardColor tại vị trí cell đó
                    BoardColor[RandomX, RandomY] = RandomColor;
                }
            }
        }
        /// <summary>
        /// Tạo banh nhỏ
        /// </summary>
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
                int RandomColor = random.Next(1, GameColor.Length);
                //Nếu ô đang xét chưa có ball thì tạo ball tại ô đó
                if (BoardColor[RandomX, RandomY] == 0)
                {
                    BoardCells[RandomX, RandomY].ApplyColorToCell(RandomColor);
                    BoardCells[RandomX, RandomY].BallToDefault();
                    //Nhận vị trí cell và đặt màu mới cho BoardColor tại vị trí cell đó
                    BoardColor[RandomX, RandomY] = -RandomColor;
                    SuccessCount++;
                }
            }
        }

        /// <summary>
        /// Phóng lớn banh đang chuẩn bị xuất hiện
        /// </summary>
        private void BallEnvol()
        {
            foreach (GameCell cell in BoardCells)
            {
                if (BoardColor[cell.X_Pos, cell.Y_Pos] < 0)
                {
                    cell.BallToEnlarged();
                    //Đổi giá trị màu tại ô đang xét thành giá trị màu dương
                    BoardColor[cell.X_Pos, cell.Y_Pos] = -BoardColor[cell.X_Pos, cell.Y_Pos]; // ? tại sao âm vậy ?

                    // Hiển thị điểm lên Main_Form
                    int gamePoint = CheckAndRemoveBall(cell);
                    if (gamePoint > 0) {
                        // Hiển thị điểm lên Main_Form
                        gameScore += ((gamePoint - 4) * (gamePoint - 4));
                        Main_Form.UpdateScore(gameScore);
                    }
                }
            }
        }

        /// <summary>
        /// Bỏ đánh dấu ô cũ
        /// </summary>
        private void ResetSelection()
        {
            if (FirstSelectedCell != null)
            {
                FirstSelectedCell.GetUnselected();
                FirstSelectedCell = null;
            }
        }

        /// <summary>
        /// Kiểm tra thua khi tất cả các ô đều có banh 
        /// </summary>
        /// <returns>
        /// true nếu thua, false nếu chưa thua
        /// </returns>
        private bool Game_Lose()
        {

            foreach (GameCell cell in BoardCells)
            {
                if (BoardColor[cell.X_Pos, cell.Y_Pos] == 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Kiểm tra xem có ăn được banh không 
        /// </summary>
        /// <param name="Src"></param>
        /// <returns>
        /// Trả về số banh ăn được
        /// </returns>
        int CheckAndRemoveBall(GameCell Src) {
            int r = Src.X_Pos;
            int c = Src.Y_Pos;
            int color = BoardColor[r, c];

            int g_Point = 0; // Điểm số là số banh ăn được 

            // Các hướng: ngang, dọc, chéo chính, chéo phụ
            int[] dx = { 0, 1, 1, 1 };
            int[] dy = { 1, 0, 1, -1 };
            bool isRemoveable = false;
            List<GameCell> toRemove = new List<GameCell>();

            for(int dir = 0; dir < 4; ++dir) {
                List<GameCell> line = new List<GameCell>();
                line.Add(Src);

                // đi xuôi hướng
                int x = r + dx[dir];
                int y = c + dy[dir];
                while(x >= 0 && x < 9 && y >= 0 && y < 9 && BoardColor[x, y] == color && BoardCells[x, y].isLargedBall()) { 
                    line.Add(BoardCells[x, y]);
                    x += dx[dir];
                    y += dy[dir];
                }

                // đi ngược hướng
                x = r - dx[dir];
                y = c - dy[dir];
                while(x >= 0 && x < 9 && y >= 0 && y < 9 && BoardColor[x, y] == color && BoardCells[x, y].isLargedBall()) {
                    line.Add(BoardCells[x, y]);
                    x -= dx[dir];
                    y -= dy[dir];
                }

                // Nếu có ≥ 5 quả liên tiếp cùng màu
                if(line.Count >= 5) {
                    isRemoveable = true;
                    foreach(GameCell cell in line) {
                        toRemove.Add(cell);
                    }
                }
            }
            
            // Xóa banh
            if (isRemoveable) {
                g_Point += toRemove.Count;
                foreach(GameCell cell in toRemove) {
                    BoardColor[cell.X_Pos, cell.Y_Pos] = 0;
                    cell.RemoveBall();
                }
                SoundPlayer player = new SoundPlayer(Properties.Resources.ding); // Âm thanh ding.wav
                player.Play();
            }

            return g_Point;
        }
    }
}
