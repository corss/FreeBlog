using FreeBlog.Api.ApiGroup;
using FreeBlog.Common;
using FreeBlog.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreeBlog.Model.Models.PoliceModel;
namespace FreeBlog.Api.Controllers.Admin.PoliceC
{
    /// <summary>
    /// 干警成员
    /// </summary>
    [Route("api/Admin/[controller]")]
    [Authorize(Policy = "Admin")]
    [Produces("application/json")]
    [ApiGroup(ApiGroupNames.AdminPolice)]
    public class PoliceController : AdminBaseController
    {
        private readonly IPoliceService db;
        private readonly IPoliceForceService dbForce;
        private readonly IUserService dbuser;
        private readonly IRoleService dbrole;
        private readonly IModuleService dbmo;
        public PoliceController(IPoliceService db,IPoliceForceService dbForce,IUserService dbuser,IRoleService dbrole,IModuleService dbmo):base(dbrole,dbuser,dbmo)
        {
            this.db = db;
            this.dbForce = dbForce;
            this.dbuser = dbuser;
            this.dbrole = dbrole;
            this.dbmo = dbmo;
        }
        /// <summary>
        /// 干警列表显示
        /// </summary>
        /// <param name="pageIndex">页数</param>
        /// <param name="Name">姓名</param>
        /// <param name="PFID">所属队伍</param>
        /// <param name="IsEnable">账号是否启用</param>
        /// <param name="PFRole">角色，1队长2队员</param>
        /// <returns></returns>
        [HttpGet("PoilceList")]
        public async Task<ApiResult<IEnumerable<PoliceInfo>>> TeamList(int pageIndex, int pageSize,string Name, int? PFID, bool? IsEnable, int? PFRole)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<IEnumerable<PoliceInfo>>() { code = (int)ApiEnum.Status };
            //判断有无权限
            if (!GetAccess(1,"/api/Admin/Police/PoilceList"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (pageIndex == 0)
                pageIndex = 1;
            if (PFRole == null)
                PFRole = 0;
            if (PFID == null)
                PFID = 0;
            var parm = db.Select
               .WhereIf(PFID != 0, m => dbForce.Select.As("a").Where(a => a.ID == m.PFID).Any())
               .WhereIf(IsEnable != null, m => m.IsEnable == IsEnable)
               .WhereIf(!string.IsNullOrEmpty(Name), m => m.Names == Name)
               .WhereIf(PFRole != 0, m => m.PFRole == PFRole);
            var list = db.GetPages(parm, new PageParm(pageIndex,pageSize), "PFRole,Sorting DESC");
            List<PoliceInfo> list2 = new List<PoliceInfo>();
            if (list.DataSource != null)
            {
                foreach (var item in list.DataSource)
                {
                    PoliceInfo pm = new PoliceInfo();
                    pm.ID = item.ID;
                    pm.OID = item.OID;
                    pm.Names = item.Names;
                    pm.PFID = item.PFID;
                    pm.PFRole = item.PFRole;
                    pm.Sorting = item.Sorting;
                    pm.Grade = item.Grade;
                    pm.GradeF = item.GradeF;
                    pm.AddDate = C.DateTimes(item.AddDate);
                    pm.IsEnable = item.IsEnable;
                    list2.Add(pm);
                }
                res.success = true;
                res.data = list2;
                res.index = pageIndex;
                res.count = list.TotalCount;
                res.size = list.PageSize;
                res.pages = list.TotalPages;
                res.code = (int)ApiEnum.Status;
            }
            else
            {
                res.success = false;
                res.code = (int)ApiEnum.Status;
            }
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 添加成员
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost("Addusers")]
        public async Task<ApiResult> Addusers(PoliceInfo p)
        {
            // 以接口的形式返回数据
            var res = new ApiResult();
            //判断有无权限
            if (!GetAccess(1,"/api/Admin/Police/Addusers"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            List<PoliceInfo> Police = db.Select.Where(o=>o.OID==p.OID && o.IsEnable == true).ToList();
            List<PoliceInfo> Police2 = db.Select.Where(o => o.IsEnable == true && o.PFID == p.PFID&& o.OID != p.OID&&o.PFRole==1).ToList();
            if (Police.Count > 0)
            {
                res.msg = "已有此人";
                res.success = false;
            }
            else if (Police2.Count > 0 && p.PFRole == 1 && p.IsEnable == true)
            {
                res.msg = "已有组长，请修改";
                res.success = false;
            }
            else
            {
                try
                {
                    PoliceInfo m = new PoliceInfo()
                    {
                        Names = p.Names,
                        IsEnable = p.IsEnable,
                        PFID = p.PFID,
                        PFRole = p.PFRole,
                        OID = p.OID,
                        Sorting = p.Sorting,
                        AddDate = DateTime.Now,
                        Grade=p.Grade,
                        GradeF=p.GradeF
                    };
                    res.success = db.Insert3(m) > 0;
                    if (res.success)
                    {
                        res.msg = "添加成功";
                    }
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
        /// 成员删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpDelete("SetTeamState")]
        public async Task<ApiResult<string>> SetTeamState(string ID)
        {
            var res = new ApiResult<string>();
            //判断有无权限
            if (!GetAccess(1,"/api/Admin/Police/SetTeamState"))
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

        /// <summary>
        /// 修改成员
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPut("Updateusers")]
        public async Task<ApiResult> Updateusers(PoliceInfo p)
        {
            // 以接口的形式返回数据
            var res = new ApiResult();
            //判断有无权限
            if (!GetAccess(1,"/api/Admin/Police/Updateusers"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            List<PoliceInfo> Police = db.Select.Where(o => o.OID == p.OID && o.IsEnable == true).ToList();
            List<PoliceInfo> Police2 = db.Select.Where(o => o.IsEnable == true && o.PFID == p.PFID && o.OID != p.OID && o.PFRole == 1).ToList();
            if (p.ID==0)
            {
                res.msg = "请输入ID";
                res.success = false;
            }
            else if (Police.Count > 1)
            {
                res.msg = "已有此人";
                res.success = false;
            }
            else if (Police2.Count > 0 && p.PFRole == 1 && p.IsEnable == true)
            {
                res.msg = "已有组长，请修改";
                res.success = false;
            }
            else
            {
                try
                {
                    PoliceInfo m = db.Select.Where(a => a.ID == p.ID).ToOne();
                    if (m != null)
                    {
                        m.Names = p.Names;
                        m.IsEnable = p.IsEnable;
                        m.PFID = p.PFID;
                        m.PFRole = p.PFRole;
                        m.OID = p.OID;
                        m.Sorting = p.Sorting;
                        m.AddDate = DateTime.Now;
                        m.Grade = p.Grade;
                        m.GradeF = p.GradeF;
                        res.success = db.UpdateDiy.SetSource(m).ExecuteAffrows() > 0;
                        if (res.success)
                        {
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
            // {"success":true,"message":null,"Code":200,"data":null}
            return await Task.Run(() => res);

        }


        /// <summary>
        /// 显示成员信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet("TeamItem")]
        public async Task<ApiResult<PoliceInfo>> TeamItem(int ID)
        {

            var res = new ApiResult<PoliceInfo>();
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/Police/TeamItem"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            PoliceInfo m;
            PoliceInfo pm = new PoliceInfo();
            if (ID > 0)
            {
                m = db.Select.Where(a => a.ID == ID).ToOne();
                if (m != null)
                {
                    pm.ID = m.ID;
                    pm.OID = m.OID;
                    pm.Names = m.Names;
                    pm.PFID = m.PFID;
                    pm.PFRole = m.PFRole;
                    pm.Sorting = m.Sorting;
                    pm.Grade = m.Grade;
                    pm.GradeF = m.GradeF;
                    pm.AddDate = C.DateTimes(m.AddDate);
                    pm.IsEnable = m.IsEnable;

                    res.success = true;
                    res.msg = "获取成功";
                }
            }
            res.data = pm;
            return await Task.Run(() => res);
        }

    }
}
