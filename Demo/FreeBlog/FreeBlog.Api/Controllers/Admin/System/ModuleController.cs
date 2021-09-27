using FreeBlog.Api.ApiGroup;
using FreeBlog.Common;
using FreeBlog.IService;
using FreeBlog.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FreeBlog.Api.Controllers.Admin.System
{
    /// <summary>
    /// 接口列表
    /// </summary>
    [Produces("application/json")]
    [Route("api/Admin/[controller]")]
    [Authorize(Policy = "Admin")]
    [ApiGroup(ApiGroupNames.AdminSystem)]

    public class ModuleController : ControllerBase
    {
        private readonly IModuleService db;
        private readonly IUserService dbuser;
        private readonly IRoleService dbRole;
        public ModuleController(IModuleService db, IUserService dbuser, IRoleService dbRole)
        {
            this.db = db;
            this.dbRole = dbRole;
            this.dbuser = dbuser;
        }


        /// <summary>
        /// 获取全部接口api
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        [HttpGet("List1")]
        public async Task<ApiResult<IEnumerable<Module>>> List1()
        {
            // 以接口的形式返回数据
            var res = new ApiResult<IEnumerable<Module>>() { code = (int)ApiEnum.Status };
            Expression<Func<Module, bool>> parm = null;
            var data = db.Select.ToList() ;
            res.success = true;
            res.data = data;
            return await Task.Run(() => res);
        }
        /// <summary>
        /// 获取全部接口api分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<IEnumerable<Module>>> List(int pageIndex, string Name)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<IEnumerable<Module>>() { code = (int)ApiEnum.Status };
            if (pageIndex == 0) pageIndex = 1;
            //DBPage dp = new DBPage(pageIndex, 15);

            Expression<Func<Module, bool>> parm = null;
            //db.Select.WhereIf(!string.IsNullOrEmpty(title), m => m.Names.Contains(title));
            //parm.And(!string.IsNullOrEmpty(title), m => m.Names.Contains(title));
            //var data = db.GetPages(parm.ToExpression(), new PageParm(pageIndex), "Sorting,ID DESC");
            var data = db.GetPages(db.Select.WhereIf(!string.IsNullOrEmpty(Name), m => m.Name.Contains(Name)), new PageParm(pageIndex), "OrderSort,Id DESC");
            res.success = true;
            res.data = data.DataSource;
            res.index = pageIndex;
            res.count = data.TotalCount;
            res.size = data.PageSize;
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 详情显示
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ApiResult<Module>> Item(int id)
        {
            var res = new ApiResult<Module>();
            if (id > 0)
            {
                Module m = db.Select.Where(m => m.Id == id).ToOne();
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
        /// 添加一条接口信息
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        // POST: api/User
        [HttpPost]
        public async Task<ApiResult<string>> Post([FromBody] Module module)
        {
            var data = new ApiResult<string>();

            var id = db.Insert3(module);
            data.success = id > 0;
            if (data.success)
            {
                data.data = id.ObjToString();
                data.msg = "添加成功";
            }

            return data;
        }

        /// <summary>
        /// 更新接口信息
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        // PUT: api/User/5
        [HttpPut]
        public async Task<ApiResult<string>> Put([FromBody] Module module)
        {
            var data = new ApiResult<string>();
            if (module != null && module.Id > 0)
            {
                data.success =  db.UpdateDiy.SetSource(module).ExecuteAffrows()>0;
                if (data.success)
                {
                    data.msg = "更新成功";
                    data.data = module?.Id.ObjToString();
                }
            }

            return data;
        }
        /// <summary>
        /// 删除接口
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Enabled"></param>
        /// <returns></returns>
        [HttpPut("Enable")]
        public async Task<ApiResult<IEnumerable<Module>>> Enable(string ID, bool Enabled)
        {
            var res = new ApiResult<IEnumerable<Module>>();
            if (!string.IsNullOrWhiteSpace(ID))
            {
                try
                {
                    string[] array = ID.Trim(',').Split(',');
                    int i = 0;
                    foreach (string item in array)
                    {
                        if (db.UpdateDiy.Set(a => a.Enabled == Enabled).Where(a => a.Id == Convert.ToInt32(item)).ExecuteAffrows() > 0)
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
    }
}
