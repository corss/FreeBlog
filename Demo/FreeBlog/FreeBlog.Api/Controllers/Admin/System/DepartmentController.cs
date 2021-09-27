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
    /// 部门设置
    /// </summary>
    [Produces("application/json")]
    [Route("api/Admin/[controller]")]
    [Authorize(Policy = "Admin")]
    [ApiGroup(ApiGroupNames.AdminSystem)]
    public class DepartmentController : AdminBaseController
    {
        private readonly IDepartmentService db;
        private readonly IUserService dbUser;
        private readonly IRoleService dbRole;
        private readonly IModuleService dbModule;

        public DepartmentController(IDepartmentService db,IUserService dbUser,IRoleService dbRole,IModuleService dbModule):base(dbRole,dbUser,dbModule)
        {
            this.db = db;
            this.dbRole = dbRole;
            this.dbUser = dbUser;
            this.dbModule = dbModule;
        }

        ///// <summary>
        ///// 列表
        ///// </summary>
        ///// <returns></returns>
        ////[Authorize(Roles = "system")]
        //[HttpGet(template: "List2")]
        //public async Task<ApiResult<List<Department>>> List2()
        //{

        //    var res = new ApiResult<List<Department>>() { Code = (int)ApiEnum.ParameterError };
        //    //判断接口权限
        //    if (!GetAccess(1, "/api/Admin/Department/List2"))
        //    {
        //        res.code = (int)ApiEnum.Unauthorized;
        //        res.msg = ApiEnum.Unauthorized.GetEnumText();
        //        return await Task.Run(() => res);
        //    }
        //    res.success = true;
        //    res.code = (int)ApiEnum.Status;
        //    res.data = db.Select.Where(a => true).ToList();
        //    return await Task.Run(() => res);
        //}
        ///// <summary>
        ///// /列表显示待分页
        ///// </summary>
        ///// <param name="pageIndex">第几页</param>
        ///// <param name="title">标题</param>
        ///// <returns></returns>
        //[HttpGet("List")]
        //public async Task<ApiResult<IEnumerable<Department>>> List(int pageIndex, string title)
        //{
        //    // 以接口的形式返回数据
        //    var res = new ApiResult<IEnumerable<Department>>() { Code = (int)ApiEnum.Status };
        //    //判断接口权限
        //    if (!GetAccess(1, "/api/Admin/Department/List"))
        //    {
        //        res.code = (int)ApiEnum.Unauthorized;
        //        res.msg = ApiEnum.Unauthorized.GetEnumText();
        //        return await Task.Run(() => res);
        //    }
        //    if (pageIndex == 0) pageIndex = 1;
        //    //DBPage dp = new DBPage(pageIndex, 15);

        //    Expression<Func<Department, bool>> parm = null;
        //    PagedInfo<Department> data = null;
        //    data = db.GetPages(db.Select.WhereIf(!string.IsNullOrEmpty(title), m => m.name.Contains(title)), new PageParm(pageIndex), "Sorting,ID DESC");
        //    res.success = true;
        //    res.data = data.DataSource;
        //    res.index = pageIndex;
        //    res.count = data.TotalCount;
        //    res.size = data.PageSize;
        //    return await Task.Run(() => res);
        //}


        /// <summary>
        /// 部门树形列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<object>> Index( string searchName)
        {
            var res = new ApiResult<object>();
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/Department"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            // 获取登录用户数据
            int UserID = GetUserID();
            if (UserID > 0)
            {
                UserInfo user = dbUser.Select.Where(u => u.id == UserID).ToOne();
                if (user != null)
                {
                    if (user != null && user.RoleId > 0)
                    {
                        //Department Department = db.Select.Where(r => r.pid == user.CompanyID).ToOne();
                        Department Department = db.Select.Where(r => r.open).ToOne();
                        if (Department != null)
                        {
                            // 后台菜单
                            List<DepartmentViewModel> list = new List<DepartmentViewModel>();
                            var menuList = db.Select.Where(a => a.open).WhereIf(!string.IsNullOrEmpty(searchName),a=>a.name.Contains(searchName)).ToList();
                            IEnumerable<Department> list2 = menuList;
                            if (list2 != null && list2.Any())
                            {
                                list2 = list2.OrderBy(a => a.orderNum);
                                IEnumerable<Department> mList1 = null;
                                if (!string.IsNullOrEmpty(searchName))
                                {
                                     mList1 = list2;
                                }
                                else
                                {
                                   mList1 = list2.Where(a => a.pid == 0);
                                }
                                //IEnumerable<Department> mList1 = list2;
                                if (mList1 != null && mList1.Count() > 0)
                                {
                                    foreach (var item in mList1)
                                    {

                                            list.Add(new DepartmentViewModel
                                            {
                                                id = item.id,
                                                pid = item.pid,
                                                likeId = item.likeId,
                                                manager = item.manager,
                                                name = item.name,
                                                parentName = item.parentName,
                                                deptAddress =item.deptAddress,
                                                deptCode=item.deptCode,
                                                deptPhone=item.deptPhone,
                                                orderNum=item.orderNum,
                                                open = item.open,
                                                childMenus = APIHelper.GetDepartment(item.id, list2 )

                                            });
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
        /// 添加
        /// </summary>
        /// <param name="Names">名称</param>
        /// <param name="Sorting">顺序</param>
        /// <param name="Remake">备注</param>
        /// <returns></returns>
        [HttpPost("Add")]
        //[ApiExplorerSettings(IgnoreApi = true)] //隐藏接口
        public async Task<ApiResult> Add([FromBody]Department department)
        {
            // 以接口的形式返回数据
            var res = new ApiResult { code = (int)ApiEnum.ParameterError };
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/Department/Add"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (string.IsNullOrWhiteSpace(department.name))
            {
                res.msg = "请填写部门名称";
            }
            else
            {
                try
                {
                    Department m = new Department();
                    m.deptAddress = department.deptAddress;
                    m.deptCode = department.deptCode;
                    m.deptPhone = department.deptPhone;
                    m.likeId = department.likeId;
                    m.manager = department.manager;
                    m.name = department.name;
                    m.open = department.open;
                    m.orderNum = department.orderNum;
                    m.parentName = department.parentName;
                    m.pid = department.pid;
                    res.success = db.Insert(m).id > 0;
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
        /// 删除部门
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]

        public ApiResult Delete(int ID)
        {

            //判断有无权限
            if (!GetAccess(1, "/api/Admin/Department/Delete"))
            {
                var res = new ApiResult();
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return res;
            }
            if (ID > 0)
                return new ApiResult { code = (int)ApiEnum.Status, success = db.UpdateDiy.Set(a=>a.open,false).Where(a=>a.id==ID).ExecuteAffrows() > 0 };
            return new ApiResult { code = (int)ApiEnum.ParameterError, msg = "数据丢失" };
        }
        /// <summary>
        /// 编辑部门
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
            
        [HttpPut("Update")]
        public async Task<ApiResult> Update([FromBody]Department m)
        {
            // 以接口的形式返回数据
            var res = new ApiResult();
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/Department/Update"))
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
                    res.success = db.UpdateDiy.SetSource(m).ExecuteAffrows() > 0;
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
        /// 根据id查询部门
        /// </summary>
        /// <returns></returns>
        [HttpGet("{ID}")]
        public async Task<ApiResult<object>> Item(int ID)
        {
            var res = new ApiResult<object>();
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/Department/{ID}"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (ID > 0)
            {
                Department m = db.Select.Where(a => a.id == ID).ToOne();
                if (m != null)
                {
                    res.success = true;
                    res.data = new
                    {
                        m.id,
                        m.pid,
                        m.likeId,
                        m.parentName,
                        m.manager,
                        m.name,
                        m.deptAddress,
                        m.deptCode,
                        m.deptPhone,
                        m.orderNum,
                        m.open
                    };
                }
                else
                {
                    res.data = new { ID = 0, Names = "", Sorting = "", Remark = "" };
                }
            }
            if (res.data != null)
            {
                res.success = true;
            }
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 根据上级部门id获取部门列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("pid")]
        public async Task<ApiResult<object>> List(int pid)
        {
            var res = new ApiResult<object>();
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/Department/List/pid"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (pid > 0)
            {
                List<Department> m = db.Select.Where(a => a.pid == pid).ToList();
                if (m != null)
                {
                    res.success = true;
                    res.data = m;
                }
                else
                {
                    res.data = new { ID = 0, Names = "", Sorting = "", Remark = "" };
                }
            }
            if (res.data != null)
            {
                res.success = true;
            }
            return await Task.Run(() => res);
        }


        /// <summary>
        /// 上级部门树
        /// </summary>
        /// <returns></returns>
        [HttpGet("parent")]
        public async Task<ApiResult<object>> parent()
        {
            var res = new ApiResult<object>();
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/Department/parent"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            Department dept = db.Select.Where(r => r.id > 0).ToOne();
            if (dept != null)
            {
                // 后台菜单
                List<DepartmentViewModel> list = new List<DepartmentViewModel>();
                var departmentsList = db.Select.Where(a => a.open).ToList();
                IEnumerable<Department> list2 = departmentsList;
                if (list2 != null && list2.Count() > 0)
                {
                    list2 = list2.OrderBy(a => a.orderNum);
                    IEnumerable<Department> mList1 = list2.Where(a => a.pid == 0);


                    list.Add(new DepartmentViewModel
                    {
                        id = 0,
                        pid = -1,
                        likeId = "0",
                        manager = null,
                        name = "顶级部门",
                        parentName = null,
                        deptAddress = null,
                        deptCode = null,
                        deptPhone = null,
                        orderNum = 0,
                        open = true,
                        childMenus = APIHelper.GetDepartment(0, list2)

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
