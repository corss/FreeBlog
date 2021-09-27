using FreeBlog.IService;
using FreeBlog.IService.Base;
using FreeBlog.Model.Models.Log;
using FreeBlog.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Service
{
    public class ExceptionLogService : BaseRepository<ExceptionLog>, IExceptionLogService
    {
        public ExceptionLogService(IFreeSql fsql) : base(fsql)
        {
        }
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="userID">用户ID</param>
        /// <param name="Type">类型 1.异常 2.文件不存在 3.登录失败 4.记录</param>
        /// <returns></returns>
        public bool AddBugLog(string title, string content = "", int userID = 0, int? Type = null)
        {
            ExceptionLog e = new ExceptionLog();
            e.Title = title;
            e.ProjectID = "";
            e.Content = content;
            if (Type == null)
            {
                if (e.Content.Contains("不存在") || e.Content.Contains("not find"))
                    e.Type = 2;
                else
                    e.Type = 1;
            }
            else
            {
                e.Type = Type.Value;
            }
            e.UserID = userID;
            e.AddDate = DateTime.Now;
            return Insert(e).ID > 0;
        }
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="e">系统异常类</param>
        /// <param name="title">标题</param>
        /// <param name="userID">用户ID</param>
        /// <param name="Type">类型 1.异常 2.文件不存在 3.登录失败 4.记录</param>
        /// <returns></returns>
        public bool AddBugLog(Exception e, string title = null, int userID = 0, int? Type = null)
        {
            ExceptionLog m = new ExceptionLog();
            if (e != null)
            {
                StringBuilder message = new StringBuilder("内部错误:");
                message.Append(e.InnerException)
                  .Append("\r\n堆栈：")
                  .Append(e.StackTrace)
                  .Append("\r 信息:")
                  .Append(e.Message)
                  .Append("\r 来源:")
                  .Append(e.Source);
                //.Append("地址:" + HttpContext.Current.Request.Url);

                if (title == null)
                    m.Title = e.Message;
                else
                    m.Title = title;
                m.Content = message.ToString() + title;
            }
            else
            {
                m.Title = title;
                m.Content = title;
            }
            if (Type == null)
            {
                if (m.Content.Contains("不存在") || m.Content.Contains("not find"))
                    m.Type = 2;
                else
                    m.Type = 1;
            }
            else
            {
                m.Type = Type.Value;
            }
            m.AddDate = DateTime.Now;
            m.UserID = userID;
            return Insert(m).ID > 0;
        }
    }
}
