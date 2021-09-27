using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FreeBlog.Common
{
    public class Utility
    {
        /// <summary>
        /// Session存用户的Key
        /// </summary>
        public static readonly string USER_SESSION_KEY = "User";
        /// <summary>
        /// Cookie存用户ID的Key
        /// </summary>
        public static readonly string USER_COOKIE_KEY = "UserID";
        /// <summary>
        /// 网站里图片的路径
        /// </summary>
        public static readonly string WEB_IMAGES_URL = AppSettings.Configuration["Site:WEB_IMAGES_URL"];
        /// <summary>
        /// 网站里上传图片的路径
        /// </summary>
        public static readonly string HostAddress = System.Web.HttpUtility.UrlDecode(AppSettings.Configuration["Site:HostAddress"]);

        /// <summary>
        /// 密钥值
        /// </summary>
        public static readonly string KEYVAL = "D6B530209F8BED78";
        /// <summary>
        /// 加密辅助向量
        /// </summary>
        public static readonly string IVVAL = "32C65FC9D1C9EB6D";

        /// <summary>
        /// 身份证加密
        /// </summary>
        /// <param name="IdCard"></param>
        /// <returns></returns>
        public static string IdCardEncrypt(string IdCard)
        {
            if (IdCard != null && (IdCard.Length == 18 || IdCard.Length == 15))
                IdCard = Security.AESEncode.CBCEncrypt(IdCard, KEYVAL, IVVAL);
            return IdCard;
        }
        /// <summary>
        /// 身份证解密
        /// </summary>
        /// <param name="IdCard"></param>
        /// <returns></returns>
        public static string IdCardUnEncrypt(string IdCard)
        {
            if (!string.IsNullOrWhiteSpace(IdCard) && IdCard.Length >= 44)
                IdCard = Security.AESEncode.CBCDecrypt(IdCard, KEYVAL, IVVAL);
            return IdCard;
        }

        /// <summary>
        /// 返回图片路径
        /// </summary>
        /// <param name="imgUrl">图片地址</param>
        /// <returns></returns>
        public static string GetImgUrl(string imgUrl)
        {
            // 没有图片就不返回，
            if (string.IsNullOrWhiteSpace(imgUrl))
            {
                // 返回本身
                return imgUrl;
            }
            // 网络图片，原样返回
            if (imgUrl.StartsWith("http"))
                return imgUrl;
            // 本地的图片则需要加头部
            return WEB_IMAGES_URL + imgUrl;
        }
        /// <summary>
        /// 返回图片路径
        /// </summary>
        /// <param name="imgUrl">图片地址</param>
        /// <param name="defaultImg">默认图片，非必传，如果要传本地的图片不用加前缀</param>
        /// <returns></returns>
        public static string GetImgUrl(string imgUrl, string defaultImg = null)
        {
            // 没有图片就不返回，
            if (string.IsNullOrWhiteSpace(imgUrl))
            {
                // 有默认图片的情况下，返回默认图片
                if (defaultImg != null)
                {
                    // 这里可以用递归
                    if (defaultImg.StartsWith("http"))
                        return defaultImg;
                    return WEB_IMAGES_URL + defaultImg;
                }
                // 返回本身
                return imgUrl;
            }
            // 网络图片，原样返回
            if (imgUrl.StartsWith("http"))
                return imgUrl;
            // 本地的图片则需要加头部
            return WEB_IMAGES_URL + imgUrl;
        }
        /// <summary>
        /// 格式化日期 yyyy-MM-dd HH:mm
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns></returns>
        public static string GetDateFormat(DateTime date)
        {
            return GetDateFormat(date, 2);
        }
        /// <summary>
        /// 格式化日期
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="Type">1.yyyy-MM-dd 2.yyyy-MM-dd HH:mm 3.yyyy-MM-dd HH:mm:ss</param>
        /// <returns></returns>
        public static string GetDateFormat(DateTime date, int Type = 2)
        {
            if (date.Year > 1900)
            {
                if (Type == 1)
                    return date.ToString("yyyy-MM-dd");
                if (Type == 2)
                    return date.ToString("yyyy-MM-dd HH:mm");
                return date.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return null;
        }
        /// <summary>
        /// 返回星期格式
        /// </summary>
        /// <returns></returns>
        public static string GetWeek()
        {
            string week = "";
            string dt = DateTime.Today.DayOfWeek.ToString(); //now.Today.DayOfWeek.ToString();

            switch (dt)
            {
                case "Monday":
                    week = "星期一";
                    break;
                case "Tuesday":
                    week = "星期二";
                    break;
                case "Wednesday":
                    week = "星期三";
                    break;
                case "Thursday":
                    week = "星期四";
                    break;
                case "Friday":
                    week = "星期五";
                    break;
                case "Saturday":
                    week = "星期六";
                    break;
                case "Sunday":
                    week = "星期日";
                    break;
            }
            return week;
        }

        /// <summary>
        /// 字符串数组转int数组
        /// </summary>
        /// <param name="strArray"></param>
        /// <param name="intArray"></param>
        /// <returns></returns>
        public static int[] ConvertArray(string[] strArray, int[] intArray)
        {
            return Array.ConvertAll(strArray, int.Parse);
        }

        /// <summary>
        /// 字符串拼接
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string StrJoin(string[] array)
        {
            return string.Join(",", array);
        }
        /// <summary>
        /// 将泛类型集合List类转换成DataTable
        /// </summary>
        /// <param name="list">泛类型集合</param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(List<T> entitys)
        {
            //检查实体集合不能为空
            if (entitys == null || entitys.Count < 1)
            {
                return null;
            }
            else
            {
                //取出第一个实体的所有Propertie
                Type entityType = entitys[0].GetType();
                PropertyInfo[] entityProperties = entityType.GetProperties();
                //生成DataTable的structure
                //生产代码中，应将生成的DataTable结构Cache起来，此处略
                DataTable dt = new DataTable();
                for (int i = 0; i < entityProperties.Length; i++)
                {
                    //dt.Columns.Add(entityProperties[i].Name, entityProperties[i].PropertyType);
                    dt.Columns.Add(entityProperties[i].Name);
                }
                //将所有entity添加到DataTable中
                foreach (object entity in entitys)
                {
                    //检查所有的的实体都为同一类型
                    if (entity.GetType() != entityType)
                    {
                        //throw new Exception("要转换的集合元素类型不一致");
                        return null;
                    }
                    object[] entityValues = new object[entityProperties.Length];
                    for (int i = 0; i < entityProperties.Length; i++)
                    {
                        entityValues[i] = entityProperties[i].GetValue(entity, null);
                    }
                    dt.Rows.Add(entityValues);
                }
                return dt;
            }
        }


        #region 身份证的处理方法
        /// <summary>
        /// 用于检查18位身份证号码的合法性
        /// </summary>
        /// <param name="CardId"></param>
        /// <returns></returns>
        private static bool CheckCard18(string CardId)//CheckCard18方法用于检查18位身份证号码的合法性
        {
            long n = 0;
            bool flag = false;
            if (long.TryParse(CardId.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(CardId.Replace('x', '0').Replace('X', '0'), out n) == false)
                return false;//数字验证
            string[] Myaddress = new string[]{ "11","22","35","44","53","12",
                "23","36","45","54","13","31","37","46","61","14","32","41",
                "50","62","15","33","42","51","63","21","34","43","52","64",
                "65","71","81","82","91"};
            for (int kk = 0; kk < Myaddress.Length; kk++)
            {
                if (Myaddress[kk].ToString() == CardId.Remove(2))
                {
                    flag = true;
                }
            }
            if (flag)
            {
                return flag;
            }
            string Mybirth = CardId.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime Mytime = new DateTime();
            if (DateTime.TryParse(Mybirth, out Mytime) == false)
                return false;//生日验证
            string[] MyVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] ai = CardId.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
                sum += int.Parse(wi[i]) * int.Parse(ai[i].ToString());
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (MyVarifyCode[y] != CardId.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }
        /// <summary>
        /// 用于检查15位身份证号码的合法性
        /// </summary>
        /// <param name="CardId"></param>
        /// <returns></returns>
        private static bool CheckCard15(string CardId)
        {
            long n = 0;
            bool flag = false;
            if (long.TryParse(CardId, out n) == false || n < Math.Pow(10, 14))
                return false;//数字验证
            string[] Myaddress = new string[]{ "11","22","35","44","53","12",
                "23","36","45","54","13","31","37","46","61","14","32","41",
                "50","62","15","33","42","51","63","21","34","43","52","64",
                "65","71","81","82","91"};
            for (int kk = 0; kk < Myaddress.Length; kk++)
            {
                if (Myaddress[kk].ToString() == CardId.Remove(2))
                {
                    flag = true;
                }
            }
            if (flag)
            {
                return flag;
            }
            string Mybirth = CardId.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime Mytime = new DateTime();
            if (DateTime.TryParse(Mybirth, out Mytime) == false)
            {
                return false;//生日验证
            }
            return true;//符合15位身份证标准
        }
        /// <summary>
        /// 是否合法身份证
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static bool IsIdCard(string card)
        {
            bool b = false;
            if (card.Length == 15)
            {
                b = CheckCard15(card);
            }
            else if (card.Length == 18)
            {
                b = CheckCard18(card);
            }
            return b;
        }
        /// <summary>
        /// 获取身份证中的生日
        /// </summary>
        /// <returns></returns>
        public static string GetIdCardBirthday(string card)
        {
            string s = "";
            if (IsIdCard(card))
            {
                card = Card15To18(card);
                string birthday = card.Substring(6, 8);//从身份证号码中截取出公民的生日
                string byear = birthday.Substring(0, 4);//获取出生年份
                string bmonth = birthday.Substring(4, 2);//获取出生月份
                string bday = birthday.Substring(6, 2);//获取出生“日”
                s = byear + "-" + bmonth + "-" + bday;
            }
            return s;
        }
        /// <summary>
        /// 获取身份证中的地址码
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static int GetIdCardAddNum(string card)
        {
            int addnum = 0;
            if (IsIdCard(card))
            {
                card = Card15To18(card);
                addnum = Convert.ToInt32(card.Remove(6));//获取身份证号码中的地址码
            }
            return addnum;
        }
        /// <summary>
        /// 获取身份证中的性别
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static bool GetIdCardGender(string card)
        {
            bool b = false;
            int PP = 0;
            if (IsIdCard(card))
            {
                if (card.Length == 15)//如果输入的身份证号码是15位
                {
                    PP = Convert.ToInt32(card.Substring(14, 1)) % 2;//判断最后一位是奇数还是偶数
                }
                else if (card.Length == 18)//如果输入的身份证号码是18位
                {
                    PP = Convert.ToInt32(card.Substring(16, 1)) % 2;//判断倒数第二位是奇数还是偶数
                }

                if (PP == 0)//如果是偶数
                {
                    b = false;//说明身份证号码的持有者是女性
                }
                else
                {
                    b = true;//如果是奇数则身份证号码的持有者是男性
                }
            }
            return b;
        }
        /// <summary>
        /// 15身份证转换18位
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        private static string Card15To18(string card)
        {
            string newID = card;
            if (card.Length == 15)          //如果输入的是15位的身份证号码，需要将其转换成18位
            {
                int[] w = new int[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2, 1 };
                char[] a = new char[] { '1', '0', 'x', '9', '8', '7', '6', '5', '4', '3', '2' };

                int s = 0;
                newID = card.Insert(6, "19");
                for (int i = 0; i < 17; i++)
                {
                    int k = Convert.ToInt32(newID[i]) * w[i];
                    s = s + k;
                }
                int h = 0;
                Math.DivRem(s, 11, out h);
                newID = newID + a[h];
            }
            return newID;
        }
        #endregion
        #region 手机号码
        /// <summary>
        /// 手机号替换中间四位数
        /// </summary>
        /// <returns></returns>
        public static string PhoneReplace(string value)
        {
            if (value.Length == 11)
            {
                value = value.Substring(0, 3) + "****" + value.Substring(7);
            }
            return value;
        }
        #endregion
        #region 文本、编码处理
        /// <summary>
        /// 设置富文本中图片宽
        /// </summary>
        /// <param name="Intro"></param>
        /// <param name="i">i:px,0:100% </param>
        /// <returns></returns>
        public static string SetImgWidth(string Intro, int i)
        {
            Intro = Intro.Replace("width:", " ").Replace("WIDTH:", " ").Replace("width=", " ").Replace("WIDTH=", " ");
            Intro = Intro.Replace("height:", " ").Replace("HEIGHT:", " ").Replace("height=", " ").Replace("HEIGHT=", " ");
            Intro = Intro.Replace("<img", "<img style='width:" + (i > 0 ? i + "px" : "100%") + ";'");
            return Intro;
        }
        /// <summary>
        /// 提取html文本中的文字
        /// </summary>
        /// <param name="zifu">源字符串</param>
        /// <returns>文字</returns>
        public string Tohtml(string zifu)
        {
            string noStyle = zifu.Replace("&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace("&nbsp;", "");
            noStyle = Regex.Replace(noStyle, @"<[\w\W]*?>", "", RegexOptions.IgnoreCase);
            noStyle = Regex.Replace(noStyle, @"\s", "", RegexOptions.IgnoreCase);
            return noStyle;
        }
        /// <summary>
        /// 获取字符长度(汉字占两个字符)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetLength(string str)
        {
            return Encoding.Default.GetBytes(str).Length;
        }

        /// <summary>
        /// 编码转换
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="soureStr"></param>
        /// <returns></returns>
        public static String GetString(Encoding source, Encoding dest, String soureStr)
        {
            return dest.GetString(Encoding.Convert(source, dest, source.GetBytes(soureStr)));
        }
        /// <summary>
        /// 编码转换
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="soureCharArr"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static String GetString(Encoding source, Encoding dest, Char[] soureCharArr, int offset, int len)
        {
            return dest.GetString(Encoding.Convert(source, dest, source.GetBytes(soureCharArr, offset, len)));
        }
        #endregion
        #region 时间戳
        private static DateTime _dtStart = new DateTime(1970, 1, 1, 8, 0, 0);
        /// <summary> 
        /// 获取时间戳（毫秒）
        /// </summary>  
        public static long GetTimeStamp(DateTime dateTime)
        {
            return Convert.ToInt64(dateTime.Subtract(_dtStart).TotalMilliseconds);
        }
        /// <summary> 
        /// 获取时间戳（秒）
        /// </summary>  
        public static long GetTimeStamp2(DateTime dateTime)
        {
            return Convert.ToInt64(dateTime.Subtract(_dtStart).TotalSeconds);
        }
        /// <summary> 
        /// 根据时间戳获取时间 
        /// </summary>  
        public static DateTime TimeStampToDateTime(string timeStamp)
        {
            return TimeStampToDateTime(Convert.ToInt64(timeStamp));
        }
        /// <summary> 
        /// 根据时间戳获取时间 
        /// </summary>  
        public static DateTime TimeStampToDateTime(long timeStamp)
        {
            if (timeStamp > 0)
                return _dtStart.AddMilliseconds(timeStamp);
            return DateTime.MinValue;
        }
        #endregion
        #region 数据验证
        #region 格式验证
        /// <summary>
        /// 手机号码格式验证
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMobilePhone(string mobile)
        {
            return Regex.IsMatch(mobile, @"^(((13[0-9]{1})|(14[0-9]{1})|(15[0-35-9]{1})|(16[0-9]{1})|(17[0-9]{1})|(18[0-9]{1})|(19[0-9]{1}))+\d{8})$");
        }
        /// <summary>
        /// 验证15或18位的身份证格式
        /// </summary>
        /// <param name="IdCard"></param>
        /// <returns></returns>
        public static bool IsIDcard(string IdCard)
        {
            return Regex.IsMatch(IdCard, @"/(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/");
        }
        /// <summary>
        /// 验证18位身份证，包含闰年验证
        /// </summary>
        /// <param name="IdCard"></param>
        /// <returns></returns>
        public static bool IsIDcard2(string IdCard)
        {
            return Regex.IsMatch(IdCard, @"^[1-9]\d{5}((((19|20)\d{2})(((0[13578]|1[02])([0-2][1-9]|[1-3]0|31))|((0[469]|11)([0-2][1-9]|[1-3]0))|(02(0[1-9]|1\d|2[0-8]))))|((((19|20)(0[48]|[2468][048]|[13579][26]))|(2000))02((0[1-9])|[12]\d)))\d{3}(\d|X|x)$");
        }
        #endregion

        #region 数据验证
        /// <summary>
        /// 判断DataTable是否有效，
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool IsValidTable(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
                return true;
            return false;
        }
        /// <summary>
        /// 判断List是否有效
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsValid<T>(IList<T> list)
        {
            if (list != null && list.Count > 0)
                return true;
            return false;
        }
        #endregion

        #region 其他验证
        /// <summary>
        /// 验证url是否正常
        /// </summary>
        /// <param name="url"></param>
        /// <returns>True通过 False不通过</returns>
        public static bool IsUrlVaild(string url)
        {
            if (url != null &&
                (!url.Contains("%27") && !url.Contains("'")) && !url.Contains(">") &&
                (!url.Contains("expression") && !url.Contains("onmouseover")))
                return true;
            return false;
        }
        #endregion
        #endregion
    }
}
