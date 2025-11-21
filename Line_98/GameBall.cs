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
        private const double BallScale = 3;

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

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; // Kích hoạt khử răng cưa

            var ballRect = new Rectangle(0, 0, Width - 1, Height - 1);

            // --- 1. Tạo bóng (shading) Hướng tâm cơ bản (Light -> Dark) ---
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(ballRect);

                using (var baseBrush = new PathGradientBrush(path))
                {
                    // Màu trung tâm: Khu vực sáng nhất
                    Color centerColor = Utilities.ColorExtensions.Lighten(MyColor, 0.5f);

                    // Màu xung quanh: Bóng tối sâu/cạnh tối
                    Color darkEdgeColor = Utilities.ColorExtensions.Darken(MyColor, 0.6f);

                    baseBrush.CenterColor = centerColor;
                    baseBrush.SurroundColors = new Color[] { darkEdgeColor };

                    // Định vị tâm "ánh sáng" ở khu vực phía trên bên phải
                    baseBrush.CenterPoint = new PointF(
                        Width * 0.65f,
                        Height * 0.35f
                    );

                    e.Graphics.FillEllipse(baseBrush, ballRect);
                }
            }

            // --- 2. Điểm sáng phản chiếu chính (Mạnh, Trắng sáng) ---
            int hSize = (int)(Width * 0.25);
            int hX = (int)(Width * 0.75) - hSize; // Vị trí X 
            int hY = (int)(Height * 0.25) - hSize; // Vị trí Y

            var highlightRect = new Rectangle(hX, hY, hSize, hSize);

            using (var highlightPath = new GraphicsPath())
            {
                highlightPath.AddEllipse(highlightRect);
                using (var highlightBrush = new PathGradientBrush(highlightPath))
                {
                    highlightBrush.CenterColor = Color.FromArgb(255, Color.White); // Trắng hoàn toàn
                    highlightBrush.SurroundColors = new Color[] { Color.FromArgb(0, Color.Transparent) }; // Chuyển dần sang trong suốt

                    highlightBrush.CenterPoint = new PointF(
                        highlightRect.X + highlightRect.Width * 0.5f,
                        highlightRect.Y + highlightRect.Height * 0.5f
                    );

                    e.Graphics.FillEllipse(highlightBrush, highlightRect);
                }
            }

            // --- 3. VIỀN SÁNG PHỤ (RIM LIGHT) Ở CẠNH TRÊN BÊN PHẢI ---
            // Vẽ một hình elip nhỏ, sáng, mờ chồng lên ở rìa trên-phải.
            int rimSize = (int)(Width * 0.8);
            var rimRect = new Rectangle(
                (int)(Width * 0.2),  // Bắt đầu từ phía trên-trái
                (int)(Height * 0.1), // Rất gần cạnh trên
                rimSize, rimSize
            );

            using (var rimBrush = new LinearGradientBrush(
                rimRect,
                Utilities.ColorExtensions.Lighten(MyColor, 0.6f), // Màu sáng cao
                Color.FromArgb(0, Color.Transparent),   // Chuyển dần sang trong suốt
                LinearGradientMode.ForwardDiagonal // Hướng gradient từ sáng ra ngoài
            ))
            {
                // Điều chỉnh ColorBlend để làm cho phần sáng rất mỏng và ở rìa
                ColorBlend blend = new ColorBlend();
                blend.Colors = new Color[] {
                Color.FromArgb(0, Color.Transparent),   // Phần lớn hình cầu
                Color.FromArgb(50, Utilities.ColorExtensions.Lighten(MyColor, 0.6f)), // Dải sáng mờ
                Color.FromArgb(0, Color.Transparent)    // Phía bên kia
            };
                // Tạo dải sáng hẹp, mờ ở rìa
                blend.Positions = new float[] { 0.0f, 0.05f, 1.0f };
                rimBrush.InterpolationColors = blend;

                // Cắt bớt phần rìa ngoài của hình elip mờ này để chỉ thấy viền sáng
                e.Graphics.SetClip(ballRect, CombineMode.Intersect);
                e.Graphics.FillEllipse(rimBrush, rimRect);
                e.Graphics.ResetClip(); // Đặt lại clip
            }

            // --- 4. Điểm sáng phản chiếu phụ mờ (Cạnh phía dưới bên trái) ---
            int secondarySize = (int)(Width * 0.1);
            int sX = (int)(Width * 0.05);
            int sY = (int)(Height * 0.85) - secondarySize;

            var secondaryRect = new Rectangle(sX, sY, secondarySize, secondarySize);

            using (var secondaryBrush = new LinearGradientBrush(
                secondaryRect,
                Color.FromArgb(50, Color.White), // Độ mờ giảm
                Color.FromArgb(0, Color.Transparent),
                LinearGradientMode.ForwardDiagonal
            ))
            {
                e.Graphics.FillEllipse(secondaryBrush, secondaryRect);
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
