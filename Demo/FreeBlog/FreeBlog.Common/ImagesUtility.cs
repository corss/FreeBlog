using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace FreeBlog.Common
{
    public class ImagesUtility
    {

        #region Base64
        /// <summary>
        /// 把Base64图片保存到本地
        /// </summary>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="type">图片类型jpeg png</param>
        /// <returns></returns>
        public static string SaveBase64(string value, string path, string name, string type)
        {
            if (value.Contains("base64"))
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                value = value.Substring(value.IndexOf(',') + 1);
                value = value.Replace(' ', '+');
                byte[] bytes = Convert.FromBase64String(value);
                MemoryStream ms = new MemoryStream(bytes);
                Bitmap bmp = new Bitmap(ms);
                //保存图片
                if (type == "image/png")
                {
                    bmp.Save(path + name + ".png", ImageFormat.Png);
                    return name + ".png";
                }
                else if (type == "image/gif")
                {
                    bmp.Save(path + name + ".gif", ImageFormat.Gif);
                    return name + ".gif";
                }
                else
                {
                    bmp.Save(path + name + ".jpg", ImageFormat.Jpeg);
                    return name + ".jpg";
                }
            }
            return "";
        }

        /// <summary>
        /// Image 转成 base64
        /// </summary>
        /// <param name="fileFullName"></param>
        public static string ImageToBase64(string fileFullName)
        {
            try
            {
                Bitmap bmp = new Bitmap(fileFullName);
                MemoryStream ms = new MemoryStream();
                var suffix = fileFullName.Substring(fileFullName.LastIndexOf('.') + 1,
                    fileFullName.Length - fileFullName.LastIndexOf('.') - 1).ToLower();
                var suffixName = suffix == "png"
                    ? ImageFormat.Png
                    : suffix == "jpg" || suffix == "jpeg"
                        ? ImageFormat.Jpeg
                        : suffix == "bmp"
                            ? ImageFormat.Bmp
                            : suffix == "gif"
                                ? ImageFormat.Gif
                                : ImageFormat.Jpeg;

                bmp.Save(ms, suffixName);
                byte[] arr = new byte[ms.Length]; ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length); ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 把Base64转文件
        /// </summary>
        /// <param name="base64String">base64</param>
        /// <param name="path">路径</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="TypeID"></param>
        /// <returns></returns>
        public static string Base64StringToFile(string base64String, string path, string fileName)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            List<string> Types = base64String.Split(';').ToList();
            if (Types.Count > 0)
            {
                List<string> TypeID = Types[0].Split('/').ToList();
                if (TypeID.Count > 0)
                {
                    string t = "";
                    if (TypeID[1] == "jpeg")
                        t = "jpg";
                    else
                        t = TypeID[1];

                    string strbase64 = base64String.Trim().Substring(base64String.IndexOf(",") + 1);   //将‘，’以前的多余字符串删除
                    MemoryStream stream = new MemoryStream(Convert.FromBase64String(strbase64));
                    FileStream fs = new FileStream(path + fileName + "." + t, FileMode.OpenOrCreate, FileAccess.Write);
                    byte[] b = stream.ToArray();
                    fs.Write(b, 0, b.Length);
                    fs.Close();

                    return fileName + "." + t;
                }
            }
            return "";
        }
        #endregion
    }
}
