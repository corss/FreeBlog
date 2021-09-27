using AutoMapper;
using FreeBlog.Api.ApiGroup;
using FreeBlog.Api.Helper;
using FreeBlog.Common;
using FreeBlog.Common.Security;
using FreeBlog.IService;
using FreeBlog.Model.Models;
using FreeBlog.Model.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static FreeBlog.Api.Extensions.JwtHelper;

namespace FreeBlog.Api.Controllers.Admin
{
    /// <summary>
    /// 首页
    /// </summary>

    [Produces("application/json")]
    [Route("api/Admin/[controller]")]
    [ApiGroup(ApiGroupNames.AdminAuth)]
    [Authorize(Policy = "Admin")]
    public class IndexController : AdminBaseController
    {
        private readonly IUserService db;
        private readonly IRoleService dbRole;
        private readonly IMenuService dbMenu;
        private readonly IMapper _IMapper;
        private readonly IModuleService dbmo;
        public IndexController(IUserService db, IMenuService dbMenu, IRoleService dbRole, IMapper IMapper,IModuleService dbmo):base(dbRole,db,dbmo)
        {
            this.db = db;
            this.dbMenu = dbMenu;
            this.dbRole = dbRole;
            this._IMapper = IMapper;
            this.dbmo = dbmo;
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [HttpPut("UpdatePwd")]
        [Authorize(Policy = "Admin")]
        public async Task<ApiResult<object>> UpdatePwd(string Oldpassword, string Newpassword1, string Newpassword2)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<object>();
            int UserID = GetUserID();
            if (UserID > 0)
            {
                TokenModelJwt tokenModelJwt = TokenDecode();
                int UID = C.Int(tokenModelJwt.Uid);
                try
                {
                    if (Newpassword1 == Newpassword2)
                    {

                        UserInfo m = db.Select.Where(u=>u.id==UID).ToOne();
                        if (m != null)
                        {
                            var user = db.Select.Where(a => a.userName == m.userName && a.password == MD5Encode.GetEncrypt(Oldpassword)).ToOne();
                            if (user == null)
                            {
                                res.code = (int)ApiEnum.ParameterError;
                                res.msg = "原密码错误！";
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(Newpassword1))
                                {
                                    m.password = MD5Encode.GetEncrypt(Newpassword1);
                                    res.success = db.UpdateDiy.SetSource(m).ExecuteAffrows() > 0;
                                }
                                else
                                {
                                    res.code = (int)ApiEnum.ParameterError;
                                    res.msg = "密码不能为空！";
                                }
                                if (res.success)
                                    res.code = (int)ApiEnum.Status;
                                     res.msg = "密码修改成功";
                            }
                        }
                        else
                        {
                            res.code = (int)ApiEnum.Status;
                            res.msg = "无该账号";
                        }
                    }
                    else
                    {
                        res.code = (int)ApiEnum.ParameterError;
                        res.msg = "两次密码不一样！";
                    }
                }
                catch (Exception ex)
                {
                    res.code = (int)ApiEnum.Error;
                    res.msg = ApiEnum.Error.GetEnumText() + ex.Message;
                }
            }
            else
            {
                res.code = (int)ApiEnum.Status;
                res.msg = "无法获取用户信息！";
            }
            return await Task.Run(() => res);

        }
        /// <summary>
        /// 获取左侧菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<object>> Index()
        {
            var res = new ApiResult<object>();
            // 获取登录用户数据
            int UserID = GetUserID();
            if (UserID > 0)
            {
                UserInfo user = db.Select.Where(u => u.id == UserID).ToOne();
                if (user != null)
                {
                    if ( user.RoleId > 0)
                    {
                        Role role = dbRole.Select.Where(r => r.id == user.RoleId).ToOne();
                        if (role != null)
                        {
                            // 后台菜单
                            List<MenuListModel> list = new List<MenuListModel>();
                            var menuList = dbMenu.Select.Where(a => a.open).Where(a=>a.type!=C.String(2)).ToList();
                            IEnumerable<Menu> list2 = menuList;
                            if (list2 != null && list2.Count() > 0)
                            {
                                list2 = list2.OrderBy(a => a.orderNum);
                                IEnumerable<Menu> mList1 = list2.Where(a => a.parentId == 0);
                                if (mList1 != null && mList1.Count() > 0)
                                {
                                    foreach (var item in mList1)
                                    {
                                        if (role.id == 1 || role.Menus.Contains("," + item.id + ","))
                                        {
                                            meta listmeta = new meta();
                                            listmeta.title = item.label;
                                            listmeta.icon = item.icon;
                                            listmeta.roles = item.code.Split(',').ToList();
                                            if (item.parentId==0&&dbMenu.Select.Where(a=>a.parentId==item.parentId).Any())
                                            {
                                                item.url = "Layout";
                                            }

    
                                                list.Add(new MenuListModel
                                                {
                                                    path = item.path,
                                                    component = item.url,
                                                    alwaysShow = item.open,
                                                    name = item.name,
                                                    meta = listmeta,

                                                    children = APIHelper.GetMenuList(item.id, list2, role, dbMenu.Select.Where(a => a.parentId == item.parentId).Any())
                                                });
                                            
                                            
                                        }
                                    }
                                }
                                else
                                {
                                    res.msg = "参数丢失";
                                }
                            }
                            else
                            {
                                res.msg = "参数丢失";
                            }
                           
                            res.data = list;
                            if (res.data != null)
                            {
                                res.success = true;
                            }
                        }
                        else
                        {
                            res.msg = "参数丢失";
                        }
                    }
                    else
                    {
                        res.msg = "参数丢失";
                    }
                }
                else
                {
                    res.msg = "无该用户";
                }
            }
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("Index2")]
        public async Task<ApiResult<object>> Index2()
        {
            var res = new ApiResult<object>();
            // 获取登录用户数据
            int UserID = GetUserID();
            if (UserID > 0)
            {
                UserInfo userInfo = db.Select.Where(s=>s.id ==UserID).First();
                if (userInfo != null)
                {
                  var moduleId= dbRole.Select.Where(a => a.id == userInfo.RoleId).ToOne().Modules.Split(",");
                    var moduleUrl = new List<string>();
                    foreach (var item in moduleId.Where(x => !string.IsNullOrEmpty(x)).ToArray())
                    {
                        var rolesUrls = dbmo.Select.Where(a => a.Id == C.Int(item)).ToOne().LinkUrl.Split('/').Where(s => !string.IsNullOrEmpty(s)).Where(s => s != "Admin").Where(s => s != "api").ToArray();
                        string rolesUrl = "sys";
                        foreach (var listurl in rolesUrls)
                        {
                            rolesUrl = rolesUrl + ":" + listurl;
                        }

                        moduleUrl.Add(rolesUrl);
                    }
                    res.data = new
                    {
                        avatar="",
                        introduction ="",
                        userInfo.id,
                        userInfo.userName,
                        roles = moduleUrl
                    };
                    res.success = true;
                }
                else
                {
                    res.msg = "参数丢失";
                }
            }
            else
            {
                res.msg = "无该用户";
            }
            return await Task.Run(() => res);
        }


    }
}
