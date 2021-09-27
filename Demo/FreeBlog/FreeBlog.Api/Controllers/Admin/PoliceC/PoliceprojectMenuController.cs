using FreeBlog.Api.ApiGroup;
using FreeBlog.Api.Helper;
using FreeBlog.Common;
using FreeBlog.IService;
using FreeBlog.Model.Models.PoliceModel;
using FreeBlog.Model.ViewModels.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeBlog.Api.Controllers.Admin.PoliceC
{
    /// <summary>
    /// 干警项目管理
    /// </summary>
    [Produces("application/json")]
    [Route("api/Admin/[controller]")]
    [Authorize(Policy = "Admin")]
    [ApiGroup(ApiGroupNames.AdminPolice)]

    public class PoliceprojectMenuController : AdminBaseController
    {
        private readonly IPoliceprojectMenuService db;
        private readonly IRoleService dbrole;
        private readonly IUserService dbuser;
        private readonly IModuleService dbmo;

        public PoliceprojectMenuController(IPoliceprojectMenuService db, IRoleService dbrole, IUserService dbuser, IModuleService dbmo) : base(dbrole, dbuser, dbmo)
        {
            this.db = db;
        }

        /// <summary>
        /// 项目列表
        /// </summary>
        /// <param name="IsEnable">是否开启</param>
        /// <param name="BeginDate">开始时间</param>
        /// <param name="EndDate">结束时间</param>
        /// <param name="Fid">所负责小队</param>
        /// <returns></returns>
        [HttpGet("List")]
        public async Task<ApiResult<IEnumerable<ELTreeViewModel>>> List(bool? IsEnable = null, string BeginDate = null, string EndDate = null,int? Fid=null)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<IEnumerable<ELTreeViewModel>>();
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/PoliceprojectMenu/List"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            List<ELTreeViewModel> list = new List<ELTreeViewModel>();
            var parm = db.Select
                .WhereIf(IsEnable != null, m => m.IsEnable == IsEnable)
                .WhereIf(Fid != null, M => M.Fid == Fid)
                .WhereIf(!string.IsNullOrWhiteSpace(BeginDate) && !string.IsNullOrWhiteSpace(EndDate), 
                m => m.AddDate > C.DateTimes(BeginDate) && m.AddDate < C.DateTimes(EndDate));
            var data = db.GetWhere(parm).OrderBy(a => a.Sorting);
            ELTreeViewModel eLTreeViewModel = new ELTreeViewModel();
            if (data != null)
            {
                // 如果要看iview tree版,这里要改成Tree2ViewModel
                list.Add(new ELTreeViewModel
                {
                    label = "根目录",
                    children = APIHelper.GetChildrenP(data, 0)
                });
                res.data = list;
                if (res.data != null && res.data.Count() > 0)
                {
                        res.success = true;
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
        /// 根据id查
        /// </summary>
        /// <returns></returns>
        [HttpGet("{ID}")]
        public async Task<ApiResult<object>> Item(int ID)
        {
            var res = new ApiResult<object>();
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/PoliceprojectMenu"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (ID > 0)
            {
                PoliceprojectMenu m = db.Select.Where(a => a.ID == ID).ToOne();
                if (m != null)
                {
                    res.success = true;
                    res.data = new
                    {
                        m.ParentID,
                        m.Names,
                        m.IsEnable,
                        m.AddDate,
                        m.Fid,
                        m.Sorting
                    };
                }
                else
                {
                    res.data = new { ParentID = 0, Names = "",IsEnable=false,AddDate=DateTime.Now, Fid="",Sorting = "" };
                }
            }
            if (res.data != null)
            {
                res.success = true;
            }
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 评测项目新增
        /// </summary>
        /// <param name="ParentID">父类id</param>
        /// <param name="Names">名称</param>
        /// <param name="Sorting">顺序</param>
        /// <param name="Fid">负责小队ID</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<ApiResult<string>> Item(int ParentID, string Names, int Sorting,int Fid)
        {
            var res = new ApiResult<string>();
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/PoliceprojectMenu/Add"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (string.IsNullOrWhiteSpace(Names))
            {
                res.msg = "请填写名称";
            }
            else if (ParentID < 0)
            {
                res.msg = "请正确填写父类id";
            }
            else if (Fid < 0)
            {
                res.msg = "请正确填写负责小队id";
            }
            else
            {
                PoliceprojectMenu m = new PoliceprojectMenu();
                PoliceprojectMenu am = db.Select.Where(a => a.ID == ParentID).ToOne();
                if (ParentID == 0 || am != null)
                {
                    m.Names = Names;
                    m.Sorting = Sorting;
                    m.ParentID = ParentID;
                    m.IsEnable = true;
                    m.AddDate = DateTime.Now;
                    m.Fid = Fid;
                    try
                    {
                        res.success = db.Insert3(m) > 0;
                        if (res.success)
                            res.msg = "添加成功";
                        else
                        {
                            res.msg = "添加失败";
                            res.code = (int)ApiEnum.Status;
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
                    res.msg = "父类填写错误，无该父类";
                }
            }
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 评测项目更新
        /// </summary>
        /// <param name="ID">选择id</param>
        /// <param name="ParentID">父类id</param>
        /// <param name="Names">名称</param>
        /// <param name="Sorting">顺序</param>
        /// <param name="AddDate">添加时间</param>
        /// <param name="Fid">负责小队ID</param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<ApiResult<string>> Item(int ID, int ParentID, string Names, int Sorting,string AddDate,int Fid)
        {
            var res = new ApiResult<string>();
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/PoliceprojectMenu/Update"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (string.IsNullOrWhiteSpace(Names))
            {
                res.msg = "请填写名称";
            }
            else if (ID > 0 && ID == ParentID)
            {
                res.msg = "类型不能在自己之下！";
            }
            else if (ParentID < 0)
            {
                res.msg = "请正确填写父类id";
            }
            else if (Fid < 0)
            {
                res.msg = "请正确填写负责小队id";
            }
            else
            {
                try
                {
                    PoliceprojectMenu m = db.Select.Where(a => a.ID == ID).ToOne();
                    PoliceprojectMenu am = db.Select.Where(a => a.ID == ParentID).ToOne();
                    if (ParentID == 0 || am != null)
                    {
                        if (m != null)
                        {
                            m.Names = Names;
                            m.Sorting = Sorting;
                            m.ParentID = ParentID;
                            m.IsEnable = true;
                            m.AddDate = C.DateTimes(AddDate);
                            m.Fid = Fid;
                            res.success = db.UpdateDiy.SetSource(m).ExecuteAffrows() > 0;
                            if (res.success)
                                res.msg = "修改成功";
                            else
                            {
                                res.msg = "修改失败";
                                res.code = (int)ApiEnum.Status;
                            }
                        }
                        else
                        {
                            res.msg = "栏目查询失败！";
                            res.code = (int)ApiEnum.Error;
                        }
                    }
                    else
                    {
                        res.msg = "父类填写错误，无该父类";
                    }
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
        /// 更换父级
        /// </summary>
        /// <returns></returns>
        [HttpPut("ModifyParentID")]
        public async Task<ApiResult<string>> ModifyParentID(int ID, int ParentID)
        {
            var res = new ApiResult<string>();
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/PoliceprojectMenu/ModifyParentID"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (ID > 0)
            {
                res.success = db.UpdateDiy.Set(a => a.ParentID, ParentID).Where(a => a.ID == ID).ExecuteAffrows() > 0;
                if (res.success)
                {
                    res.msg = "更换成功";
                }
                else
                {
                    res.msg = "更换失败";
                    res.code = (int)ApiEnum.Status;
                }
            }
            else
                res.msg = "参数丢失";
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 启用禁用
        /// </summary>
        /// <returns></returns>
        [HttpPut("Enable")]
        public async Task<ApiResult<string>> Enable(string ID)
        {
            var res = new ApiResult<string>();
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/PoliceprojectMenu/Enable"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (!string.IsNullOrWhiteSpace(ID))
            {
                string[] array = ID.Trim(',').Split(',');
                int i = 0;
                bool IsEnable = false;
                foreach (string item in array)
                {
                    if (db.UpdateDiy.Set(a => a.IsEnable, IsEnable).Where(a => a.ID == Convert.ToInt32(item)).ExecuteAffrows() > 0)
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
