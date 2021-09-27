using FreeBlog.Api.ApiGroup;
using FreeBlog.Common;
using FreeBlog.IService;
using FreeBlog.Model.Models.PoliceModel;
using FreeBlog.Model.ViewModels;
using FreeSql;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FreeBlog.Api.Controllers.Admin.PoliceC
{
    /// <summary>
    /// 分数管理接口
    /// </summary>
    [Route("api/Admin/[controller]")]
    [Authorize(Policy = "Admin")]
    [Produces("application/json")]
    [ApiGroup(ApiGroupNames.AdminPolice)]
    public class PoliceGradeController : AdminBaseController
    {
        private readonly IPoliceGradeService db;
        private readonly IPoliceprojectService dbpro;
        private readonly IUserService dbuser;
        private readonly IRoleService dbRole;
        private readonly IModuleService dbmo;
        public PoliceGradeController(IPoliceGradeService db,IPoliceprojectService dbpro, IUserService dbuser, IRoleService dbRole,IModuleService dbmo) :base(dbRole,dbuser,dbmo)
        {
            this.db = db;
            this.dbpro = dbpro;
            this.dbuser = dbuser;
            this.dbRole = dbRole;
            this.dbmo = dbmo;
        }
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="Name">姓名</param>
        /// <param name="AddTime">开始区间</param>
        /// <param name="EndTime">结束区间</param>
        /// <param name="Pid">分数来源所属活动表ID，0表示由分数管理添加</param>
        /// <param name="IsProj">是否由分数管理添加</param>    
        /// <returns></returns>

        [HttpGet("GradeList")]

        public async Task<ApiResult<IEnumerable<PoliceGrade>>> List(int pageIndex, int pageSize, string Name, string AddTime, string EndTime, string Pid, bool? IsProj)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<IEnumerable<PoliceGrade>>() { code = (int)ApiEnum.Status };
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/PoliceGrade/GradeList"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (pageIndex == 0)
                pageIndex = 1;
            var parm = db.Select
                .WhereIf(!string.IsNullOrEmpty(Name), m => m.Name == Name)
                .WhereIf(!string.IsNullOrEmpty(Pid), m => m.PID == Pid)
                .WhereIf(IsProj == true, m => m.PID == "0")
                .WhereIf(IsProj == false, m => m.PID != "0")
                .WhereIf(!string.IsNullOrEmpty(AddTime) && !string.IsNullOrEmpty(EndTime)
                , m => m.AddDate > C.DateTimes(AddTime) && m.AddDate < C.DateTimes(EndTime));
            var list = db.GetPages(parm, new PageParm(pageIndex,pageSize), "Sorting,ID DESC");
            res.success = true;
            List<PoliceGrade> pagedInfo = new List<PoliceGrade>();
            res.data = list.DataSource;
            if (res.data != null && res.data.Count() > 0)
            {
                res.success = true;
                res.index = pageIndex;
                res.count = list.TotalCount;
                res.size = list.PageSize;
                res.pages = list.TotalPages;
            }
            else
            {
                res.msg = "无数据";
                res.code = (int)ApiEnum.Status;
            }

            return await Task.Run(() => res);
        }

        /// <summary>
        /// 根据id查
        /// </summary>
        /// <returns></returns>
        [HttpGet("{ID}")]
        public async Task<ApiResult<object>> Item(int ID)
        {
            var res = new ApiResult<object>();
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/PoliceGrade"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (ID > 0)
            {
                PoliceGrade m = db.Select.Where(a => a.ID == ID).ToOne();
                if (m != null)
                {
                    res.success = true;
                    res.data = new
                    {
                        m.ID,
                        m.OID,
                        m.Name,
                        m.PID,
                        m.Grade,
                        m.GradeC,
                        m.AddDate
                    };
                }
                else
                {
                    res.data = new { ID = 0, OID=0, PID=0,Name = "", Grade = 0, GradeC=0, AddDate = DateTime.Now };
                }
            }
            if (res.data != null)
            {
                res.success = true;
                res.msg = "获取成功";
            }
            return await Task.Run(() => res);
        }


        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="pg"></param>
        /// <returns></returns>

        [HttpPost("Add")]
        public async Task<ApiResult> Add(PoliceGrade pg)
        {
            // 以接口的形式返回数据
            var res = new ApiResult();
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/PoliceGrade/Add"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (!string.IsNullOrWhiteSpace(pg.Name) && pg.ID != 0)
            {
                try
                {
                    pg.PID = "0";
                    pg.AddDate = DateTime.Now;
                    res.success = db.Insert3(pg) > 0;
                    res.code = (int)ApiEnum.Status;
                    if (res.success)
                    {
                        res.msg = "添加成功";
                        res.code = (int)ApiEnum.Status;
                    }
                    else
                    {
                        res.msg = "参数丢失";
                        res.code = (int)ApiEnum.Error;

                    }
                }
                catch (Exception ex)
                {

                    res.code = (int)ApiEnum.Error;
                    res.msg = ApiEnum.Error.GetEnumText() + ex.Message;
                }
            }
            else
            {
                res.msg = "参数丢失";
                res.code = (int)ApiEnum.ParameterError;
            }
            // {"success":true,"message":null,"Code":200,"data":null}
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ID">可多行如：1,2,3</param>
        /// <returns></returns>
        [HttpDelete("Delete")]

        public async Task<ApiResult<string>> Delete(string ID)
        {
            var res = new ApiResult<string>();
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/PoliceGrade/Delete"))
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
                    if (db.DeleteDiy(item).ExecuteAffrows() > 0)
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
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="pg"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<ApiResult> Update(PoliceGrade pg)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<string>() { code = (int)ApiEnum.ParameterError };
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/PoliceGrade/Update"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (!string.IsNullOrWhiteSpace(pg.Name) && pg.ID != 0)
            {
                try
                {
                    PoliceGrade m = db.Select.Where(a => a.ID == pg.ID).ToOne();
                    if (m != null)
                    {
                        m.AddDate = pg.AddDate;
                        m.Name = pg.Name;
                        m.OID = pg.OID;
                        m.PID = pg.PID;
                        m.Grade = pg.Grade;
                        m.GradeC = pg.GradeC;
                        res.success = db.UpdateDiy.SetSource(m).ExecuteAffrows() > 0;

                        if (res.success)
                        {
                        res.code = (int)ApiEnum.Status;
                            res.msg = "修改成功";
                        }
                    }
                }
                catch (Exception ex)
                {
                    res.code = (int)ApiEnum.Error;
                    res.msg = ApiEnum.Error.GetEnumText() + ex.Message;
                }

            }
            else
            {
                res.msg = "请正确填写选择的id";
            }
            return await Task.Run(() => res);
        }
        /// <summary>
        /// 导出列表查看
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="Name"></param>
        /// <param name="AddTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        [HttpGet("GradeListRep")]

        public async Task<ApiResult<IEnumerable<PoliceGradeModel>>> ListRep(int pageIndex, string Name, string AddTime, string EndTime)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<IEnumerable<PoliceGradeModel>>() { code = (int)ApiEnum.Status };
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/PoliceGrade/GradeListRep"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (pageIndex == 0)
                pageIndex = 1;
            PagedInfo<PoliceGradeModel> page = new PagedInfo<PoliceGradeModel>();
            List<PoliceGradeModel> listG = new List<PoliceGradeModel>();
            PageParm pa = new PageParm(pageIndex);
            var parm = db.Select
                          .WithSql("SELECT OID AS ID,PID = stuff((SELECT ',' + cast(PID AS varchar(10)) FROM PoliceGrade t WHERE t.OID = PoliceGrade.OID FOR xml path('')), 1, 1, ''), SUM(Grade) as Grade,SUM(GradeC) as GradeC,Name,AddDate FROM PoliceGrade GROUP BY OID, Name,AddDate")
                          .WhereIf(!string.IsNullOrEmpty(Name), m => m.Name == Name)
                          .WhereIf(!string.IsNullOrEmpty(AddTime) && !string.IsNullOrEmpty(EndTime)
                        , m => m.AddDate > C.DateTimes(AddTime) && m.AddDate < C.DateTimes(EndTime))
                        .GroupBy(a => new { a.ID, a.PID, a.Name }).Page(pageIndex, 15).ToList(a => new { a.Key, GradeC = a.Sum(a.Value.GradeC), Grade = a.Sum(a.Value.Grade) });

            if (parm != null)
            {

                foreach (var item in parm)
                {
                    listG.Add(new PoliceGradeModel
                    {
                        OID = item.Key.ID,
                        Pid = item.Key.PID,
                        Name = item.Key.Name,
                        Grade = C.Int(item.Grade),
                        GradeC = C.Int(item.GradeC)
                    });
                }

                page.PageIndex = pa.PageIndex;
                page.PageSize = pa.PageSize;
                page.TotalCount = 0;
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

            return await Task.Run(() => res);
        }



        /// <summary>
        /// a标签页面跳转导出
        /// </summary>
        /// <param name="RoleID">选择用户</param>
        /// <param name="StateID">选择状态</param>
        /// <param name="Title">选择标题</param>
        /// <param name="Phone">选择手机号</param>
        /// <returns></returns>
        [HttpGet("Export")]
        public IActionResult Export(string Name, string AddTime, string EndTime)
        {

            //判断有无权限
            if (!GetAccess(1, "/api/Admin/PoliceGrade/Export"))
            {
                return null;
            }
            // 方法2：文件是生成在内存中
            string fileName = $"{Guid.NewGuid().ToString()}.xlsx";
            //store in memory rather than pysical directory
            var stream = new MemoryStream();
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                // add worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");
                // 表头
                worksheet.Cells[1, 1].Value = "干警编号";
                worksheet.Cells[1, 2].Value = "干警姓名";
                worksheet.Cells[1, 3].Value = "活动项目";
                worksheet.Cells[1, 4].Value = "总分";

                // 获取数据
                List<UserViewModel> list = new List<UserViewModel>();
                var parm = db.Select
                .WithSql("SELECT OID AS ID,PID = stuff((SELECT ',' + cast(PID AS varchar(10)) FROM PoliceGrade t WHERE t.OID = PoliceGrade.OID FOR xml path('')), 1, 1, ''), SUM(Grade) as Grade,SUM(GradeC) as GradeC,Name,AddDate FROM PoliceGrade GROUP BY OID, Name,AddDate")
                 .WhereIf(!string.IsNullOrEmpty(Name), m => m.Name == Name)
                  .WhereIf(!string.IsNullOrEmpty(AddTime) && !string.IsNullOrEmpty(EndTime)
                 , m => m.AddDate > C.DateTimes(AddTime) && m.AddDate < C.DateTimes(EndTime))
                   .GroupBy(a => new { a.ID, a.PID, a.Name }).ToList(a => new { a.Key, GradeC = a.Sum(a.Value.GradeC), Grade = a.Sum(a.Value.Grade) });

                if (parm != null && parm.Count > 0)
                {
                    int a = 2;
                    foreach (var item in parm)
                    {
                        string[] array = item.Key.PID.Trim(',').Split(',');
                        List<string> arrayList = new List<string>();
                        foreach (var item1 in array.Distinct().ToArray())
                        {
                            arrayList.Add(dbpro.Select.Where(a=>a.ID==C.Int(item1)).ToOne().Name);
                        }
                        worksheet.Cells["A" + a].Value = item.Key.ID;
                        worksheet.Cells["B" + a].Value = item.Key.Name;
                        worksheet.Cells["C" + a].Value = string.Join(",", arrayList.ToArray());
                        if (item.Grade>=10)
                        {
                            worksheet.Cells["D" + a].Value = item.Grade;
                        }
                        else if(item.Grade+(item.GradeC/1000)>=10)
                        {
                            worksheet.Cells["D" + a].Value = 10;
                        }
                        else
                        {
                            worksheet.Cells["D" + a].Value = item.Grade + (item.GradeC / 1000);
                        }
                        
                        a++;
                    }
                }
                package.Save();
            }
            stream.Position = 0;
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }


}
