using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Line_98
{
    internal class GameBall : PictureBox
    {
        private Color MyColor;
        private const double BallScale = 1.5;

        //Hằng quyết định kích cỡ các quả banh
        private const int BallRadius = 10;

        public GameBall(Color NewColor)
        {
            MyColor = NewColor;

            this.BackColor = Color.Transparent;
            this.Width = BallRadius * 2;
            this.Height = BallRadius * 2;
            this.Enabled = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //Yêu cầu vẽ các phần của GameCell trước khi vẽ ball
            base.OnPaint(e);
            //Vẽ ball
            using (SolidBrush brush = new SolidBrush(MyColor))
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.FillEllipse(brush, 0, 0, Width - 1, Height - 1);
            }
        }

        public void Enlarge()
        {
            this.Width = (int)(BallRadius * 2 * BallScale);
            this.Height = (int)(BallRadius * 2 * BallScale);

            this.Invalidate();
        }

        public void ToDefault()
        {
            this.Width = BallRadius * 2;
            this.Height = BallRadius * 2;

            this.Invalidate();
        }

        public Color myColor{
            set
            {
                MyColor = value;
                this.Invalidate();
            }
        }
    }
}
