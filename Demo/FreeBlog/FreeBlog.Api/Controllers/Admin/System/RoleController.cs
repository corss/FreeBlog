using FreeBlog.Api.ApiGroup;
using FreeBlog.Api.Helper;
using FreeBlog.Common;
using FreeBlog.IService;
using FreeBlog.Model.Models;
using FreeBlog.Model.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FreeBlog.Api.Controllers.Admin.System
{

    /// <summary>
    /// 角色接口
    /// </summary>
    [Authorize(Policy = "Admin")]
    [Produces("application/json")]
    [Route("api/Admin/[controller]")]
    [ApiGroup(ApiGroupNames.AdminSystem)]
    public class RoleController : AdminBaseController
    {
        private readonly IRoleService db;
        private readonly IFreeSql freeSql;
        private readonly IUserService dbUser;
        private readonly IModuleService dbModule;
        private readonly IMenuService dbMenu;
        public RoleController(IRoleService db ,IFreeSql freeSql,IUserService dbUser,IModuleService dbModule,IMenuService dbMenu):base(db,dbUser,dbModule)
        {
            this.db = db;
            this.freeSql = freeSql;
            this.dbUser = dbUser;
            this.dbModule = dbModule;
            this.dbMenu = dbMenu;
        }


        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        //[Authorize(Roles = "system")]
        [HttpGet(template: "List2")]
        public async Task<ApiResult<List<Role>>> List2()
        {

            var res = new ApiResult<List<Role>>() { code = (int)ApiEnum.ParameterError };
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/Role/List2"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            res.success = true;
            res.code = (int)ApiEnum.Status;
            res.data = db.Select.Where(a => true).ToList();
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 列表显示带分页
        /// </summary>
        /// <returns></returns>
        [HttpGet("List")]
        public async Task<ApiResult<IEnumerable<RoleViewModel>>> List(int pageIndex, int  pageSize,string title)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<IEnumerable<RoleViewModel>>() { code = (int)ApiEnum.Status };
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/Role/List"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (pageIndex == 0) pageIndex = 1;
            //DBPage dp = new DBPage(pageIndex, 15);
            //改变格式了，改回来的话下面全部注释掉
            PagedInfo<RoleViewModel> page = new PagedInfo<RoleViewModel>();
            List<RoleViewModel> listG = new List<RoleViewModel>();
            PageParm pa = new PageParm(pageIndex, pageSize);
            var parm = db.Select
               .WhereIf(!string.IsNullOrEmpty(title), m => m.name.Contains(title)).Page(pageIndex, pageSize).ToList();
            if (parm != null)
            {

                foreach (var item in parm)
                {
                    listG.Add(new RoleViewModel
                    {
                        id=item.id,
                        name=item.name,
                        remark=item.remark
                    });
                }

                page.PageIndex = pa.PageIndex;
                page.PageSize = pa.PageSize;
                page.TotalCount = db.Select
               .WhereIf(!string.IsNullOrEmpty(title), m => m.name.Contains(title)).ToList().Count();
                page.TotalPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(page.TotalCount) / page.PageSize));
                page.DataSource = listG.ToList();
                res.success = true;
                res.data = page.DataSource;
                if (res.data != null && res.data.Count() > 0)
                {
                    res.success = true;
                    res.index = pageIndex;
                    res.count = page.TotalCount;
                    res.size = page.PageSize;
                    res.pages = page.TotalPages;
                    res.msg = "数据获取成功";
                }
                else
                {
                    res.msg = "无数据";
                    res.code = (int)ApiEnum.Status;
                }
            }
            //var list = db.GetPages(parm, new PageParm(pageIndex,pageSize), "PFRole,Sorting DESC");
            //var data= db.GetPages(parm, new PageParm(pageIndex,pageSize), "Sorting,ID DESC");
            //res.success = true;
            //res.data = page.DataSource;
            //res.index = pageIndex;
            //res.count = page.TotalCount;
            //res.size = page.PageSize;
            return await Task.Run(() => res);
        }
        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        //[ApiExplorerSettings(IgnoreApi = true)] //隐藏接口
        public async Task<ApiResult> Add([FromBody] Role role)
        {
            // 以接口的形式返回数据
            var res = new ApiResult { code = (int)ApiEnum.ParameterError };
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/Role/Add"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (string.IsNullOrWhiteSpace(role.name))
            {
                res.msg = "请填写标题";
            }
            else
            {
                try
                {
            
                    res.success = db.Insert3(role) > 0;
                    res.msg = "添加成功";
                    if (res.success)
                        res.code = (int)ApiEnum.Status;
                }
                catch (Exception ex)
                {
                    res.code = (int)ApiEnum.Error;
                    res.msg = ApiEnum.Error.GetEnumText() + ex.Message;
                }
            }
            // {"success":true,"message":null,"Code":200,"data":null}
            return await Task.Run(() => res);
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        //[ApiExplorerSettings(IgnoreApi = true)] //隐藏接口
        //public async Task<ApiResult<string>> Update(Role m)
        public async Task<ApiResult> Update([FromBody] Role m)
        {
            // 以接口的形式返回数据
            var res = new ApiResult();
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/Role/Update"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (string.IsNullOrWhiteSpace(m.name))
            {
                res.msg = "请填写标题";
                res.code = (int)ApiEnum.ParameterError;
            }
            else
            {
                try
                {
                    res.success = freeSql.GetRepository<Role>().Update(m)> 0;
                }
                catch (Exception ex)
                {
                    res.code = (int)ApiEnum.Error;
                    res.msg = ApiEnum.Error.GetEnumText() + ex.Message;
                }
            }
            // {"success":true,"message":null,"Code":200,"data":null}
            return await Task.Run(() => res);
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpDelete("{ID}")]
        
        public ApiResult Delete(int ID)
        {
            // 以接口的形式返回数据
            var res = new ApiResult();
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/Role/{ID}"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return res;
            }
            if (ID > 0)
                return new ApiResult { code = (int)ApiEnum.Status, success = freeSql.Delete<Role>(ID).ExecuteAffrows() > 0 };
            return new ApiResult { code = (int)ApiEnum.ParameterError, msg = "数据丢失" };
        }



        /// <summary>
        /// 分配权限--树形数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAssignPermissionTree")]
        public async Task<ApiResult<object>> getAssignPermissionTree( int roleId,int userId)
        {
            var res = new ApiResult<object>();
            //判断接口权限
            if (!GetAccess(1, "/api/Role/getAssignPermissionTree"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            int UserID = GetUserID();
            if (UserID!= userId)
            {
                res.success = false;
                res.code = (int)ApiEnum.HttpRequestError;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            Role role = db.Select.Where(r => r.id > 0).ToOne();
            if (role != null)
            {
                // 后台菜单
                List<MenuViewModel> list = new List<MenuViewModel>();
               MenuRoleView datalist = new MenuRoleView();
                var menuList = dbMenu.Select.Where(a => a.open).ToList();
                IEnumerable<Menu> list2 = menuList;
                if (list2 != null && list2.Count() > 0)
                {
                    list2 = list2.OrderBy(a => a.orderNum);
                    IEnumerable<Menu> mList1 = list2.Where(a => a.parentId == 0);


                    list.Add(new MenuViewModel
                    {
                        id = 0,
                        parentId = -1,
                        parentName = null,
                        label = "顶级菜单",
                        code = null,
                        path = null,
                        name = null,
                        url = null,
                        orderNum = 0,
                        type = null,
                        icon = null,
                        remark = null,
                        createTime = DateTime.Now,
                        updateTime = DateTime.Now,
                        children = APIHelper.GetMenu(0, list2, role),
                        open = true

                    });
                }
                else
                {
                    res.msg = "参数丢失";
                }
                var checkList = db.Select.Where(a => a.id == roleId).ToOne().Menus;
                if (checkList!=null)
                {
                    datalist.checkList = checkList.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToList();
                }
                else
                {
                    datalist.checkList = new List<string>();
                }
                datalist.listmenu = list;

                res.data = datalist;
                if (res.data != null)
                {
                    res.success = true;
                }
            }
            else
            {
                res.msg = "参数丢失";
            }




            return await Task.Run(() => res);
        }
        /// <summary>
        /// 分配权限--保存
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="list">菜单权限ID</param>
        /// <returns></returns>
        [HttpPost("roleAssignSave")]
        public async Task<ApiResult> roleAssignSave([FromBody] roleAssignSaveModel r)
        {
            var res = new ApiResult();
            string Menu_Id = ",";
            if (r.roleId > 0)
            {
                if (r.list != null)
                {
                    foreach (var item in r.list)
                    {
                        Menu_Id = Menu_Id + item + ",";
                    }
                }
                else
                {
                    Menu_Id = null;
                }

                res.success = db.UpdateDiy.Set(a => a.Menus, Menu_Id).Where(a => a.id == r.roleId).ExecuteAffrows() > 0;
                res.msg = "菜单权限：" + db.Select.Where(rs => rs.id == r.roleId).ToOne().Menus;
            }
            else
                res.msg = "参数丢失";
            return await Task.Run(() => res);
        }


        ///// <summary>
        ///// 分配权限--保存
        ///// </summary>
        ///// <param name="roleId"></param>
        ///// <param name="list">菜单权限ID</param>
        ///// <returns></returns>
        //[HttpPost("roleAssignSave")]
        //public async Task<ApiResult> roleAssignSave( int roleId, List<string> list)
        //{
        //    var res = new ApiResult();
        //    string Menu_Id = ",";
        //    if (roleId > 0)
        //    {
        //        if (list!=null)
        //        {
        //            foreach (var item in list)
        //            {
        //                Menu_Id = Menu_Id + item + ",";
        //            }
        //        }
        //        else
        //        {
        //            Menu_Id = null;
        //        }

        //        res.success = db.UpdateDiy.Set(a => a.Menus, Menu_Id).Where(a => a.id == roleId).ExecuteAffrows() > 0;
        //        res.msg = "菜单权限：" + db.Select.Where(r => r.id == roleId).ToOne().Menus;
        //    }
        //    else
        //        res.msg = "参数丢失";
        //    return await Task.Run(() => res);
        //}
    }
}
