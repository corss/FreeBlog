using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeBlog.Api.ApiGroup;
using FreeBlog.Common;
using FreeBlog.IService;
using FreeBlog.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FreeBlog.Api.Controllers.Admin.System
{
    /// <summary>
    /// 网络基本配置
    /// </summary>
    [Produces("application/json")]
    [Route("api/Admin/[controller]")]
    [Authorize(Policy = "Admin")]
    [ApiGroup(ApiGroupNames.AdminSystem)]
    public class ConfigBaseController : ControllerBase
    {
        public readonly IConfigBaseService db;
        public ConfigBaseController(IConfigBaseService db)
        {
            this.db = db;
        }
        [HttpGet]
        public async Task<ApiResult<string>> Item(ConfigBase m)
        {
            var res = new ApiResult<string>() { code = (int)ApiEnum.ParameterError };
            try
            {
                if (m.ID == 0)
                {
                    //添加、返回是否成功
                    res.success = db.Insert3(m) > 0;
                }
                else
                {
                    //更新功能
                    res.success = db.UpdateDiy.SetSource(m).ExecuteAffrows() > 0;

                }
                //如果是true返回成功的状态码
                if (res.success)
                {
                    res.code = (int)ApiEnum.Status;
                }
            }
            catch (Exception e)
            {
                res.code = (int)ApiEnum.Error;
                res.msg = ApiEnum.Error.GetEnumText() + e.Message;
            }
            return await Task.Run(() => res);
        }
    }
}
