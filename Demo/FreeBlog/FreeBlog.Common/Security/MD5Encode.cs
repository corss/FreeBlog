using System;
using System.Security.Cryptography;
using System.Text;
namespace FreeBlog.Common.Security
{
    public class MD5Encode
    {
        public static string MD5Encrypt(string pwd)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            return BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(pwd))).Replace("-", "").ToUpper();
        }

        /// <summary>
        /// MD5加密，并进行我们的二次加密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEncrypt(string value)
        {
            return MD5Encrypt(value)
                .Replace("A", "9")
                .Replace("B", "7")
                .Replace("C", "2")
                .Replace("D", "3")
                .Replace("E", "1")
                .Replace("F", "5");
        }
    }
}
