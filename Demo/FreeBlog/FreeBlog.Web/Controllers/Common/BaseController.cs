using FreeBlog.Common;
using Microsoft.AspNetCore.Mvc.Filters;
namespace FreeBlog.Web.Controllers.Common
{
    /// <summary>
    /// 需要登录控制器的父类
    /// </summary>
    public class BaseController : CommonController
    {
        public bool IsCheckedUserLogin = true;
        /// <summary>
        /// 校验用户是否登陆
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //校验用户是否已登录
            if (IsCheckedUserLogin)
            {
                if (GetSession(Utility.USER_SESSION_KEY) == null)
                {
                    if(!string.IsNullOrWhiteSpace(GetCookieValue(Utility.USER_COOKIE_KEY)))
                    {                        
                        filterContext.HttpContext.Response.Redirect("/Index/Login?url="+ HttpContext.Request.Path.Value);
                    }
                    else
                    {
                        filterContext.HttpContext.Response.Redirect("/Index/Login");
                    }
                }
            }
        }
    }
}

