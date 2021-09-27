using FreeBlog.Common;
using FreeBlog.Common.Security;
using FreeBlog.Model.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeBlog.Web.Controllers.Common
{
    /// <summary>
    /// 所有控制器的父类
    /// </summary>
    public class CommonController : Controller
    {
        #region User处理
        /// <summary>
        /// 获取用户，可能为null
        /// </summary>
        /// <returns></returns>
        public UserInfo GetUserInfo()
        {
            return GetSession<UserInfo>(Utility.USER_SESSION_KEY);
        }

        /// <summary>
        /// 保存用户状态，用于登录
        /// </summary>
        /// <param name="user"></param>
        public void UserSave(UserInfo user)
        {
            SetSession(Utility.USER_SESSION_KEY, user);
            AddCookie(Utility.USER_COOKIE_KEY, AESEncode.CBCEncrypt(user.ID.ToString(), Utility.KEYVAL, Utility.IVVAL));
        }
        /// <summary>
        /// 清除用户状态，用于退出
        /// </summary>
        public void UserClear()
        {
            ClearSession();
            DeleteCookie(Utility.USER_COOKIE_KEY);
        }
        #endregion

        #region Session
        /// <summary>
        /// 存储对象
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void SetSession<T>(string key, T value)
        {
            HttpContext.Session.SetString(key, JsonConvert.SerializeObject(value));
        }
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public T GetSession<T>(string key)
        {
            string value = HttpContext.Session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// 设置Session
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        protected void SetSession(string key, string value)
        {
            HttpContext.Session.SetString(key, value);
        }

        /// <summary>
        /// 获取Session
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>返回对应的值</returns>
        protected string GetSession(string key)
        {
            var value = HttpContext.Session.GetString(key);
            if (string.IsNullOrEmpty(value))
                value = null;
            return value;
        }

        /// <summary>
        /// 清除Session
        /// </summary>
        /// <param name="key">键</param>
        protected void RemoveSession(string key)
        {
            HttpContext.Session.Remove(key);
        }
        /// <summary>
        /// 清除所有Session
        /// </summary>
        protected void ClearSession()
        {
            HttpContext.Session.Clear();
        }

        #endregion

        #region Cookie
        /// <summary>
        /// 添加cookie缓存不设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddCookie(string key, string value)
        {
            //try
            //{
            HttpContext.Response.Cookies.Append(key, value);
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}
        }
        /// <summary>
        /// 添加cookie缓存设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="time"></param>
        public void AddCookie(string key, string value, int time)
        {
            HttpContext.Response.Cookies.Append(key, value, new CookieOptions
            {
                Expires = DateTime.Now.AddMilliseconds(time)
            });
        }
        /// <summary>
        /// 删除cookie缓存
        /// </summary>
        /// <param name="key"></param>
        public void DeleteCookie(string key)
        {
            HttpContext.Response.Cookies.Delete(key);
        }
        /// <summary>
        /// 根据键获取对应的cookie
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetCookieValue(string key)
        {
            var value = "";
            HttpContext.Request.Cookies.TryGetValue(key, out value);
            if (string.IsNullOrWhiteSpace(value))
            {
                value = string.Empty;
            }
            return value;
        }
        #endregion
    }
}
