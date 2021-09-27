using FreeBlog.Api.ApiGroup;
using FreeBlog.Api.Extensions;
using FreeBlog.Common;
using FreeBlog.IService;
using FreeBlog.Model.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static FreeBlog.Api.Extensions.JwtHelper;

namespace FreeBlog.Api.Controllers
{
    /// <summary>
    /// 用户接口父类
    /// </summary>
    /// 
    [ApiGroup(ApiGroupNames.AdminAuth)]
    public class AdminBaseController : Controller
    {
        private  IUserService dbuser;
        private  IModuleService dbmo;
        private IRoleService dbRole;
        
        public AdminBaseController(IRoleService dbRole, IUserService dbuser,IModuleService dbmo)
        {
            this.dbRole = dbRole;
            this.dbuser = dbuser;
            this.dbmo = dbmo;

        }


        /// <summary>
        /// 获取UserID
        /// </summary>
        /// <returns></returns>
        [HttpGet(template: "GetUserID")]
        [ApiExplorerSettings(IgnoreApi = true)] //隐藏接口
        public int GetUserID()
        {
            string authHeader = this.Request.Headers["Authorization"];//Header中的token
            if (!string.IsNullOrWhiteSpace(authHeader))
            {
                authHeader = authHeader.Replace("Bearer ", "");
                return JwtHelper.SerializeJwt2(authHeader);
            }
            return 0;
        }
        /// <summary>
        /// 解码Token
        /// </summary>
        /// <returns></returns>
        [HttpGet(template: "TokenDecode")]
        [ApiExplorerSettings(IgnoreApi = true)] //隐藏接口
        public TokenModelJwt TokenDecode()
        {
            string authHeader = this.Request.Headers["Authorization"];//Header中的token
            if (!string.IsNullOrWhiteSpace(authHeader))
                authHeader = authHeader.Replace("Bearer ", "");
            return JwtHelper.SerializeJwt(authHeader);
        }
        /// <summary>
        /// 获取菜单权限
        /// </summary>
        /// <returns></returns>
        [HttpGet(template: "GetAccess")]
        [ApiExplorerSettings(IgnoreApi = true)] //隐藏接口

        public bool GetAccess( int roleID,string apiurl)
        {
            if (GetUserID() > 0)
            {
                int RoleID = dbuser.Select.Where(a => a.id == GetUserID()).ToOne().RoleId;
                string access= dbRole.Select.Where(a => a.id == RoleID).ToOne().Modules;
                if (dbmo.Select.Where(a => a.LinkUrl == apiurl).ToOne() == null)
                {
                    Module m = new Module();
                    m.LinkUrl = apiurl;
                    dbmo.Insert(m);
                }
                if (RoleID== roleID)
                {
                    return true;
                }

                if (!string.IsNullOrEmpty(access))
                {
                    List<string> linkurl = new List<string>();
                    foreach (var item in access.Split(","))
                    {
                        var url = dbmo.Select.Where(a => a.Id == C.Int(item)).ToOne();
                        if (url != null)
                             linkurl.Add(url.LinkUrl);
                    }
                  return  linkurl.Contains(apiurl);


                }
            }
            return false;
        }
    }
}
