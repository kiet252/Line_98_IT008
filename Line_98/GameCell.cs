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
        //Màu các quả banh
        static internal Color[] GameColor = {
            Color.LightGray, //Màu ô rỗng
            Color.Red, // 1
            Color.Blue,// 2
            Color.Green, // 3
            Color.DarkRed, // 4
            Color.Purple // 5
        };

        //Kích thước từng banh trong ô
        private int LocalBallSize;

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

        public GameCell(int CellSize = 0, int BallsSize = 0, Point CellLocationOnBoard = default, Point CellLocationToIndex = default)
        {
            //Khởi tạo kích thước của Cell
            this.Width = CellSize;
            this.Height = CellSize;

            //Khởi tạo vị trí của cell
            this.Location = CellLocationOnBoard;

            //Mỗi Cell sẽ lưu trữ vị trí hàng - cột của bản thân nó vào Tag
            this.X_Position = CellLocationToIndex.X;
            this.Y_Position = CellLocationToIndex.Y;

            //Kích thước của banh tại ô này sẽ là BallsSize
            this.LocalBallSize = BallsSize;

            //.Font quyết định kích thước của quả banh (Về sau nếu làm banh bằng control khác thì có thể bỏ đi)
            this.Font = new Font("Arial", BallsSize, FontStyle.Regular);

            //Chỉnh Cell (Button) hiện tại có hình phẳng thay vì nút 3D mặc định
            this.FlatStyle = FlatStyle.Flat;

            // Tránh bị Tab focus 
            this.TabStop = false;
        }

        public void ApplyColorToCell(int ColorType = 0)
        {

            if (ColorType != 0)
            {
                //Nếu ô không rỗng, hiến thị ball là "●" (về sau có thể đổi lại)
                this.Text = "●";
                //Dùng border để tránh việc border của Cell tự ý đổi thành ball color
                this.GetUnselected();
                this.ForeColor = GameColor[ColorType];
            }
        }

        public void BallToDefault()
        {
            this.Font = new Font("Arial", LocalBallSize, FontStyle.Regular);
        }

        public void BallToEnlarged()
        {
            this.Font = new Font("Arial", (int)(LocalBallSize * 1.5), FontStyle.Regular);
        }

        public void RemoveBall()
        {
            this.Text = "";
        }

        public bool HasBall()
        {
            
            return (this.Text == "●");
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
