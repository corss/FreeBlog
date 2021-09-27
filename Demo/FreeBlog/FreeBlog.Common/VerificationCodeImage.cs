using System;
using System.Drawing;
using System.IO;

namespace FreeBlog.Common
{
    /// <summary>
    /// 生成验证码
    /// </summary>
    public class VerificationCodeImage
    {

        //生成随机验证码数字字符串——4位数
        public string RandomNum()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            string intString = random.Next(1000, 9999).ToString();
            return intString;
        }

        //生成随机点
        public int[] RandomPoint()
        {
            int[] intArray = new int[6];
            for (int i = 0; i < 6; i += 2)
            {
                Random random = new Random(Guid.NewGuid().GetHashCode());
                switch (i)
                {
                    case 0:
                        intArray[i] = random.Next(0, 10);
                        break;
                    case 2:
                        intArray[i] = random.Next(45, 55);
                        break;
                    case 4:
                        intArray[i] = random.Next(90, 100);
                        break;
                }
            }
            for (int i = 1; i < 6; i += 2)
            {
                Random random = new Random(Guid.NewGuid().GetHashCode());
                intArray[i] = random.Next(0, 42);
            }
            return intArray;
        }

        /// <summary>
        /// 生成图片
        /// </summary>
        /// <returns></returns>
        public byte[] CreateImage(out string code)
        {
            //设置图片大小
            Image image = new Bitmap(100, 42);
            //设置画笔在哪一张图片上画图
            Graphics graph = Graphics.FromImage(image);
            //背景色
            graph.Clear(Color.White);
            //笔刷
            Pen pen = new Pen(Brushes.Black, 2);
            for (int i = 0; i < 4; i++)
            {
                int[] points = RandomPoint();
                //画一条曲线
                graph.DrawCurve(pen, new Point[] {
                    new Point(points[0], points[1]),
                    new Point(points[2], points[3]),
                    new Point(points[4], points[5])
                });
            }
            //画一条直线
            //graph.DrawLines(pen, new Point[] { new Point(10, 10), new Point(90, 40) });
            //画数字
            code = RandomNum();
            graph.DrawString(code, new Font(new FontFamily("Microsoft YaHei"), 20, FontStyle.Bold),
                Brushes.Black, new PointF(10, 0));
            //内存流
            MemoryStream ms = new MemoryStream();
            //把图片存进内存流
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            //获取内存流的byte数组
            byte[] buf = ms.GetBuffer();
            return buf;
        }
    }
}
