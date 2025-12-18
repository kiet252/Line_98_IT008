using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;


namespace Line_98
{
    public class MainGamePanel : Panel
    {
        //Form tham chiếu từ Main_Form
        public Main_Form Main_Form;

        //Tạo 1 bảng gồm 9*9 Cells (button) độ lớn 9x9
        private GameCell[,] BoardCells = new GameCell[9, 9];

        //Hằng quyết định màu từng ô
        private Color CellBaseColor = Color.LightBlue;

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
            Color.LimeGreen,
            Color.Blue,
            Color.Gold,
            Color.DarkRed,
            Color.Magenta,
            Color.Cyan
        };

        //Hằng quyết định số lượng banh mới mỗi lần tạo
        private const int MaxBallsPerGeneration = 3;
        private const int MaxBallsPerInitialization = 7;

        //Biến theo dõi xem ô nào mới được chọn, vị trí ô mới được chọn
        private GameCell FirstSelectedCell = null;

        //Các biến bổ trợ cho animation di chuyển banh
        private List<Point> pathPoints = new List<Point>();
        private int currentPathIndex = 0;
        private float animationProgress = 0f;
        private Timer pathTimer;
        private bool isMoving = false;
        private float moveSpeed = 0.8f;
        private GameBall animationBall; // Banh tạm thời cho animation
        private GameCell sourceCell; // Ô nguồn
        private GameCell destinationCell; // Ô đích
        private int movingBallColor; // Màu của banh đang di chuyển
        private bool isAnimating = false;

        //Display
        // Điểm số
        private int gameScore;
        private int gameHishtestScore;
        private PictureBox[] currentScore = new PictureBox[5];          // Điểm số hiện tại 
        private PictureBox[] highestScore = new PictureBox[5];          // Điểm số cao nhất
        private PictureBox[] gameTime = new PictureBox[5];              // Thời gian
        private Timer gameTimer = new Timer();
        private int gameTimeSec = 0;                                    // Thời gian ván game (đơn vị: giây)
        public int GameTime { get { return gameTimeSec; } set { gameTimeSec = value; } }// Lấy thông tin thời gian 
        public int GameScore { get { return gameScore; } set { gameScore = value; } }   // Lấy thông tin điểm hiện tại
        public int[,] GameBoardColor { get { return BoardColor; } }     // Lấy thông tin BoardColor để lưu game 

        public void setGameBoardColor(int row, int col, int color) {    // Thiết lập lại BoardCell khi load game
            BoardColor[row, col] = color;

            if(color > 0) {
                BoardCells[row, col].BallToEnlarged();
                BoardCells[row, col].ApplyColorToCell(color);
            }else if(color < 0) {
                BoardCells[row, col].BallToDefault();
                BoardCells[row, col].ApplyColorToCell(color);
            } else {
                BoardCells[row, col].RemoveBall();
            }
        }

        //Constructor
        public MainGamePanel(Main_Form mainForm)
        {
            this.Main_Form = mainForm;

            this.Location = new Point(10, 20);
            this.Size = new Size(700, 800);
            //this.BackColor = Color.Black;

            gameTimer.Interval = 1000;
            
            InitializePanel();

            //
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            //Tạo các banh khi mới vào game
            GenerateFirstPieces(MaxBallsPerInitialization);

            //Tạo các banh chuẩn bị biến thành banh to
            GenerateNewPieces();

            //Tạo PictureBox cho điểm hiện tại 
            BuildCurrentScore();
            //Tạo PictureBox cho điểm cao nhất 
            BuildHighestScore();
            //Tạo PictureBox cho thời gian
            BuildGameTime();

            //Hiển thị điểm hiện tại lên Panel 
            RepaintCurrentScore();
            //Hiển thị điểm cao nhất lên Panel 
            RepaintHighestScore();
            //Hiển thị thời gian lên Panel
            RepaintGameTime();

            //Lưu vào bảng PrevBroadColor để Undo
            GameAlgorithm.GetPrevBoard(BoardColor, 0);

            this.Invalidate();
        }

        /// <summary>
        /// Đặt nền đen tăng tương phản cho hiển thị điểm số 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            using (SolidBrush brush = new SolidBrush(Color.Black)) {
                Rectangle rect = new Rectangle(0, 8, this.Width, 70);
                e.Graphics.FillRectangle(brush, rect);
            }
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

            // Bắt đầu tính điểm
            gameScore = 0;
            // Bắt đầu tính giờ
            gameTimeSec = 0;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

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
                    GameCell Cell = new GameCell(CellSize, new Point(Col * CellSize, Row * CellSize), new Point(Row, Col));
                    Cell.BackColor = CellBaseColor;
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
            if (isAnimating) return;

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

                //Âm thanh chọn banh
                GameSound.PlaySelectSound();
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

                    //Âm thanh chọn banh
                    GameSound.PlaySelectSound();
                }
                //Không phải thì tiến hành di chuyển 
                else if (BoardColor[Clicked_Cell.X_Pos, Clicked_Cell.Y_Pos] <= 0)
                {
                    //Di chuyển banh
                    MoveBall(FirstSelectedCell, Clicked_Cell);
                }
                else
                {
                    //Nếu chọn một banh khác, chuyển lựa chọn sang bang được chọn tiếp theo
                    FirstSelectedCell.GetUnselected();
                    FirstSelectedCell = Clicked_Cell;
                    Clicked_Cell.GetSelected();

                    //Âm thanh chọn banh
                    GameSound.PlaySelectSound();
                }
            }

        }

        /// <summary>
        /// Di chuyển ball từ Selected Cell đến Clicked Cell 
        /// </summary>
        /// <param name="Src"></param>
        /// <param name="Des"></param>
        /// <returns></returns>
        internal bool MoveBall(GameCell Src, GameCell Des)
        {
            //Lấy vị trí của ô nguồn và ô đích
            int Src_x = Src.X_Pos;
            int Src_y = Src.Y_Pos;

            int Des_x = Des.X_Pos;
            int Des_y = Des.Y_Pos;

            //Kiểm tra xem đến được đích không
            bool CanMoveToDes = GameAlgorithm.CanMoveBall(new Point(Src_x, Src_y), new Point(Des_x, Des_y), ref BoardColor);

            if (CanMoveToDes)
            {
                //Lấy bảng màu banh trước khi di chuyển
                GameAlgorithm.GetPrevBoard(BoardColor, gameScore);

                //Âm thanh banh di chuyển
                GameSound.PlayMoveSound();

                //Animation chạy banh
                CreateMoveAnimation(Src, Des);

                //Xóa ball ở ô nguồn
                BoardColor[Src_x, Src_y] = 0;
                Src.RemoveBall();

                // Reset chọn sau khi di chuyển
                this.Focus();
                ResetSelection();
            }
            else
            {
                //Không đến được đích
                GameSound.PlayCantMoveSound();
            }
            
            return CanMoveToDes;
        }

        /// <summary>
        /// Tạo animation di chuyển banh từ ô nguồn đến ô đích
        /// </summary>
        private void CreateMoveAnimation(GameCell src, GameCell dest)
        {
            // Tìm đường đi
            List<Point> path = GameAlgorithm.FindPath(new Point(src.X_Pos, src.Y_Pos), new Point(dest.X_Pos, dest.Y_Pos), ref BoardColor);

            if (path == null || path.Count < 2)
            {
                path = new List<Point> {
                    new Point(src.X_Pos, src.Y_Pos),
                    new Point(dest.X_Pos, dest.Y_Pos)
                };
            }

            sourceCell = src;
            destinationCell = dest;
            movingBallColor = BoardColor[src.X_Pos, src.Y_Pos];

            // Tạo banh tạm thời cho animation
            animationBall = new GameBall(GameColor[Math.Abs(movingBallColor)]);
            animationBall.Size = new Size(CellSize, CellSize);
            animationBall.BackColor = CellBaseColor;
            animationBall.Enlarge();

            // Đặt vị trí ban đầu
            Point startPos = GetBallCenterPosition(new Point(src.X_Pos, src.Y_Pos));
            animationBall.Location = new Point(startPos.X - animationBall.Width / 2, startPos.Y - animationBall.Height / 2);

            CellBoard.Controls.Add(animationBall);
            animationBall.BringToFront();

            // Bắt đầu animation
            StartPathAnimation(path);
        }

        /// <summary>
        /// Bắt đầu animation
        /// </summary>
        private void StartPathAnimation(List<Point> path)
        {
            pathPoints = path;
            currentPathIndex = 0;
            animationProgress = 0f;
            isMoving = true;
            isAnimating = true;

            //Vô hiệu hóa board khi đang thực hiện di chuyển
            CellBoard.Enabled = false;

            if (pathTimer == null)
            {
                pathTimer = new Timer();
                pathTimer.Interval = 16; // ~60 FPS
                pathTimer.Tick += PathTimer_Tick;
            }

            pathTimer.Start();
        }

        private void PathTimer_Tick(object sender, EventArgs e)
        {
            if (!isMoving || animationBall == null || pathPoints.Count < 2 || currentPathIndex >= pathPoints.Count - 1)
            {
                FinishAnimation();
                return;
            }

            // Tăng tiến trình animation
            animationProgress += moveSpeed;

            // Chuyển sang đoạn đường tiếp theo nếu cần
            if (animationProgress >= 1.0f)
            {
                animationProgress = 0f;
                currentPathIndex++;

                if (currentPathIndex >= pathPoints.Count - 1)
                {
                    FinishAnimation();
                    return;
                }
            }

            // Tính toán vị trí hiện tại
            Point startCell = pathPoints[currentPathIndex];
            Point endCell = pathPoints[currentPathIndex + 1];

            // Chuyển từ tọa độ ô sang tọa độ pixel cho trung tâm banh
            Point startPixel = GetBallCenterPosition(startCell);
            Point endPixel = GetBallCenterPosition(endCell);

            // Di chuyển banh
            int currentX = (int)(startPixel.X + (endPixel.X - startPixel.X) * animationProgress);
            int currentY = (int)(startPixel.Y + (endPixel.Y - startPixel.Y) * animationProgress);

            animationBall.Location = new Point(currentX - animationBall.Width / 2, currentY - animationBall.Height / 2);
        }

        /// <summary>
        /// Lấy vị trí trung tâm của banh trong ô
        /// </summary>
        private Point GetBallCenterPosition(Point cell)
        {
            int cellX = cell.Y * CellSize; // Chuyển từ (row,col) sang (x,y)
            int cellY = cell.X * CellSize;
            int centerX = cellX + CellSize / 2;
            int centerY = cellY + CellSize / 2;
            return new Point(centerX, centerY);
        }

        private void FinishAnimation()
        {
            isMoving = false;
            isAnimating = false;
            //Kích hoạt lại board
            CellBoard.Enabled = true;

            pathTimer?.Stop();

            // Cập nhật board state sau khi animation hoàn thành
            if (sourceCell != null && destinationCell != null && animationBall != null)
            {
                // Xóa banh tạm
                CellBoard.Controls.Remove(animationBall);
                animationBall.Dispose();
                animationBall = null;

                //Nếu tại ô đích có banh nhỏ, phóng to banh nhỏ đó và chuyển nó đến vị trí bất kì khác
                if (BoardColor[destinationCell.X_Pos, destinationCell.Y_Pos] < 0) GenerateFirstPieces(1, -BoardColor[destinationCell.X_Pos, destinationCell.Y_Pos]);

                // Cập nhật ô đích với banh thật
                BoardColor[destinationCell.X_Pos, destinationCell.Y_Pos] = movingBallColor;
                destinationCell.ApplyColorToCell(movingBallColor);
                destinationCell.BallToEnlarged();

                    
                // Kiểm tra và xử lý điểm số
                int gamePoint = GameAlgorithm.CheckAndRemoveBall(destinationCell, ref BoardColor, ref BoardCells);
                if (gamePoint > 0)
                {
                    ScoreAndDisplayUpdate(gamePoint);
                }
                else
                {
                    BallEnvol();
                    GenerateNewPieces();
                }
            }

            sourceCell = null;
            destinationCell = null;
        }

        /// <summary>
        /// Tạo những banh lớn đầu tiên khi trò chơi bắt đầu hoặc chuyển banh nhỏ vừa bị đè mất sang vị trí bất kì
        /// </summary>
        /// <remarks>
        /// ToGenerateColor = 0: giá trị mặc định đánh dấu các banh lớn được tạo ngẫu nhiên sẽ mang màu ngẫu nhiên
        /// ToGenerateColor != 0: giá trị màu của banh sẽ có màu được truyền vào
        /// </remarks>
        private void GenerateFirstPieces(int GenerateCounts, int ToGenerateColor = 0)
        {
            Random random = new Random();
            for (int i = 0; i < GenerateCounts; i++)
            {
                //Tìm vị trí ngẫu nhiên để tạo banh
                int RandomX = random.Next(0, 9);
                int RandomY = random.Next(0, 9);
                //Chọn màu ngẫu nhiên cho banh
                int RandomColor = (ToGenerateColor == 0 ? random.Next(1, GameColor.Length) : ToGenerateColor);
                //Nếu ô đang xét chưa có ball thì tạo ball tại ô đó
                if (BoardColor[RandomX, RandomY] == 0)
                {
                    BoardCells[RandomX, RandomY].ApplyColorToCell(RandomColor);
                    BoardCells[RandomX, RandomY].BallToEnlarged();
                    //Nhận vị trí cell và đặt màu mới cho BoardColor tại vị trí cell đó
                    BoardColor[RandomX, RandomY] = RandomColor;
                }
                //Trong trường hợp vị trí ngẫu nhiên không rỗng, tìm vị trí khác
                else
                    i--; 
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
                // Kết thúc
                if (isFinished())
                {
                    gameTimer.Stop();
                    GameEnd();
                    return;
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
                    BoardColor[cell.X_Pos, cell.Y_Pos] = -BoardColor[cell.X_Pos, cell.Y_Pos]; 

                    // Hiển thị điểm lên Main_Form
                    int gamePoint = GameAlgorithm.CheckAndRemoveBall(cell, ref BoardColor, ref BoardCells); // Phóng to banh rồi xét xem có banh nào ăn được không 
                    
                    if (gamePoint > 0) {
                        // Hiển thị điểm lên Main_Form
                        ScoreAndDisplayUpdate(gamePoint);
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
        private bool isFinished()
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
        /// Tính điểm hiện tại và cập nhật điểm lên Panel 
        /// </summary>
        /// <param name="numBall"></param>
        private void ScoreAndDisplayUpdate(int numBall) {
            if (numBall < 5)
                return;

             gameScore += (numBall - 4) * (numBall + 5) / 2;

            RepaintCurrentScore();

        } 

        /// <summary>
        /// Thêm PictureBox hiển thị điểm số hiện tại
        /// </summary>
        private void BuildCurrentScore() {
            int currentScore_PosX = this.Width - 180;

            for (int i = 0; i < 5; ++i) {
                currentScore[i] = new PictureBox();
                currentScore[i].BackColor = this.BackColor;
                currentScore[i].Location = new Point(currentScore_PosX + 22 * i, 25);
                currentScore[i].Size = Number.SCORE_SIZE;
                currentScore[i].SizeMode = PictureBoxSizeMode.StretchImage;
                this.Controls.Add(currentScore[i]);
            }
        }

        /// <summary>
        /// Cập nhật điểm số hiện tại 
        /// </summary>
        public void RepaintCurrentScore() {
            int[] digits = Number.Split(gameScore);

            for(int i = 0; i < 5; ++i) {
                currentScore[i].Image = Number.bmpSCORE_Digits[digits[i]];
            }
        }

        /// <summary>
        /// Thêm PictureBox hiển thị điểm số cao nhất (Cập nhật lại sau khi thêm Database)
        /// </summary>
        private void BuildHighestScore() {
            int highestScore_PosX = 45;

            for(int i = 0; i < 5; ++i) {
                highestScore[i] = new PictureBox();
                highestScore[i].BackColor = this.BackColor;
                highestScore[i].Location = new Point(highestScore_PosX + 22 * i, 25);
                highestScore[i].Size = Number.SCORE_SIZE;
                highestScore[i].SizeMode = PictureBoxSizeMode.StretchImage;
                this.Controls.Add(highestScore[i]);
            }
        }

        /// <summary>
        /// Cập nhật điểm số cao nhất (Cập nhật lại sau khi thêm Database)
        /// </summary>
        private void RepaintHighestScore() {
            int[] digits = Number.Split(gameHishtestScore);

            for(int i = 0; i < 5; ++i) {
                highestScore[i].Image = Number.bmpSCORE_Digits[digits[i]];
            }
        }

        /// <summary>
        /// Thêm PictureBox thể hiện thời gian
        /// </summary>
        private void BuildGameTime() {
            int timePos = this.Width / 2 - 38;

            for (int i = 0; i < 5; ++i) {
                gameTime[i] = new PictureBox();
                gameTime[i].BackColor = this.BackColor;
                gameTime[i].Location = new Point(timePos + 10 * i, 40);
                gameTime[i].Size = Number.TIME_SIZE;
                gameTime[i].SizeMode = PictureBoxSizeMode.StretchImage;
                this.Controls.Add(gameTime[i]);
            }
        }

        /// <summary>
        /// Cập nhật thời gian
        /// </summary>
        public void RepaintGameTime() {
            int[] digit = Number.Split(gameTimeSec);

            for(int i = 0; i < 5; ++i) {
                gameTime[i].Image = Number.bmpTIME_Digits[digit[i]];
            }
        }

        /// <summary>
        /// Tăng thời gian ván game lên 1 (đơn vị: giây)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameTimer_Tick(object sender, EventArgs e) {
            gameTimeSec++;
            RepaintGameTime();
        }

        /// <summary>
        /// Hiển thị thông báo cho người chơi biết game đã kết thúc
        /// Đồng thời hiển thị thời gian và điểm số
        /// </summary>
        private void GameEnd() {
            if(gameScore > gameHishtestScore) {
                MessageBox.Show("Chúc mừng bạn đạt được điểm cao nhất!\n" +
                    $"Điểm của bạn là: {gameScore}\n" +
                    $"Tổng thời gian chơi: {gameTimeSec}", "WIN", MessageBoxButtons.OK);
            } else {
                MessageBox.Show("Cố gắng vượt điểm cao nhất nhé!\n" +
                    $"Điểm của bạn là: {gameScore}\n" +
                    $"Tổng thời gian chơi: {gameTimeSec}", "LOSE", MessageBoxButtons.OK);
            }

            DialogResult ContinueGame = MessageBox.Show("Ban có muốn chơi tiếp?", "Chơi tiếp", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ContinueGame == DialogResult.No)
            {
                Environment.Exit(0);
            }
            else GameReset();
        }

        private void GameReset() 
        {
            //Chỉnh tất cả các ô thành ô rỗng
            foreach (GameCell cell in BoardCells)
            {
                BoardColor[cell.X_Pos, cell.Y_Pos] = 0;
                cell.RemoveBall();
            }
            // Bắt đầu tính điểm
            gameScore = 0;
            // Bắt đầu tính giờ
            gameTimeSec = 0;
            // Đổi lại điểm
            RepaintCurrentScore();
            //Khởi động lại bộ đếm thời gian
            gameTimer.Start();
            //Tạo các banh khi mới vào game
            GenerateFirstPieces(MaxBallsPerInitialization);
            //Tạo banh nhỏ
            GenerateNewPieces();
        }
    }
}
