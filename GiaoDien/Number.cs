using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Utilities {
    public class Number {
        private static Bitmap SCORE = new Bitmap(Properties.Resources.digits);
        private static Bitmap TIME = new Bitmap(Properties.Resources.time);
        //private static 

        public static Bitmap[] bmpCurrentScoreDigits = SubImageNumber(SCORE);
        public static Bitmap[] bmpHighestScoreDigits = SubImageNumber(TIME);

        public static int PosY_SCORE = 20;
        public static int PosX_currentSCORE = 350;
        public static int PosX_highestSCORE = 17;

        public static Size SCORE_SIZE = new Size(18, 35);

        /// <summary>
        /// Cắt ảnh từ dãy số 0 đến 9 thành từng số riêng biệt 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap[] SubImageNumber(Bitmap bmp) {
            if (bmp == null)
                throw new ArgumentNullException(nameof(bmp));

            int pieceWidth = bmp.Width / 10;  // = 18
            int height = bmp.Height;          // = 35

            Bitmap[] images = new Bitmap[10];
            for (int i = 0; i < 10; ++i) {
                Rectangle rect = new Rectangle(i * pieceWidth, 0, pieceWidth, height);
                images[i] = bmp.Clone(rect, System.Drawing.Imaging.PixelFormat.DontCare);
            }

            return images;
        }



        /// <summary>
        /// Cắt điểm thành các số để in ra 
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int[] Slipt(int num) {
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
    }
}
