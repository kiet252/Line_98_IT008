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
    internal class GameBall : PictureBox
    {
        //Màu của banh
        private Color MyColor;

        //Chỉ số phóng lớn banh
        private const double BallScale = 2.5;

        //Hằng quyết định kích cỡ các quả banh
        private const int BallRadius = 8;

        //Các biến bổ trợ animation banh nảy
        private float velocityY = 0;
        private const float initialBounceVelocity = -2f;
        private Timer bounceTimer;
        private bool isBouncing = false;
        private float bounceTime = 0f;
        private float bounceSpeed = 1.6f; // Tốc độ nảy (số chu kỳ mỗi giây)

        /// <summary>
        /// Setter đổi màu banh
        /// </summary>
        public Color myColor
        {
            set
            {
                MyColor = value;
                this.Invalidate();
            }
        }

        public GameBall(Color NewColor)
        {
            MyColor = NewColor;

            this.BackColor = Color.Transparent;
            //Chiều rộng và cao của ball = 2 lần bán kính của ball
            this.Width = BallRadius * 2;
            this.Height = BallRadius * 2;
            this.Enabled = false;

            //Khởi tạo timer phục vụ cho animation banh nảy
            bounceTimer = new Timer();
            bounceTimer.Interval = 16; // ~60 FPS
            bounceTimer.Tick += BounceTimer_Tick;
        }

        private void BounceTimer_Tick(object sender, EventArgs e)
        {
            // Cập nhật thời gian nảy
            bounceTime += 0.016f;

            // Tính giá trị sine (-1 đến 1)
            float sineValue = (float)Math.Sin(bounceTime * bounceSpeed * Math.PI * 2);

            // Biến đổi sine wave từ (-1 đến 1) thành (0 đến 1)
            float normalizedValue = (sineValue + 1f) / 2f;

            // Tính toán vị trí Y (Vị trí trên và dưới)
            int topBoundary = this.Parent.Height / 3;
            int bottomBoundary = this.Parent.Height * 2 / 3 - this.Height;
            int range = bottomBoundary - topBoundary;

            this.Top = topBoundary + (int)(normalizedValue * range);
        }

        /// <summary>
        /// Bắt đầu animation nảy banh
        /// </summary>
        public void StartBouncing()
        {
            if (!isBouncing)
            {
                velocityY = initialBounceVelocity;
                isBouncing = true;
                bounceTimer.Start();
            }
        }

        /// <summary>
        /// Dừng animation nảy banh
        /// </summary>
        public void StopBouncing()
        {
            isBouncing = false;
            bounceTimer.Stop();
            velocityY = 0;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var rect = new Rectangle(0, 0, Width, Height);
            //Tạo hiệu ứng 3D cho banh (Hiệu ứng sáng tối)
            using (var brush = new LinearGradientBrush(
                rect,
                ColorExtensions.Lighten(MyColor, 0.3f),
                ColorExtensions.Darken(MyColor, 0.5f),
                LinearGradientMode.BackwardDiagonal
            ))
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.FillEllipse(brush, 0, 0, Width - 1, Height - 1);
            }
        }

        /// <summary>
        /// Phóng lớn banh
        /// </summary>
        public void Enlarge()
        {
            this.Width = (int)(BallRadius * 2 * BallScale);
            this.Height = (int)(BallRadius * 2 * BallScale);

            this.Invalidate();
        }

        /// <summary>
        /// Thu nhỏ banh
        /// </summary>
        public void ToDefault()
        {
            this.Width = BallRadius * 2;
            this.Height = BallRadius * 2;

            this.Invalidate();
        }

        /// <summary>
        /// Kiểm tra xem banh có được phóng lớn chưa 
        /// </summary>
        /// <returns><see langword="true"/> Nếu banh đã được phóng lớn ; ngược lại, <see
        /// langword="false"/>.</returns>
        public bool isLargedBall() {
            if(Width == BallRadius * 2 * BallScale)
                return true;
            return false;
        }
    }
}
