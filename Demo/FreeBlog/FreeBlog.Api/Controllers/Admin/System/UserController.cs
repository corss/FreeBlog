using FreeBlog.Api.ApiGroup;
using FreeBlog.Common;
using FreeBlog.Common.Security;
using FreeBlog.IService;
using FreeBlog.Model.Models;
using FreeBlog.Model.ViewModels;
using FreeBlog.Model.ViewModels.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FreeBlog.Api.Controllers.Admin.System
{
    /// <summary>
    /// 用户
    /// </summary>
    [Authorize(Policy = "Admin")]
    [Produces("application/json")]
    [Route("api/Admin/[controller]")]
    [ApiGroup(ApiGroupNames.AdminSystem)]
    public class UserController : AdminBaseController
    {
        private readonly IUserService db;
        private readonly IRoleService roleDb;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IModuleService dbmo;
        public UserController(IUserService db, IRoleService roleDb, IWebHostEnvironment _webHostEnvironmentm, IModuleService dbmo) : base(roleDb, db, dbmo)
        {
            this.db = db;
            this.roleDb = roleDb;
            this._webHostEnvironment = _webHostEnvironment;
            this.dbmo = dbmo;
        }

        /// <summary>
        /// 自己详情
        /// </summary>
        /// <returns></returns>
        [HttpGet("Item")]
        public async Task<object> Item()
        {
            var res = new ApiResult<object>();
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/User/Item"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            int UserID = GetUserID();
            if (UserID > 0)
            {
                res.data = db.Select.Where(u => u.id == UserID).ToList();
                if (res.data != null)
                    res.success = true;
                else
                {
                    res.msg = "无数据";
                    res.code = (int)ApiEnum.Status;
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
        /// 根据Id查询用户
        /// </summary>
        /// <returns></returns>
        [HttpGet("id")]
        public async Task<ApiResult<object>> Item(int id)
        {
            var res = new ApiResult<object>();
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/User/{id}"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            int UserID = GetUserID();
            if (UserID > 0)
            {
                UserInfo m;
                UserViewModel d;
                if (id > 0)
                {
                    d = new UserViewModel();
                    m = db.Select.Where(u => u.id == id).ToOne();
                    if (m != null)
                    {
                        d.id = m.id;
                        d.isAdmin = m.isAdmin;
                        d.accountNonExpired = m.accountNonExpired;
                        d.accountNonLocked = m.accountNonLocked;
                        d.authorities = m.authorities;
                        d.createTime = C.String(m.createTime);
                        d.credentialsNonExpired = m.credentialsNonExpired;
                        d.deptId = m.deptId;
                        d.deptName = m.depName;
                        d.email = m.email;
                        d.enabled = m.enabled;
                        d.loginName = m.loginName;
                        d.mobile = m.mobile;
                        d.nickName = m.nickName;
                        d.password = m.password;
                        d.permissionList = null;
                        d.postId = m.postId;
                        d.postName = m.postName;
                        d.sex = m.sex;
                        d.updateTime = C.String(m.updateTime);
                        d.username = m.userName;
                        if (m.RoleId == 0)
                        {
                            d.RoleName = "";
                        }
                        else
                        {
                            d.RoleName = roleDb.Select.Where(r => r.id == m.RoleId).ToOne().name;
                        }
                        //d.AddDate = C.String(m.AddDate);
                    }
                    else
                    {
                        res.msg = "参数丢失";
                        res.code = (int)ApiEnum.Status;
                    }
                }
                else
                {
                    d = new UserViewModel();
                    // 设置默认值用于页面显示
                    //d.RoleID = 0;
                    //d.State = 2;
                }
                res.data = d;
                if (res.data != null)
                {
                    res.success = true;
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
        /// 获取角色全部信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetRoleViewModel")]
        public async Task<ApiResult<IEnumerable<BaseViewModel>>> GetRoleViewModel()
        {
            var roleList = roleDb.Select.ToList();
            var res = new ApiResult<IEnumerable<BaseViewModel>>();
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/User/GetRoleViewModel"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            int UserID = GetUserID();
            if (UserID > 0)
            {
                List<BaseViewModel> roles = new List<BaseViewModel>();
                if (roleList != null)
                {
                    foreach (var item in roleList)
                    {
                        roles.Add(new BaseViewModel(item.id, item.name));
                    }
                    res.data = roles;
                    if (res.data != null)
                    {
                        res.success = true;
                        res.code = (int)ApiEnum.Status;
                    }
                    else
                    {
                        res.msg = "无数据";
                    }
                }
                else
                {
                    res.msg = "参数丢失";
                }
            }
            else
            {
                res.msg = "无法获取用户信息！";
                res.code = (int)ApiEnum.Status;
            }

            return await Task.Run(() => res);
        }



        /// <summary>
        /// 分页查询角色id
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="name"></param>
        /// <param name="pagSize">页容量</param>
        /// <param name="roleId"></param>
        /// <param name="userId">当前登录用户id</param>
        /// <returns></returns>
        [HttpGet("{userId}")]
        public async Task<ApiResult<IEnumerable<UserViewModel>>> List(int pageIndex,string name,int pagSize,int roleId,int userId)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<IEnumerable<UserViewModel>>();
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/User/{userId}"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            int UserID = GetUserID();
            if (UserID > 0&&UserID== userId)
            {
                if (pageIndex == 0) pageIndex = 1;
                List<UserViewModel> list = new List<UserViewModel>();
                var parm = db.Select
                    .WhereIf(!string.IsNullOrEmpty(name), m => m.userName.Contains(name) || m.nickName.Contains(name))
                    .WhereIf(roleId != 0, m => m.RoleId == roleId);
                var Paged = db.GetPages(parm, new PageParm(pageIndex,pagSize), "ID DESC");
                var data = Paged.DataSource;
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        UserViewModel sysUserView = new UserViewModel();
                        sysUserView.id = item.id;
                        sysUserView.isAdmin = item.isAdmin;
                        sysUserView.loginName = item.loginName;
                        sysUserView.mobile = item.mobile;
                        sysUserView.nickName = item.nickName;
                        sysUserView.password = item.password;
                        sysUserView.permissionList = null;
                        sysUserView.postId = item.postId;
                        sysUserView.postName = item.postName;
                        sysUserView.accountNonExpired = item.accountNonExpired;
                        sysUserView.accountNonLocked = item.accountNonLocked;
                        sysUserView.authorities = item.authorities;
                        sysUserView.createTime = C.String(item.createTime);
                        sysUserView.credentialsNonExpired = item.credentialsNonExpired;
                        sysUserView.deptId = item.deptId;
                        sysUserView.deptName = item.depName;
                        sysUserView.email = item.email;
                        sysUserView.enabled = item.enabled;
                        sysUserView.sex = item.sex;
                        sysUserView.updateTime = C.String(item.updateTime);
                        sysUserView.username = item.userName;
                        if (item.RoleId == 0)
                        {
                            sysUserView.RoleName = "";
                        }
                        else
                        {
                            sysUserView.RoleName = roleDb.Select.Where(r => r.id == item.RoleId).ToOne().name;
                        }
                        list.Add(sysUserView);
                    }
                    res.data = list;
                    if (res.data != null)
                    {
                        res.success = true;
                        res.index = pageIndex;
                        res.count = Paged.TotalCount;
                        res.size = Paged.PageSize;
                    }
                    else
                    {
                        res.msg = "无数据";
                        res.code = (int)ApiEnum.Status;
                    }
                }
                else
                {
                    res.msg = "参数丢失";
                    res.code = (int)ApiEnum.Status;
                }
            }
            else
            {
                res.msg = "无法获取用户信息！";
                res.code = (int)ApiEnum.Status;
            }
            return await Task.Run(() => res);
        }



        /// <summary>
        /// 根据部门id查用户
        /// </summary>
        /// <param name="currentPage">当前页</param>
        /// <param name="name"></param>
        /// <param name="pagSize">页容量</param>
        /// <param name="deptId"></param>
        /// <param name="userId">当前登录用户id</param>
        /// <returns></returns>
        [HttpGet("listByDept")]
        public async Task<ApiResult<IEnumerable<UserViewModel>>> list1(int currentPage, string name, int pageSize, int deptId, int userId)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<IEnumerable<UserViewModel>>();
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/User/{deptId}"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            int UserID = GetUserID();
            if (UserID > 0 && UserID == userId)
            {
                if (currentPage == 0) currentPage = 1;
                List<UserViewModel> list = new List<UserViewModel>();
                var parm = db.Select
                    .WhereIf(!string.IsNullOrEmpty(name), m => m.userName.Contains(name) || m.nickName.Contains(name))
                    .WhereIf(deptId != 0, m => m.deptId == deptId);
                var Paged = db.GetPages(parm, new PageParm(currentPage, pageSize), "ID DESC");
                var data = Paged.DataSource;
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        UserViewModel sysUserView = new UserViewModel();
                        sysUserView.id = item.id;
                        sysUserView.isAdmin = item.isAdmin;
                        sysUserView.loginName = item.loginName;
                        sysUserView.mobile = item.mobile;
                        sysUserView.nickName = item.nickName;
                        sysUserView.password = item.password;
                        sysUserView.permissionList = null;
                        sysUserView.postId = item.postId;
                        sysUserView.postName = item.postName;
                        sysUserView.accountNonExpired = item.accountNonExpired;
                        sysUserView.accountNonLocked = item.accountNonLocked;
                        sysUserView.authorities = item.authorities;
                        sysUserView.createTime = C.String(item.createTime);
                        sysUserView.credentialsNonExpired = item.credentialsNonExpired;
                        sysUserView.deptId = item.deptId;
                        sysUserView.deptName = item.depName;
                        sysUserView.email = item.email;
                        sysUserView.enabled = item.enabled;
                        sysUserView.sex = item.sex;
                        sysUserView.updateTime = C.String(item.updateTime);
                        sysUserView.username = item.userName;
                        if (item.RoleId == 0)
                        {
                            sysUserView.RoleName = "";
                        }
                        else
                        {
                            sysUserView.RoleName = roleDb.Select.Where(r => r.id == item.RoleId).ToOne().name;
                        }
                        list.Add(sysUserView);
                    }
                    res.data = list;
                    if (res.data != null)
                    {
                        res.success = true;
                        res.index = currentPage;
                        res.count = Paged.TotalCount;
                        res.size = Paged.PageSize;
                    }
                    else
                    {
                        res.msg = "无数据";
                        res.code = (int)ApiEnum.Status;
                    }
                }
                else
                {
                    res.msg = "参数丢失";
                    res.code = (int)ApiEnum.Status;
                }
            }
            else
            {
                res.msg = "无法获取用户信息！";
                res.code = (int)ApiEnum.Status;
            }
            return await Task.Run(() => res);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<ApiResult<string>> Delete(string ID)
        {
            var res = new ApiResult<string>();
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/User/Delete"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            int UserID = GetUserID();
            if (UserID > 0)
            {
                if (!string.IsNullOrWhiteSpace(ID))
                {
                    string[] array = ID.Trim(',').Split(',');
                    if (array != null && array.Length > 0)
                    {
                        int[] array2 = Array.ConvertAll(array, int.Parse);
                        foreach (int item in array2)
                        {
                          
                            if (db.UpdateDiy.Set(a => a.State, 4).Where(a => a.id == item).ExecuteAffrows() > 0)
                                res.count++;
                        }
                        res.success = res.count > 0;
                        if (res.success)
                        {
                            res.msg = "删除成功";
                        }
                        else
                        {
                            res.msg = "删除失败";
                            res.code = (int)ApiEnum.Status;
                        }
                    }
                }
            }
            else
            {
                res.msg = "无法获取用户信息！";
                res.code = (int)ApiEnum.Status;
            }
            return await Task.Run(() => res);
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost("SysItem")]
        public async Task<ApiResult<string>> SysItem([FromBody] UserInfo vm)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<string>();
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/User/SysItem"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            int UserID = GetUserID();
            if (UserID > 0)
            {
                if (!string.IsNullOrWhiteSpace(vm.userName))
                {
                    try
                    {
                        UserInfo m = new UserInfo();
                        m.accountNonExpired = vm.accountNonExpired;
                        m.accountNonLocked = vm.accountNonLocked;
                        m.authorities = vm.authorities;
                        m.createTime = vm.createTime;
                        m.credentialsNonExpired = vm.credentialsNonExpired;
                        m.depName = vm.depName;
                        m.deptId = vm.deptId;
                        m.email = vm.email;
                        m.enabled = vm.enabled;
                        m.isAdmin = vm.isAdmin;
                        m.loginName = vm.loginName;
                        m.mobile = vm.mobile;
                        m.nickName = vm.nickName;
                        m.postId = vm.postId;
                        m.postName = vm.postName;
                        m.RoleId = vm.RoleId;
                        m.sex = vm.sex;
                        m.State = vm.State;
                        m.updateTime = vm.updateTime;
                        m.userName = vm.userName;
                        // 设置默认密码
                        if (string.IsNullOrWhiteSpace(vm.password))
                            vm.password = "888888";
                        // 如果设置了密码、就进行加密
                        if (!string.IsNullOrWhiteSpace(vm.password))
                            m.password = MD5Encode.GetEncrypt(vm.password);
                        res.success = db.Insert(m).id > 0;
                        if (res.success)
                        {
                            res.msg = "添加成功";
                        }
                        else
                        {
                            res.msg = "添加失败";
                            res.code = (int)ApiEnum.Status;
                        }
                    }
                    catch (Exception ex)
                    {
                        res.code = (int)ApiEnum.Error;
                        res.msg = ApiEnum.Error.GetEnumText() + ex.Message;
                    }
                }
                else
                    res.msg = "参数丢失";
            }
            else
            {
                res.msg = "无法获取用户信息！";
                res.code = (int)ApiEnum.Status;
            }
            return await Task.Run(() => res);
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPut("Add")]
        public async Task<ApiResult<string>> SysItem2([FromBody] UserInfo vm)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<string>();
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/User/Add"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            int UserID = GetUserID();
            if (UserID > 0)
            {
                if (!string.IsNullOrWhiteSpace(vm.userName))
                {
                    UserInfo m = db.Select.Where(u => u.id == UserID).First();
                    if (m != null)
                    {
                        m.accountNonExpired = vm.accountNonExpired;
                        m.accountNonLocked = vm.accountNonLocked;
                        m.authorities = vm.authorities;
                        m.createTime = vm.createTime;
                        m.credentialsNonExpired = vm.credentialsNonExpired;
                        m.depName = vm.depName;
                        m.deptId = vm.deptId;
                        m.email = vm.email;
                        m.enabled = vm.enabled;
                        m.isAdmin = vm.isAdmin;
                        m.loginName = vm.loginName;
                        m.mobile = vm.mobile;
                        m.nickName = vm.nickName;
                        m.postId = vm.postId;
                        m.postName = vm.postName;
                        m.RoleId = vm.RoleId;
                        m.sex = vm.sex;
                        m.State = vm.State;
                        m.updateTime = vm.updateTime;
                        m.userName = vm.userName;
                        // 设置默认密码
                        if (string.IsNullOrWhiteSpace(vm.password))
                            vm.password = "888888";
                        // 如果设置了密码、就进行加密
                        if (!string.IsNullOrWhiteSpace(vm.password))
                            m.password = MD5Encode.GetEncrypt(vm.password);
                    }
                    else
                    {
                        res.msg = "参数丢失";
                    }
                    try
                    {
                        res.success = db.UpdateAsync(m).Id > 0;
                        if (res.success)
                        {
                            res.msg = "修改成功";
                        }
                        else
                        {
                            res.msg = "修改失败";
                            res.code = (int)ApiEnum.Status;
                        }
                    }
                    catch (Exception ex)
                    {
                        res.code = (int)ApiEnum.Error;
                        res.msg = ApiEnum.Error.GetEnumText() + ex.Message;
                    }
                }
                else
                    res.msg = "参数丢失";
            }
            else
            {
                res.msg = "无法获取用户信息！";
                res.code = (int)ApiEnum.Status;
            }
            return await Task.Run(() => res);
        }
    }
}


