using FreeBlog.Api.ApiGroup;
using FreeBlog.Common;
using FreeBlog.IService;
using FreeBlog.Model.Models.PoliceModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FreeBlog.Api.Controllers.Admin.Poilce
{
    /// <summary>
    /// 干警队伍
    /// </summary>
    [Route("api/Admin/[controller]")]
    [Authorize(Policy = "Admin")]
    [Produces("application/json")]
    [ApiGroup(ApiGroupNames.AdminPolice)]
    public class PoliceForceController : AdminBaseController
    {
        private readonly IPoliceForceService db;
        private readonly IUserService dbuser;
        private readonly IRoleService dbrole;
        private readonly IModuleService dbmo;

        public PoliceForceController(IPoliceForceService db,IUserService dbuser,IRoleService dbrole,IModuleService dbmo):base(dbrole,dbuser,dbmo)
        {
            this.db = db;
            this.dbuser = dbuser;
            this.dbrole = dbrole;
            this.dbmo = dbmo;
        }
        /// <summary>
        /// 查看列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("List2")]
        public async Task<ApiResult<List<PoliceForce>>> List2()
        {
            var res = new ApiResult<List<PoliceForce>>() { code = (int)ApiEnum.ParameterError };
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/PoliceForce/List2"))
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
        public async Task<ApiResult<IEnumerable<PoliceForce>>> List(int pageIndex, int pageSize,string title, bool IsEnable)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<IEnumerable<PoliceForce>>() { code = (int)ApiEnum.Status };
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/PoliceForce/List"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (pageIndex == 0) pageIndex = 1;
            //DBPage dp = new DBPage(pageIndex, 15);

            Expression<Func<PoliceForce, bool>> parm = null;
            var data = db.GetPages(db.Select.WhereIf(!string.IsNullOrEmpty(title), m => m.Names.Contains(title)).Where(m => m.IsEnable == IsEnable), new PageParm(pageIndex,pageSize), "Sorting,ID DESC");
            res.success = true;
            res.data = data.DataSource;
            res.index = pageIndex;
            res.count = data.TotalCount;
            res.size = data.PageSize;
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="Names">名称</param>
        /// <param name="Sorting">顺序</param>
        /// <param name="pid">队长ID</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<ApiResult> Add(string Names, int Sorting, int pid)
        {
            // 以接口的形式返回数据
            var res = new ApiResult { code = (int)ApiEnum.ParameterError };
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/PoliceForce/Add"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (string.IsNullOrWhiteSpace(Names))
            {
                res.msg = "请填写标题";
            }
            else
            {
                try
                {
                    PoliceForce m = new PoliceForce();
                    m.Names = Names;
                    m.Sorting = Sorting;
                    m.PID = pid;
                    m.AddDate = DateTime.Now;
                    m.IsEnable = true;
                    res.success = db.Insert(m).ID > 0;
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
        /// 修改
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>

        [HttpPut("Update")]
        public async Task<ApiResult> Update(PoliceForce m)
        {
            // 以接口的形式返回数据
            var res = new ApiResult();
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/PoliceForce/Update"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (string.IsNullOrWhiteSpace(m.Names))
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
        /// 数据删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpDelete("{ID}")]

        public ApiResult Delete(int ID)
        {

            //判断有无权限
            if (!GetAccess(1, "/api/Admin/PoliceForce"))
            {
                var res = new ApiResult();
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return res;
            }
            if (ID > 0)
                return new ApiResult { code = (int)ApiEnum.Status, success = db.DeleteDiy(ID).ExecuteAffrows() > 0 };
            return new ApiResult { code = (int)ApiEnum.ParameterError, msg = "数据丢失" };
        }

        /// <summary>
        /// 删除,修改状态为未启用
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpDelete("SetTeamState")]
        public async Task<ApiResult<string>> SetTeamState(string ID)
        {
            var res = new ApiResult<string>();
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/PoliceForce/SetTeamState"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (!string.IsNullOrWhiteSpace(ID))
            {
                string[] array = ID.Trim(',').Split(',');
                int i = 0;
                foreach (string item in array)
                {
                    if (db.UpdateDiy.Set(a => a.IsEnable == false).Where(a => a.ID == Convert.ToInt32(item)).ExecuteAffrows() > 0)
                        i++;
                }
                res.success = i > 0;
                if (res.success)
                {
                    res.msg = "删除成功";
                }
                else
                {
                    res.msg = "删除失败";
                    res.code = (int)ApiEnum.Status;
                }
                res.count = i;
            }
            return await Task.Run(() => res);
        }
    }
}
