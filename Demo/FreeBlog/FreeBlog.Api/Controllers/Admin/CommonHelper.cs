using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace FreeBlog.Api.Controllers.Admin
{
    //[ApiExplorerSettings(IgnoreApi = true)] //隐藏接口
    [Route("api/[controller]/[action]")]
    public class CommonHelper: ControllerBase
    {
        #region Session
        /// <summary>
        /// 存储对象
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        [HttpGet(template: "SetSession")]
        protected void SetSession<T>(string key, T value)
        {
            HttpContext.Session.SetString(key, JsonConvert.SerializeObject(value));
        }
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        [HttpGet(template: "GetSession")]
        protected T GetSession<T>(string key)
        {
            string value = HttpContext.Session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// 设置Session
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        [HttpGet(template: "SetSession2")]
        protected void SetSession(string key, string value)
        {
            HttpContext.Session.SetString(key, value);
        }

        /// <summary>
        /// 获取Session
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>返回对应的值</returns>
        [HttpGet(template: "GetSession2")]
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
        [HttpGet(template: "AddCookie")]
        protected void AddCookie(string key, string value)
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
        [HttpGet(template: "AddCookie2")]
        protected void AddCookie(string key, string value, int time)
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
        protected void DeleteCookie(string key)
        {
            HttpContext.Response.Cookies.Delete(key);
        }
        /// <summary>
        /// 根据键获取对应的cookie
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet(template: "GetCookieValue")]
        protected string GetCookieValue(string key)
        {
            string value;
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
