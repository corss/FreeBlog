using FreeBlog.Api.ApiGroup;
using FreeBlog.Common;
using FreeBlog.IService.Log;
using FreeBlog.Model.Models;
using FreeBlog.Model.Models.Log;
using FreeBlog.Model.ViewModels.Log;
using FreeSql;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FreeBlog.Api.Controllers.Admin
{
    /// <summary>
    /// 登录日志
    /// </summary>
    [Produces("application/json")]
    [Authorize(Policy = "Admin")]
    [ApiGroup(ApiGroupNames.AdminSystem)]
    public class LoginLogController : Controller
    {
        private readonly ILoginLogService db;
        private readonly IFreeSql freeSql;
        public LoginLogController(ILoginLogService db,IFreeSql freeSql)
        {
            this.db = db;
            this.freeSql = freeSql;
        }
        /// <summary>
        /// 列表显示
        /// </summary>
        /// <returns></returns>
        [HttpGet(template: "List")]
        public async Task<ApiResult<IEnumerable<LoginLogViewModel>>> List(int pageIndex, string Title, DateTime? Date1, DateTime? Date2)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<IEnumerable<LoginLogViewModel>>();
            if (pageIndex == 0) pageIndex = 1;
            List<LoginLogViewModel> list = new List<LoginLogViewModel>();
            var parm="";
            var source=db.Select.WhereIf(!string.IsNullOrEmpty(Title), m => m.Title.Contains(Title))
                .WhereIf(Date1 != null, a => a.AddDate >= Date1)
                .WhereIf(Date1 != null, a => a.AddDate >= Date1);
            var Paged = db.GetPages(source, new PageParm(pageIndex), "ID DESC");
            if (Paged.DataSource != null)
            {
                foreach (var item in Paged.DataSource)
                {
                    list.Add(new LoginLogViewModel
                    {
                        AddDate = Utility.GetDateFormat(item.AddDate),
                        Browser = item.Browser,
                        ID = item.ID,
                        Os = item.Os,
                        R_H = item.R_H,
                        R_W = item.R_W,
                        Title = item.Title,
                        UserID = item.UserID
                    });
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
            return await Task.Run(() => res);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<ApiResult<IEnumerable<LoginLog>>> Delete(string ID)
        {
            ISelect<UserInfo> select = freeSql.Select<UserInfo>();
            var res = new ApiResult<IEnumerable<LoginLog>>() { code = (int)ApiEnum.Status };
            if (!string.IsNullOrWhiteSpace(ID))
            {
                string[] array = ID.Trim(',').Split(',');
                if (array != null && array.Length > 0)
                {
                    int[] array2 = Array.ConvertAll(array, int.Parse);
                    res.success = freeSql.Delete<LoginLog>(array2).ExecuteAffrows()> 0;
                    if (res.success)
                    {
                        res.msg = "删除成功";
                        res.count = array2.Length;
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
    }
}
