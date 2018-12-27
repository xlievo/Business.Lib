using System;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

public static class VerifyCode
{
    /// <summary>
    /// 获取一个验证码字串及PNG图片数据
    /// </summary>
    /// <param name="length">验证码长度(4比较合适)</param>
    /// <param name="checkcode">随机生成的验证码字符串</param>
    /// <returns></returns>
    public static byte[] GetImage(int length, out string checkcode)
    {
        checkcode = RandomText.String(length);
        var random = new Random();

        //注意，字体应该是所在操作系统中存在的字体 "Comic Sans MS"
        var font = new Font("Comic Sans MS", 20, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Pixel);
        var stringFormat = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        using (var bitmap = new Bitmap((int)Math.Ceiling((double)(length * 18)), 27, PixelFormat.Format24bppRgb))
        {
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                //画图片的背景噪音线
                for (int i = 0; i < 5; i++)
                {
                    int x1 = random.Next(bitmap.Width);
                    int x2 = random.Next(bitmap.Width);
                    int y1 = random.Next(bitmap.Height);
                    int y2 = random.Next(bitmap.Height);
                    graphics.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }

                graphics.DrawString(checkcode, font, Brushes.Black, new RectangleF(0f, 0f, (float)bitmap.Width, (float)bitmap.Height), stringFormat);

                //画图片的前景噪音点
                for (int i = 0; i < 20; i++)
                {
                    int x = random.Next(bitmap.Width);
                    int y = random.Next(bitmap.Height);

                    bitmap.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
            }

            using (var bitmap2 = WaveDistortion(bitmap))
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    bitmap2.Save(ms, ImageFormat.Png);
                    //bitmap2.Save("img.jpg");
                    return ms.ToArray();
                }
            }
        }
    }

    /// <summary>
    /// 波纹扭曲
    /// </summary>
    private static Bitmap WaveDistortion(Bitmap bitmap)
    {
        var rnd = new Random();

        var width = bitmap.Width;
        var height = bitmap.Height;

        var destBmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

        var foreColor = Color.FromArgb(rnd.Next(10, 100), rnd.Next(10, 100), rnd.Next(10, 100));
        var backColor = Color.FromArgb(rnd.Next(200, 250), rnd.Next(200, 250), rnd.Next(200, 250));


        using (var g = Graphics.FromImage(destBmp))
        {
            g.Clear(backColor);
        }

        // periods 时间
        var rand1 = rnd.Next(710000, 1200000) / 10000000.0;
        var rand2 = rnd.Next(710000, 1200000) / 10000000.0;
        var rand3 = rnd.Next(710000, 1200000) / 10000000.0;
        var rand4 = rnd.Next(710000, 1200000) / 10000000.0;

        // phases  相位
        var rand5 = rnd.Next(0, 31415926) / 10000000.0;
        var rand6 = rnd.Next(0, 31415926) / 10000000.0;
        var rand7 = rnd.Next(0, 31415926) / 10000000.0;
        var rand8 = rnd.Next(0, 31415926) / 10000000.0;

        // amplitudes 振幅
        var rand9 = rnd.Next(330, 420) / 110.0;
        var rand10 = rnd.Next(330, 450) / 110.0;
        var amplitudesFactor = rnd.Next(5, 6) / 12.0;//振幅小点防止出界
        var center = width / 2.0;

        //波纹扭曲，重绘新图片
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var sx = x + (Math.Sin(x * rand1 + rand5) + Math.Sin(y * rand2 + rand6)) * rand9 - width / 2 + center + 1;
                var sy = y + (Math.Sin(x * rand3 + rand7) + Math.Sin(y * rand4 + rand8)) * rand10 * amplitudesFactor;

                int color, color_x, color_y, color_xy;
                var overColor = Color.Empty;

                if (sx < 0 || sy < 0 || sx >= width - 1 || sy >= height - 1)
                {
                    continue;
                }
                else
                {
                    color = bitmap.GetPixel((int)sx, (int)sy).B;
                    color_x = bitmap.GetPixel((int)(sx + 1), (int)sy).B;
                    color_y = bitmap.GetPixel((int)sx, (int)(sy + 1)).B;
                    color_xy = bitmap.GetPixel((int)sx + 1, (int)sy + 1).B;
                }

                if (color == 255 && color_x == 255 && color_y == 255 && color_xy == 255)
                {
                    continue;
                }
                else if (color == 0 && color_x == 0 && color_y == 0 && color_xy == 0)
                {
                    overColor = Color.FromArgb(foreColor.R, foreColor.G, foreColor.B);
                }
                else
                {
                    var frsx = sx - Math.Floor(sx);
                    var frsy = sy - Math.Floor(sy);
                    var frsx1 = 1 - frsx;
                    var frsy1 = 1 - frsy;

                    var newColor = color * frsx1 * frsy1 + color_x * frsx * frsy1 + color_y * frsx1 * frsy + color_xy * frsx * frsy;

                    if (newColor > 255)
                    {
                        newColor = 255;
                    }

                    newColor = newColor / 255;

                    var newcolor0 = 1 - newColor;

                    var newred = Math.Min((int)(newcolor0 * foreColor.R + newColor * backColor.R), 255);
                    var newgreen = Math.Min((int)(newcolor0 * foreColor.G + newColor * backColor.G), 255);
                    var newblue = Math.Min((int)(newcolor0 * foreColor.B + newColor * backColor.B), 255);
                    overColor = Color.FromArgb(newred, newgreen, newblue);
                }

                destBmp.SetPixel(x, y, overColor);
            }
        }

        return destBmp;
    }

    static class RandomText
    {
        static readonly string[] source = { "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f", "g", "h", "j", "k", "m", "n", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        private static Random _random = new Random();


        public static string String(int length)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                int index = _random.Next(0, source.Length - 1);
                sb.Append(source[index]);
            }
            return sb.ToString();
        }

    }
}