using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Line_98
{
    
    internal class GameCell : Button
    {
        //Tạo Ball tại Cell
        private GameBall Ball;
        //Lưu vị trí của Cell theo index
        private int X_Position;
        private int Y_Position;

        public int X_Pos
        {
            get { return X_Position; }
        }

        public int Y_Pos
        {
            get { return Y_Position; }
        }

        public GameCell(int CellSize = 0, Point CellLocationOnBoard = default, Point CellLocationToIndex = default)
        {
            //Khởi tạo kích thước của Cell
            this.Width = CellSize;
            this.Height = CellSize;

            //Khởi tạo vị trí của cell
            this.Location = CellLocationOnBoard;

            //Mỗi Cell sẽ lưu trữ vị trí hàng - cột của bản thân nó vào Tag
            this.X_Position = CellLocationToIndex.X;
            this.Y_Position = CellLocationToIndex.Y;

            //Khởi tạo Ball tại cell
            Ball = new GameBall(MainGamePanel.GameColor[0]);
            Ball.Visible = false;
            this.Controls.Add(Ball);

            //.Font quyết định kích thước của quả banh (Về sau nếu làm banh bằng control khác thì có thể bỏ đi)
            //this.Font = new Font("Arial", BallsSize, FontStyle.Regular);

            //Chỉnh Cell (Button) hiện tại có hình phẳng thay vì nút 3D mặc định
            this.FlatStyle = FlatStyle.Flat;

            // Tránh bị Tab focus 
            this.TabStop = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // Gọi phương thức OnPaint của lớp cơ sở

            // Kích hoạt khử răng cưa để các đường vẽ mịn hơn
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Định nghĩa màu nền Seashell
            Color baseColor = this.BackColor; // #FFF5EE

            // Định nghĩa màu cho shading (tối hơn một chút và xanh xám)
            Color darkerBaseColor = Color.FromArgb(
                (int)(baseColor.R * 0.95), // Giảm nhẹ kênh R
                (int)(baseColor.G * 0.95), // Giảm nhẹ kênh G
                (int)(baseColor.B * 0.95)  // Giảm nhẹ kênh B
            );
            Color bluishGray = Color.FromArgb(220, 230, 240); // Một màu xanh xám rất nhạt và mềm mại

            // Giả sử "gameCell" là chính Control hiện tại (this).
            // Kích thước của ô vuông là Width và Height của Control.
            var squareRect = new Rectangle(0, 0, Width - 1, Height - 1);

            // --- 1. Vẽ nền phẳng màu Seashell ---
            using (var solidBrush = new SolidBrush(baseColor))
            {
                e.Graphics.FillRectangle(solidBrush, squareRect);
            }

            // --- 2. Áp dụng shading mềm mại cho cạnh phải ---
            // Gradient dọc từ màu hơi tối hơn sang xanh xám
            // Tạo một dải hẹp ở cạnh phải
            int shadeWidth = (int)(Width * 0.1); // Chiều rộng của dải shading (ví dụ 10% chiều rộng của ô)
            if (shadeWidth < 1) shadeWidth = 1; // Đảm bảo không quá nhỏ

            var rightShadeRect = new Rectangle(Width - shadeWidth, 0, shadeWidth, Height);
            using (var rightBrush = new LinearGradientBrush(
                rightShadeRect,
                darkerBaseColor,      // Bắt đầu từ màu hơi tối
                bluishGray,          // Chuyển sang xanh xám ở rìa ngoài
                LinearGradientMode.Vertical // Gradient dọc
            ))
            {
                e.Graphics.FillRectangle(rightBrush, rightShadeRect);
            }

            // --- 3. Áp dụng shading mềm mại cho cạnh dưới ---
            // Gradient ngang từ màu hơi tối hơn Seashell sang xanh xám
            // Tạo một dải hẹp ở cạnh dưới
            int shadeHeight = (int)(Height * 0.1); // Chiều cao của dải shading
            if (shadeHeight < 1) shadeHeight = 1; // Đảm bảo không quá nhỏ

            var bottomShadeRect = new Rectangle(0, Height - shadeHeight, Width, shadeHeight);
            using (var bottomBrush = new LinearGradientBrush(
                bottomShadeRect,
                darkerBaseColor,      // Bắt đầu từ màu hơi tối
                bluishGray,          // Chuyển sang xanh xám ở rìa ngoài
                LinearGradientMode.Horizontal // Gradient ngang
            ))
            {
                e.Graphics.FillRectangle(bottomBrush, bottomShadeRect);
            }
        }

        /// <summary>
        /// Đổi màu của banh tại ô đang xét
        /// </summary>
        /// <param name="ColorType"></param>
        public void ApplyColorToCell(int ColorType = 0)
        {

            if (ColorType > 0)
            {
                //Nếu ô không rỗng, hiến thị ball
                Ball.Visible = true;
                Ball.myColor = MainGamePanel.GameColor[ColorType];
                this.GetUnselected();
            }
        }

        /// <summary>
        /// Đưa banh vào vị trí giữa ô sau khi banh được phóng to/thu nhỏ
        /// </summary>
        private void CenterBall()
        {
            if (Ball != null)
            {
                Ball.Location = new Point(
                    (this.Width - Ball.Width) / 2,
                    (this.Height - Ball.Height) / 2
                );
            }
        }

        /// <summary>
        /// Phóng to banh tại ô đang xét
        /// </summary>
        public void BallToEnlarged()
        {
            Ball.Enlarge();
            CenterBall();
        }
        /// <summary>
        /// Thu nhỏ banh tại ô đang xét
        /// </summary>
        public void BallToDefault()
        {
            Ball.ToDefault();
            CenterBall();
        }

        /// <summary>
        /// Kiểm tra xem banh hiện tại đã phóng lớn chưa 
        /// </summary>
        /// <returns><see langword="true"/> nếu banh đã được phóng lớn ; ngược lại, <see langword="false"/>.</returns>
        public bool isLargedBall() {
            return Ball.isLargedBall();
        }

        /// <summary>
        /// Xóa banh đi tại ô đang xét (Trường hợp màu tại ô đang xét là 0)
        /// </summary>
        public void RemoveBall()
        {
            Ball.Visible = false;
        }

        /// <summary>
        /// Hiển thị ô đang xét đang được chọn
        /// </summary>
        public void GetSelected()
        {
            Ball.StartBouncing();
        }

        /// <summary>
        /// Hiển thị ô đang xét bị bỏ chọn
        /// </summary>
        public void GetUnselected()
        {
            Ball.StopBouncing();
            CenterBall();
        }

        
    }
}
