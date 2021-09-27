using FreeBlog.Api.ApiGroup;
using FreeBlog.Common;
using FreeBlog.IService;
using FreeBlog.Model.Models.Log;
using FreeBlog.Model.ViewModels.Log;
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
    /// 异常日志
    /// </summary>
    [Produces("application/json")]
    [Route("api/Admin/[controller]")]
    [Authorize(Policy = "Admin")]
    [ApiGroup(ApiGroupNames.AdminSystem)]
    public class ExceptionLogController : ControllerBase
    {

        private readonly IExceptionLogService db;
        private readonly IFreeSql freeSql;
        public ExceptionLogController(IExceptionLogService db, IFreeSql freeSql)
        {
            this.db = db;
            this.freeSql = freeSql;
        }
        /// <summary>
        /// 列表显示
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<IEnumerable<ExceptionLogViewModel>>> List(int pageIndex, int? TypeID, string Title, DateTime? Date1, DateTime? Date2, bool? IsSolve)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<IEnumerable<ExceptionLogViewModel>>();
            if (pageIndex == 0) pageIndex = 1;
            List<ExceptionLogViewModel> list = new List<ExceptionLogViewModel>();
          var source=  db.Select.WhereIf(!string.IsNullOrEmpty(Title), m => m.Title.Contains(Title))
                .WhereIf(TypeID != null, m => m.Type == TypeID)
                .WhereIf(IsSolve != null, m => m.IsSolve == IsSolve)
                .WhereIf(Date1 != null, a => a.AddDate >= Date1)
                .WhereIf(Date1 != null, a => a.AddDate >= Date1);
            //Expression<Func<ExceptionLog, bool>> parm = null;
            //parm.And(!string.IsNullOrEmpty(Title), m => m.Title.Contains(Title));
            //parm.And(TypeID != null, m => m.Type == TypeID);
            //parm.And(IsSolve != null, m => m.IsSolve == IsSolve);
            //parm.And(Date1 != null, a => a.AddDate >= Date1);
            //parm.And(Date1 != null, a => a.AddDate >= Date1);
            var Paged = db.GetPages(source, new PageParm(pageIndex), "ID DESC");
            var data = Paged.DataSource;
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(new ExceptionLogViewModel
                    {
                        AddDate = Utility.GetDateFormat(item.AddDate),
                        ID = item.ID,
                        Title = item.Title,
                        UserID = item.UserID,
                        IsSolve = item.IsSolve,
                        Type = item.Type
                    });
                }
                res.data = list;
                if (res.data != null && res.data.Count() > 0)
                {
                    res.success = true;
                    res.index = pageIndex;
                    res.count = Paged.TotalCount;
                    res.size = Paged.PageSize;
                }
                else
                {
                    res.msg = "暂无数据";
                    res.code = (int)ApiEnum.Status;
                }
            }
            else
            {
                res.msg = "参数丢失";
                res.code = (int)ApiEnum.Status;
            }
            return await Task.Run(() => res);
        }
                /// <summary>
        /// 批量处理
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ApiResult<ExceptionLog>> Solve(string ID)
        {
            var res = new ApiResult<ExceptionLog>();
            if (!string.IsNullOrWhiteSpace(ID))
            {
                string[] array = ID.Trim(',').Split(',');
                int i = 0;
                ExceptionLog m = new ExceptionLog();
                foreach (string item in array)
                {
                    m = db.Select.Where(s => s.ID == int.Parse(item)).First();
                    if (m != null)
                    {
                        m.IsSolve = true;
                        if (db.UpdateAsync(m).Id > 0)
                            i++;
                    }
                    else
                    {
                        res.msg = "参数丢失";
                        res.code = (int)ApiEnum.Status;
                    }

                }
                res.success = i > 0;
                if (res.success)
                {
                    res.msg = "处理成功";
                    res.count = i;
                }
                else
                {
                    res.msg = "处理失败";
                    res.code = (int)ApiEnum.Status;
                }

            }
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ApiResult<object>> Delete(string ID)
        {
            var res = new ApiResult<object>();
            if (!string.IsNullOrWhiteSpace(ID))
            {
                string[] array = ID.Trim(',').Split(',');
                if (array != null && array.Length > 0)
                {
                    int[] array2 = Array.ConvertAll(array, int.Parse);
                    res.success = freeSql.Delete<ExceptionLog>(array2).ExecuteAffrows() > 0 ? true : false;
                    if (res.success)
                    {
                        res.count = array2.Length;
                        res.msg = "删除成功";
                    }
                    else
                    {
                        res.msg = "删除失败";
                        res.code = (int)ApiEnum.Status;
                    }

                }
            }
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 详情查看
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ApiResult<ExceptionLogViewModel>> Item(int id)
        {
            ExceptionLogViewModel vm = new ExceptionLogViewModel();
            var res = new ApiResult<ExceptionLogViewModel>();
            if (id > 0)
            {
                ExceptionLog m = db.Select.Where(s => s.ID == id).First();
                if (m != null)
                {
                    vm.ID = m.ID;
                    vm.AddDate = Utility.GetDateFormat(m.AddDate);
                    vm.Title = m.Title;
                    vm.Type = m.Type;
                    vm.Content = m.Content;
                }
                else
                {
                    res.msg = "参数丢失";
                    res.code = (int)ApiEnum.Status;
                }
            }
            res.data = vm;
            if (res.data != null)
            {
                res.success = true;
            }
            return await Task.Run(() => res);
        }
    }
}
