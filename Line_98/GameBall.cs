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
