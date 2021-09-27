using FreeBlog.Common;
using FreeBlog.Common.Security;
using FreeBlog.IService;
using FreeBlog.IService.Log;
using FreeBlog.Model.Models;
using FreeBlog.Web.Controllers.Common;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FreeBlog.Web.Controllers
{
    public class IndexController : CommonController
    {
        private readonly IUserService db;
        private readonly ILoginLogService loginLogDB;
        public IndexController(IUserService db, ILoginLogService loginLogDB)
        {
            this.db = db;
            this.loginLogDB = loginLogDB;
        }
        public IActionResult Login(string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                string userID = GetCookieValue(Utility.USER_COOKIE_KEY);
                if (!string.IsNullOrWhiteSpace(userID) && userID.Length > 10)
                {
                    int uID = C.Int(AESEncode.CBCDecrypt(userID, Utility.KEYVAL, Utility.IVVAL));
                    if (uID > 0)
                    {
                        UserInfo user = db.Select.Where(s=>s.ID== uID).First();
                        if (user != null)
                        {
                            UserSave(user);
                            HttpContext.Response.Redirect(url);
                        }
                        else
                        {
                            // 登录失败、删除cookie的值，防止因数据删除导致死循环
                            DeleteCookie(Utility.USER_COOKIE_KEY);
                        }
                    }
                }
            }
            ViewBag.Title = "登录";
            return View();
        }
        /// <summary>
        /// 退出登录
        /// </summary>
        public void SignOut()
        {
            UserClear();    // 默认清除登录状态
            HttpContext.Response.Redirect("/Index/Login");
        }
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns>文件流</returns>
        public IActionResult VerificationCode()
        {
            VerificationCodeImage image = new VerificationCodeImage();
            string code;
            byte[] buf = image.CreateImage(out code);
            AddCookie("vcode", code);   // 存入cookie
            //SetSession("vcode1", code);   // 存入session
            return File(buf, "image/png");
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<ApiResult<string>> LoginVerification(string username, string password, string platform, string browser, int width, int height)
        {
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Status };
            var user = db.Select.Where(a => a.UserName == username).First();
            switch (user.State)
            {
                case 2:
                    if (user.Password == MD5Encode.GetEncrypt(password))
                    {
                        res.success = true;
                        // 保存用户登录状态
                        UserSave(user);
                        // 登录日志
                        loginLogDB.AddLoginLog(user.UserName, user.ID, browser, platform, width, height);
                        // 删除验证码
                        DeleteCookie("vcode");
                    }
                    else
                    {
                        res.success = false;
                        res.msg = "账号密码不匹配！";
                    }
                    break;
                default:
                    res.success = false;
                    res.msg = "账号状态异常！";
                    break;
            }
            return await Task.Run(() => res);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public object AddUser(string username, string password, string platform, string browser, int width, int height)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            return LoginVerification(username, password, platform, browser, width, height);
        }
    }
}
