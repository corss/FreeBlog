using FreeBlog.Api.ApiGroup;
using FreeBlog.Api.Helper;
using FreeBlog.Common;
using FreeBlog.IService;
using FreeBlog.Model.Models;
using FreeBlog.Model.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeBlog.Api.Controllers.Admin
{
    /// <summary>
    /// 菜单管理
    /// </summary>
    [Produces("application/json")]
    [Route("api/Admin/[controller]")]
    [Authorize(Policy = "Admin")]
    [ApiGroup(ApiGroupNames.AdminSystem)]
    public class MenuController : AdminBaseController
    {
        private readonly IMenuService db;
        private readonly IRole_valueService role_Value;
        private readonly IFreeSql freeSql;
        private readonly IRoleService dbRole;
        private readonly IUserService dbusers;
        private readonly IModuleService dbmodule;
        public MenuController(IMenuService db, IRole_valueService role_Value, IFreeSql freeSql,
            IRoleService dbRole, IUserService dbusers, IModuleService dbmodeule) : base(dbRole, dbusers, dbmodeule)
        {
            this.db = db;
            this.role_Value = role_Value;
            this.freeSql = freeSql;
            this.dbRole = dbRole;
            this.dbusers = dbusers;
            this.dbmodule = dbmodule;
        }

        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<object>> Index()
        {
            var res = new ApiResult<object>();
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/Menu"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            // 获取登录用户数据
            int UserID = GetUserID();
            if (UserID > 0)
            {
                UserInfo user = dbusers.Select.Where(u => u.id == UserID).ToOne();
                if (user != null)
                {
                    if (user != null && user.RoleId > 0)
                    {
                        Role role = dbRole.Select.Where(r => r.id == user.RoleId).ToOne();
                        if (role != null)
                        {
                            // 后台菜单
                            List<MenuViewModel> list = new List<MenuViewModel>();
                            var menuList = db.Select.Where(a => a.open).ToList();
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
                                            if (item.parentId == 0)
                                            {
                                                list.Add(new MenuViewModel
                                                {
                                                    id = item.id,
                                                    name = item.name,
                                                    parentId = item.parentId,
                                                    parentName = item.parentName,
                                                    code = item.code,
                                                    path = item.path,
                                                    label = item.label,
                                                    url = item.url,
                                                    orderNum = item.orderNum,
                                                    type = item.type,
                                                    icon = item.icon,
                                                    remark = item.remark,
                                                    createTime = item.createTime,
                                                    updateTime = item.updateTime,
                                                    children = APIHelper.GetMenu(item.id, list2, role),
                                                    open = item.open

                                                });
                                            }
                                            else
                                            {
                                                list.Add(new MenuViewModel
                                                {
                                                    id = item.id,
                                                    parentId = item.parentId,
                                                    parentName = item.parentName,
                                                    code = item.code,
                                                    path = item.path,
                                                    name = item.label,
                                                    url = item.url,
                                                    orderNum = item.orderNum,
                                                    type = item.type,
                                                    icon = item.icon,
                                                    remark = item.remark,
                                                    createTime = item.createTime,
                                                    updateTime = item.updateTime,
                                                    children = APIHelper.GetMenu(item.id, list2, role),
                                                    open = item.open

                                                });
                                            }
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
        /// 启用禁用
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="open"></param>
        /// <returns></returns>
        [HttpPut("Enable")]
        public async Task<ApiResult<IEnumerable<Menu>>> Enable(string ID, bool open)
        {
            var res = new ApiResult<IEnumerable<Menu>>();
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/Menu/Enable"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (!string.IsNullOrWhiteSpace(ID))
            {
                try
                {
                    string[] array = ID.Trim(',').Split(',');
                    int i = 0;
                    foreach (string item in array)
                    {
                        if (db.UpdateDiy.Set(a => a.open == open).Where(a => a.id == Convert.ToInt32(item)).ExecuteAffrows() > 0)
                            i++;
                    }
                    res.success = i > 0;
                    res.count = i;
                }
                catch (Exception ex)
                {
                    res.code = (int)ApiEnum.Error;
                    res.msg = ApiEnum.Error.GetEnumText() + ex.Message;
                }
            }
            else
            {
                res.success = false;
                res.msg = "参数丢失";
            }
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 详情显示
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ApiResult<Menu>> Item(int id)
        {
            var res = new ApiResult<Menu>();
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/Menu/{id}"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (id > 0)
            {
                Menu m = db.Select.Where(m => m.id == id).ToOne();
                if (m != null)
                {
                    res.success = true;
                    res.data = m;
                }
                else
                    res.msg = "查询失败";
            }
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 查询下拉框菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet("Menu")]
        public async Task<ApiResult<IEnumerable<Menu>>> GetMenu()
        {
            var res = new ApiResult<IEnumerable<Menu>>();
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/Menu/Menu"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            List<Menu> list0 = db.Select.ToList();
            List<Menu> list = new List<Menu>();
            IEnumerable<Menu> list1 = list0.Where(m => m.parentId == 0);
            IEnumerable<Menu> list2 = null;
            foreach (var item in list1)
            {
                item.label = "└ " + item.label;
                list.Add(item);

                // 增加子级
                list2 = list0.Where(a => a.parentId == item.id);
                if (list2 != null && list2.Count() > 0)
                {
                    foreach (var item2 in list2)
                    {
                        item2.label = "　├ " + item2.label;
                    }
                    list.AddRange(list2);
                }
            }
            res.data = list;
            res.success = true;
            res.code = (int)ApiEnum.Status;
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<ApiResult> Add([FromBody] Menu menu)
        {
            // 以接口的形式返回数据
            var res = new ApiResult { code = (int)ApiEnum.ParameterError };
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/Menu/Add"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (string.IsNullOrWhiteSpace(menu.label) && string.IsNullOrWhiteSpace(menu.parentName) && string.IsNullOrWhiteSpace(menu.url) && string.IsNullOrWhiteSpace(menu.code) && menu.parentId != null)
            {
                res.msg = "参数丢失";
            }
            else
            {
                try
                {
                    Menu m = new Menu();
                    m.code = menu.code;
                    m.createTime = DateTime.Now;
                    m.icon = menu.icon;
                    m.label = menu.label;
                    m.name = menu.name;
                    m.open = menu.open;
                    m.orderNum = menu.orderNum;
                    m.parentId = menu.parentId;
                    m.parentName = menu.parentName;
                    m.path = menu.path;
                    m.remark = menu.remark;
                    m.type = menu.type;
                    m.updateTime = DateTime.Now;
                    m.url = menu.url;
                    res.success = db.Insert3(m) > 0;
                    List<Role_value> r = new List<Role_value>();
                    r.Add(new Role_value
                    {
                        Menu_name = menu.label,
                        Action_Type = "Show"
                    });
                    //r.Add(new Role_value
                    //{
                    //    Menu_name = Names,
                    //    Action_Type = "Delete"
                    //});
                    //r.Add(new Role_value
                    //{
                    //    Menu_name = Names,
                    //    Action_Type = "Add"
                    //});
                    //r.Add(new Role_value
                    //{
                    //    Menu_name = Names,
                    //    Action_Type = "Update"
                    //});

                    role_Value.Insert2(r);
                    if (res.success)
                        res.code = (int)ApiEnum.Status;
                }
                catch (Exception ex)
                {
                    res.code = (int)ApiEnum.Error;
                    res.msg = ApiEnum.Error.GetEnumText() + ex.Message;
                }
            }
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<ApiResult> Update([FromBody] Menu m)
        {
            // 以接口的形式返回数据
            var res = new ApiResult();
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/Menu/Update"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (string.IsNullOrWhiteSpace(m.label))
            {
                res.msg = "请填写名称";
                res.code = (int)ApiEnum.ParameterError;
            }
            else
            {
                try
                {
                    var names = db.Select.Where(a => a.id == m.id).ToOne().label;
                    var a = freeSql.Update<Role_value>().Set(r => r.Menu_name, m.label).Where(r => r.Menu_name == names).ExecuteAffrows();
                    res.success = db.UpdateDiy.SetSource(m).ExecuteAffrows() > 0;
                }
                catch (Exception ex)
                {
                    res.code = (int)ApiEnum.Error;
                    res.msg = ApiEnum.Error.GetEnumText() + ex.Message;
                }
            }
            return await Task.Run(() => res);
        }


        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]

        public ApiResult Delete(int id)
        {

            //判断有无权限
            if (!GetAccess(1, "/api/Admin/Menu/Delete"))
            {
                var res = new ApiResult();
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return res;
            }
            if (id > 0)
                return new ApiResult { code = (int)ApiEnum.Status, success = db.UpdateDiy.Set(a => a.open, false).Where(a => a.id == id).ExecuteAffrows() > 0 };
            return new ApiResult { code = (int)ApiEnum.ParameterError, msg = "数据丢失" };
        }
        /// <summary>
        /// 获取上级菜单列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("parent")]
        public async Task<ApiResult<object>> parent()
        {
            var res = new ApiResult<object>();

            //判断接口权限
            if (!GetAccess(1, "/api/Admin/Menu/parent"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }

            Role role = dbRole.Select.Where(r => r.id > 0).ToOne();
            if (role != null)
            {
                // 后台菜单
                List<MenuViewModel> list = new List<MenuViewModel>();
                var menuList = db.Select.Where(a => a.open).ToList();
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
                        label= "顶级菜单",
                        code = null,
                        path = null,
                        name =null,
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




            return await Task.Run(() => res);
        }
    }
}
