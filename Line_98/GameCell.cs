using System;
using System.Collections.Generic;
using System.Drawing;
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

        public void BallToEnlarged()
        {
            Ball.Enlarge();
            CenterBall();
        }

        public void BallToDefault()
        {
            Ball.ToDefault();
            CenterBall();
        }

        public void RemoveBall()
        {
            Ball.Visible = false;
        }

        public void GetSelected()
        {
            this.FlatAppearance.BorderSize = 3;
            this.FlatAppearance.BorderColor = Color.Red;
        }

        public void GetUnselected()
        {
            this.FlatAppearance.BorderSize = 1;
            this.FlatAppearance.BorderColor = Color.Black;
        }

        
    }
}
