using System;

namespace FreeBlog.Common
{
    /// <summary>
    /// 数据类型转换类
    /// </summary>
    public class C
    {
        /// <summary>
        /// 强转为string
        /// </summary>
        /// <param name="o">值</param>
        /// <returns></returns>
        public static String String(object o) { return Convert.ToString(o); }
        /// <summary>
        /// 强转为string
        /// </summary>
        /// <param name="o">值</param>
        /// <returns></returns>
        public static String String(string o)
        {
            if (o == null)
                return "";
            return o;
        }
        /// <summary>
        /// 强转为int
        /// </summary>
        /// <param name="o">值</param>
        /// <returns></returns>
        public static Int32 Int(object o)
        {
            return Int(o + "");
        }
        /// <summary>
        ///  强转为int
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static Int32 Int(string o)
        {
            int i = 0;
            if (int.TryParse(o, out i))
                return i;
            if (IsDecimal(o))
                return Convert.ToInt32(Convert.ToDecimal(o));
            if (IsDouble(o))
                return Convert.ToInt32(Convert.ToDouble(o));
            return 0;
        }
        /// <summary>
        /// 转为长整形
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static Int64 Long(object o) { return IsLong(o + "") ? Convert.ToInt64(o) : 0; }
        /// <summary>
        /// 强转为byte
        /// </summary>
        /// <param name="o">值</param>
        /// <returns></returns>
        public static Byte Byte(object o) { return IsByte(o + "") ? Convert.ToByte(o) : Convert.ToByte(0); }

        /// <summary>
        /// 最小默认时间
        /// </summary>
        public static DateTime min = new DateTime(1900, 1, 1);
        /// <summary>
        /// 强转为DateTime
        /// </summary>
        /// <param name="o">值</param>
        /// <returns></returns>
        public static DateTime DateTimes(object o)
        {
            return DateTimes(o + "");
        }
        /// <summary>
        /// 强转为DateTime
        /// </summary>
        /// <param name="o">值</param>
        /// <returns></returns>
        public static DateTime DateTimes(string o)
        {
            if (o == "")
                return min;
            DateTime d = min;
            if (DateTime.TryParse(o, out d))
            {
                if (d < min)
                    return min;
            }
            if (d < min)
                return min;
            return d;
        }
        /// <summary>
        /// 强转为Double
        /// </summary>
        /// <param name="o">值</param>
        /// <returns></returns>
        public static Double Doubles(object o) { return IsDouble(o + "") ? Convert.ToDouble(o) : 0; }
        /// <summary>
        /// 强转为Decimal
        /// </summary>
        /// <param name="o">值</param>
        /// <returns></returns>
        public static Decimal Decimals(object o)
        {
            return Decimals(o + "");
        }
        /// <summary>
        /// 强转为Decimal
        /// </summary>
        /// <param name="o">值</param>
        /// <returns></returns>
        public static Decimal Decimals(string o)
        {
            Decimal d = 0;
            if (Decimal.TryParse(o, out d))
                return d;
            return 0;
        }
        /// <summary>
        /// 强转为bool
        /// </summary>
        /// <param name="o">值</param>
        /// <returns></returns>
        public static Boolean Bool(object o) { return Bool(o + ""); }
        /// <summary>
        /// 强转为bool
        /// </summary>
        /// <param name="o">值</param>
        /// <returns></returns>
        public static Boolean Bool(string o)
        {
            if (o == "1")
                return true;
            if (o == "0")
                return false;
            bool b = false;
            if (Boolean.TryParse(o, out b))
                return b;
            return false;
        }

        /// <summary>
        /// 返回是否int
        /// </summary>
        /// <param name="s">值</param>
        /// <returns></returns>
        public static Boolean IsInt(string s) { int i = 0; return int.TryParse(s, out i); }
        /// <summary>
        /// 返回是否byte
        /// </summary>
        /// <param name="s">值</param>
        /// <returns></returns>
        public static Boolean IsByte(string s) { byte i = 0; return byte.TryParse(s, out i); }
        /// <summary>
        /// 返回是否long
        /// </summary>
        /// <param name="s">值</param>
        /// <returns></returns>
        public static Boolean IsLong(string s) { long i = 0; return long.TryParse(s, out i); }
        /// <summary>
        /// 返回是否double
        /// </summary>
        /// <param name="s">值</param>
        /// <returns></returns>
        public static Boolean IsDouble(string s) { double i = 0; return double.TryParse(s, out i); }
        /// <summary>
        /// 返回是否DateTime
        /// </summary>
        /// <param name="s">值</param>
        /// <returns></returns>
        public static Boolean IsDateTime(string s) { DateTime d = min; return DateTime.TryParse(s, out d); }
        /// <summary>
        /// 返回是否Decimal
        /// </summary>
        /// <param name="s">值</param>
        /// <returns></returns>
        public static Boolean IsDecimal(string s) { Decimal d = 0; return Decimal.TryParse(s, out d); }
        /// <summary>
        /// 返回是否Boolean
        /// </summary>
        /// <param name="s">值</param>
        /// <returns></returns>
        public static Boolean IsBool(string s) { bool b = true; return Boolean.TryParse(s, out b); }
        /// <summary>
        /// 返回是否NULL
        /// </summary>
        /// <param name="s">值</param>
        /// <returns></returns>
        public static Boolean IsNull(string s) { return Convert.IsDBNull(s); }
    }
}
