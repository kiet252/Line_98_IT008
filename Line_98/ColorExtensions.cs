using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Line_98
{
    /// <summary>
    /// Các phương thức làm sáng và làm tối, bổ trợ tạo banh 3D
    /// </summary>
    public static class ColorExtensions
    {
        //Phương thức tạo vùng sáng
        public static Color Lighten(this Color color, float factor)
        {
            return Color.FromArgb(
                color.A,
                Math.Min(255, (int)(color.R + (255 - color.R) * factor)),
                Math.Min(255, (int)(color.G + (255 - color.G) * factor)),
                Math.Min(255, (int)(color.B + (255 - color.B) * factor))
            );
        }
        //Phương thức tạo vùng tối
        public static Color Darken(this Color color, float factor)
        {
            return Color.FromArgb(
                color.A,
                Math.Max(0, (int)(color.R * (1 - factor))),
                Math.Max(0, (int)(color.G * (1 - factor))),
                Math.Max(0, (int)(color.B * (1 - factor)))
            );
        }
    }
}
