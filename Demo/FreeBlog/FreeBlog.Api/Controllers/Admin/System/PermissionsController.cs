using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeBlog.Api.ApiGroup;
using FreeBlog.Api.Helper;
using FreeBlog.Common;
using FreeBlog.IService;
using FreeBlog.Model.Models;
using FreeBlog.Model.ViewModels;
using FreeBlog.Model.ViewModels.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FreeBlog.Api.Controllers.Admin.System
{
    /// <summary>
    /// 权限管理
    /// </summary>
    [Produces("application/json")]
    [Route("api/Admin/[controller]")]
    [Authorize(Policy = "Admin")]
    [ApiGroup(ApiGroupNames.AdminSystem)]
    public class PermissionsController : AdminBaseController
    {
        private readonly IUserService db;
        private readonly IRoleService roleDb;
        private readonly IMenuService menuDb;
        private readonly IModuleService dbMo;
        public PermissionsController(IUserService db, IRoleService roleDb, IMenuService menuDb,IModuleService dbMo):base(roleDb,db,dbMo)
        {
            this.db = db;
            this.roleDb = roleDb;
            this.menuDb = menuDb;
            this.dbMo = dbMo;
        }
        /// <summary>
        /// 获取角色数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetRole")]
        public async Task<ApiResult<object>> GetRoles()
        {
            var res = new ApiResult<object>() { code = (int)ApiEnum.ParameterError };
            var roleList = roleDb.Select.ToList();
            if (roleList != null)
            {
                List<BaseViewModel> roles = new List<BaseViewModel>();
                foreach (var item in roleList)
                    roles.Add(new BaseViewModel(item.id, item.name));
                res.success = true;
                res.data = roles;
                res.code = (int)ApiEnum.Status;
            }
            return await Task.Run(() => res);
        }
        /// <summary>
        /// 获取菜单数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetMenu")]
        public async Task<ApiResult<object>> GetMenus()
        {
            var res = new ApiResult<object>() { code = (int)ApiEnum.ParameterError };
            List<ELTreeViewModel> list = new List<ELTreeViewModel>();
            var data = menuDb.GetWhere(a => a.open,a => a.orderNum);
            if (data != null)
            {
                list.Add(new ELTreeViewModel
                {
                    label = "根目录",
                    children = GetMenusHelps.GetChildren(data, 0)
                });
                res.success = true;
                res.data = list;
                res.code = (int)ApiEnum.Status;
            }
            return await Task.Run(() => res);
        }


        /// <summary>
        /// 根据id显示权限
        /// </summary>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        [HttpGet("UserList")]
        public async Task<ApiResult<IEnumerable<BaseViewModel>>> UserList(int RoleID)
        {
            var res = new ApiResult<IEnumerable<BaseViewModel>>() { code = (int)ApiEnum.Status };
            if (RoleID > 0)
            {
                var list = db.GetPages(db.Select.Where(a => a.RoleId == RoleID && a.State == 2), new PageParm(1, 50));
                if (list.DataSource != null)
                {
                    List<BaseViewModel> userList = new List<BaseViewModel>();
                    foreach (var item in list.DataSource)
                        userList.Add(new BaseViewModel(item.id, item.userName + (item.loginName != "" ? "(" + item.loginName + ")" : item.nickName)));
                    res.data = userList;
                    res.count = list.TotalCount;
                }
                res.success = true;

                // 获取菜单权限
                Role role = roleDb.Select.Where(r=>r.id==RoleID).ToOne();
                if (role != null)
                    res.msg = role.Menus;
            }
            else
                res.msg = "参数丢失";
            return await Task.Run(() => res);
        }
        /// <summary>
        /// 更新权限菜单
        /// </summary>
        /// <param name="RoleID"></param>
        /// <param name="Role_value">接口表ID</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ApiResult> SetMenus(int RoleID, string Role_value,string Module_Id)
        {
            var res = new ApiResult();
            if (RoleID > 0)
            {
                if (!string.IsNullOrWhiteSpace(Role_value))
                    Role_value = "," + Role_value + ",";
                if (!string.IsNullOrWhiteSpace(Module_Id))
                    Module_Id = "," + Module_Id + ",";
                res.success = roleDb.UpdateDiy.Set(a =>a.Menus, Role_value).Set(a=>a.Modules,Module_Id).Where(a => a.id == RoleID).ExecuteAffrows() > 0;
                res.msg ="菜单权限："+ roleDb.Select.Where(r => r.id == RoleID).ToOne().Menus;
            }
            else
                res.msg = "参数丢失";
            return await Task.Run(() => res);
        }


    }
}
