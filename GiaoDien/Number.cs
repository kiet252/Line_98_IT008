using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Utilities {
    public class Number {
        private static Bitmap SCORE = new Bitmap(Properties.Resources.digits);
        private static Bitmap TIME = ScaleImage(new Bitmap(Properties.Resources.time), 70, 25);
        //private static 

        public static Bitmap[] bmpSCORE_Digits = SubImageNumber(SCORE);
        public static Bitmap[] bmpTIME_Digits = SubImageNumber(TIME);

        public static int PosY_SCORE = 20;
        public static int PosX_currentSCORE = 350;
        public static int PosX_highestSCORE = 17;

        public static Size SCORE_SIZE = new Size(SCORE.Width / 10, SCORE.Height);
        public static Size TIME_SIZE = new Size(TIME.Width / 10, TIME.Height);

        /// <summary>
        /// Cắt ảnh từ dãy số 0 đến 9 thành từng số riêng biệt 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap[] SubImageNumber(Bitmap bmp) {
            if (bmp == null)
                throw new ArgumentNullException(nameof(bmp));

            int pieceWidth = bmp.Width / 10;  
            int height = bmp.Height;         

            Bitmap[] images = new Bitmap[10];
            for (int i = 0; i < 10; ++i) {
                Rectangle rect = new Rectangle(i * pieceWidth, 0, pieceWidth, height);
                images[i] = bmp.Clone(rect, System.Drawing.Imaging.PixelFormat.DontCare);
            }

            return images;
        }

        /// <summary>
        /// Cắt ảnh thành các số để in ra 
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int[] Split(int num = 0) {
            int[] result = new int[5];

            int i = 4;
            while(num != 0) {
                result[i] = num % 10;
                num /= 10;
                i--;
            }

            while(i >= 0) {
                result[i] = 0;
                i--;
            }

            return result;
        }

        /// <summary>
        /// Scale ảnh theo kích thước
        /// </summary>
        /// <param name="original"></param>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public static Bitmap ScaleImage(Bitmap original, int newWidth, int newHeight) {
            Bitmap newBitmap = new Bitmap(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(newBitmap)) {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                g.DrawImage(original, 0, 0, newWidth, newHeight);
            }
            return newBitmap;
        }
    }
}
