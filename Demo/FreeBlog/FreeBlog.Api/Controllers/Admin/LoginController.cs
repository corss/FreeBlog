using FreeBlog.Api.ApiGroup;
using FreeBlog.Api.Extensions;
using FreeBlog.Common;
using FreeBlog.Common.Security;
using FreeBlog.IService;
using FreeBlog.IService.Log;
using FreeBlog.Model;
using FreeBlog.Model.Models;
using FreeBlog.Model.ViewModels;
using FreeSql;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FreeBlog.Api.Controllers.Admin
{
    /// <summary>
    /// 登录
    /// </summary>
    [Produces("application/json")]
    [Route("api/Admin/[controller]")]
    [ApiGroup(ApiGroupNames.AdminAuth)]
    public class LoginController : CommonHelper
    {
        private readonly IUserService db;
        private readonly IExceptionLogService exceptionDB;
        private readonly IRoleService dbRole;
        private IFreeSql freeSql;
        private readonly ILoginLogService loginLogDB;
        private readonly IModuleService dbmo;
        public LoginController(IUserService db, IExceptionLogService exceptionDB, ILoginLogService loginLogDB, IFreeSql freeSql,IRoleService dbRole,
            IModuleService dbmo)
        {
            
            this.db = db;
            this.exceptionDB = exceptionDB;
            this.loginLogDB = loginLogDB;
            this.freeSql = freeSql;
            this.dbRole = dbRole;
            this.dbmo = dbmo;
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult<object>> Login(string userName, string password, string vcode)
        {
            var res = new ApiResult<object>();
            string value = GetSession("yzm");
            string value2 = GetCookieValue("vcode");
            // 如果非开发模式并且验证码不符
            //if (vcode != value && !env.IsDevelopment())
            //{
            //    res.code = (int)ApiEnum.ParameterError;
            //    res.msg = "验证码不正确！";
            //}
            //else
            //{
            if (string.IsNullOrWhiteSpace(value2))
            {
                // 登录
                var user = db.Select.Where(a => a.userName == userName && a.password == MD5Encode.GetEncrypt(password)).ToOne();
                if (user == null)
                {
                    res.code = (int)ApiEnum.ParameterError;
                    res.msg = "用户名或密码错误！";
                }
                else if (user.State == 2)
                {
                    res.success = true;
                    // 登录日志
                    loginLogDB.AddLoginLog(user.userName, user.id);
                    TokenInfoViewModel token = null;
                    try
                    {
                        token = JwtHelper.IssueJwt(new JwtHelper.TokenModelJwt { Uid = user.id, Role = "Admin" });
                    }
                    catch (Exception ex)
                    {
                        exceptionDB.AddBugLog(ex, "获取token失败", user.id);
                    }
                    string moduleId = null;
                    //获取对应接口url
                    var moduleUrl = new List<string>();
                    try
                    {
                        moduleId = dbRole.Select.Where(a => a.id == user.RoleId).ToOne().Modules;
                        foreach (var item in moduleId.Split(",").Where(x => !string.IsNullOrEmpty(x)).ToArray())
                        {
                            moduleUrl.Add(dbmo.Select.Where(a => a.Id == C.Int(item)).ToOne().LinkUrl);
                        }
                    }
                    catch (Exception ex)
                    {

                        exceptionDB.AddBugLog(ex, "获取菜单权限失败", user.id);
                    }

                    res.data = new
                    {
                        user.id,
                        user.userName,
                        user.nickName,
                        user.loginName,
                        user.RoleId,
                        Roles= moduleUrl,
                        token
                    };
                    res.msg = "获取成功";
                }
                else if (user.State == 1)
                {
                    res.code = (int)ApiEnum.ParameterError;
                    res.msg = "请等待审核！";
                }
                else if (user.State == 4)
                {
                    res.code = (int)ApiEnum.ParameterError;
                    res.msg = "账号不存在！";
                }
                else
                {
                    res.code = (int)ApiEnum.ParameterError;
                    res.msg = "账号异常！";
                }
            }
            else
            {
                if (vcode == value && vcode == value2)
                {
                    // 登录
                    var user = db.Select.Where(a => a.userName == userName && a.password == MD5Encode.GetEncrypt(password)).ToOne();
                    if (user == null)
                    {
                        res.code = (int)ApiEnum.ParameterError;
                        res.msg = "用户名或密码错误！";
                    }
                    else if (user.State == 2)
                    {
                        res.success = true;
                        // 登录日志
                        loginLogDB.AddLoginLog(user.userName, user.id);
                        TokenInfoViewModel token = null;
                        try
                        {
                            token = JwtHelper.IssueJwt(new JwtHelper.TokenModelJwt { Uid = user.id, Role = "Admin" });
                        }
                        catch (Exception ex)
                        {
                            exceptionDB.AddBugLog(ex, "获取token失败", user.id);
                        }

                        var moduleId = dbRole.Select.Where(a => a.id == user.RoleId).ToOne().Modules.Split(",");
                        var moduleUrl = new List<string>();
                        foreach (var item in moduleId.Where(x => !string.IsNullOrEmpty(x)).ToArray())
                        {
                            var rolesUrls = dbmo.Select.Where(a => a.Id == C.Int(item)).ToOne().LinkUrl.Split('/').Where(s => !string.IsNullOrEmpty(s)).Where(s=>s!="Admin").Where(s => s != "api").ToArray();
                            string rolesUrl = "sys";
                            foreach (var listurl in rolesUrls)
                            {
                                rolesUrl = rolesUrl + ":" + listurl;
                            }

                            moduleUrl.Add(rolesUrl);
                        }
                        res.data = new
                        {
                            user.id,
                            user.userName,
                            user.nickName,
                            user.loginName,
                            user.RoleId,
                            Roles = moduleUrl,
                            token
                        };
                        res.msg = "获取成功";
                    }
                    else if (user.State == 1)
                    {
                        res.code = (int)ApiEnum.ParameterError;
                        res.msg = "请等待审核！";
                    }
                    else if (user.State == 4)
                    {
                        res.code = (int)ApiEnum.ParameterError;
                        res.msg = "账号不存在！";
                    }
                    else
                    {
                        res.code = (int)ApiEnum.ParameterError;
                        res.msg = "账号异常！";
                    }
                }
                else
                {
                    res.msg = "验证码错误";
                }
            }
            //}
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns>文件流</returns>
        [HttpPost("VerificationCode")]
        public IActionResult VerificationCode()
        {
            VerificationCodeImage image = new VerificationCodeImage();
            string code;
            byte[] buf = image.CreateImage(out code);
            //HttpContext.Response.Cookies.Append("vcode", code); // 存入cookie
            AddCookie("vcode", code);// 存入cookie
                                     //HttpContext.Session.SetString("yzm", JsonConvert.SerializeObject(code));//存session
            SetSession("yzm", code);//存session;           
            return File(buf, "image/png");
        }


        /// <summary>
        /// 请求刷新Token（以旧换新）
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
         [HttpPost]
        [Route("RefreshToken")]
        public async Task<ApiResult<object>> RefreshToken(string token = "")
        {
            string jwtStr = string.Empty;
            var res = new ApiResult<object>();
            if (string.IsNullOrEmpty(token))
            {
                res.code = (int)ApiEnum.ParameterError;
                res.msg = "token不存在";
                return await Task.Run(() => res);
            }
            TokenInfoViewModel refreshToken = null;
            var tokenModel = JwtHelper.SerializeJwt(token);
            if (tokenModel != null && tokenModel.Uid > 0)
            {
                var user =  db.Select.Where(a=>a.id==tokenModel.Uid).ToOne();
                if (user != null)
                {
                    try
                    {
                     refreshToken = JwtHelper.IssueJwt(new JwtHelper.TokenModelJwt { Uid = tokenModel.Uid, Role = tokenModel.Role,Work=tokenModel.Work });
                    }
                    catch (Exception ex)
                    {
                        exceptionDB.AddBugLog(ex, "获取token失败", user.id);
                    }
                    res.success = true;
                    res.msg = "获取成功";
                    res.data = refreshToken;
                    return await Task.Run(() => res);
                }
            }
            res.success = false;
            res.msg = "认证失败！";
            return await Task.Run(() => res);

        }


        [HttpGet("getInfoByToken")]
        public  async Task<ApiResult<object>> getInfoByToken(string token = "")
        {
            string jwtStr = string.Empty;
            //标准接口返回格式
            var res = new ApiResult<object>();
            if (string.IsNullOrEmpty(token))
            {
                res.code = (int)ApiEnum.ParameterError;
                res.msg = "token不存在";
                return await Task.Run(() => res);
            }
            var refreshToken = "";
            var tokenModel = JwtHelper.SerializeJwt(token);
            if (tokenModel != null && tokenModel.Uid > 0)
            {
                var user = db.Select.Where(a => a.id == tokenModel.Uid).ToOne();
                if (user != null)
                {
                    res.data = user;
                    res.success = true;
                    res.msg = "获取成功";
                    return await Task.Run(() => res);
                }
            }
            res.success = false;
            res.msg = "认证失败！";
            res.code = (int)ApiEnum.TokenInvalid;
            return await Task.Run(() => res);
        }
    }
}
