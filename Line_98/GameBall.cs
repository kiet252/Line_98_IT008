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
        private Color BallColor;
        private int BallRadius;

        public GameBall(Color ballColor, int ballRadius = 0)
        {
            BallColor = ballColor;
            BallRadius = ballRadius;

            this.Width = ballRadius;
            this.Height = ballRadius;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //Yêu cầu vẽ các phần của GameCell trước khi vẽ ball
            base.OnPaint(e);
            //Vẽ ball
            using (SolidBrush brush = new SolidBrush(BallColor))
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.FillEllipse(brush, 0, 0, Width - 1, Height - 1);
            }
        }

    }
}
