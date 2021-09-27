using AutoMapper;
using FreeBlog.Api.ApiGroup;
using FreeBlog.Api.Helper;
using FreeBlog.Common;
using FreeBlog.IService;
using FreeBlog.Model.Models;
using FreeBlog.Model.Models.PoliceModel;
using FreeBlog.Model.ViewModels;
using FreeBlog.Model.ViewModels.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeBlog.Api.Controllers.Admin.Police
{
    /// <summary>
    /// 干警活动管理
    /// </summary>
    [Route("api/Admin/[controller]")]
    [Authorize(Policy = "Admin")]
    [Produces("application/json")]
    [ApiGroup(ApiGroupNames.AdminPolice)]
    public class PoliceprojectController : AdminBaseController
    {
        private readonly IPoliceprojectService db;
        private readonly IPoliceprojectMenu_PoliceprojectService dbpp;
        private readonly IPoliceprojectMenuService dbm;
        private readonly IMapper IMapper;
        private readonly IPoliceGradeService dbs;
        private readonly IUserService dbuser;
        private readonly IRoleService dbrole;
        private readonly IModuleService dbmo;

        public PoliceprojectController(IPoliceprojectService db, IPoliceprojectMenu_PoliceprojectService dbpp,
            IPoliceprojectMenuService dbm, IMapper mapper,IPoliceGradeService dbs,IUserService dbuser
            ,IRoleService dbrole,IModuleService dbmo):base(dbrole,dbuser,dbmo)
        {
            this.db = db;
            this.dbpp = dbpp;
            this.dbm = dbm;
            this.IMapper = mapper;
            this.dbs = dbs;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <returns></returns>
        [HttpGet("{ID}")]
        public async Task<ApiResult<object>> Item(int ID)
        {
            var res = new ApiResult<object>();
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/Policeproject"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (ID > 0)
            {
                Policeproject p = db.Select.Where(p => p.ID == ID).ToOne();
                if (p != null)
                {
                    List<PoliceprojectMenu_Policeproject> list = dbpp.Select.Where(m => m.ProjectID == ID).ToList();
                    List<PoliceprojectMenuModel> PoliceProjectMenuModel = new List<PoliceprojectMenuModel>();
                    List<PoliceGrade> PoliceGrade = new List<PoliceGrade>();
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            PoliceprojectMenu policeprojectMenu = dbm.Select.Where(a => a.ID == item.ProjectMenuID).ToOne();
                            PoliceGrade = dbs.Select.Where(a => a.PID == C.String(item.ProjectID)).ToList();
                            if (policeprojectMenu != null)
                            {
                                PoliceProjectMenuModel.Add(new PoliceprojectMenuModel
                                {
                                    MenuID = item.ProjectMenuID,
                                    Names = policeprojectMenu.Names


                                });
                            }
                            else
                            {
                                res.msg = "参数丢失";
                            }
                            if (PoliceGrade== null)
                            {
                                res.msg = "没有参加人员与分数";
                            }
                        }
                    }
                    if (p.Context == null)
                    {
                        p.Context = "";
                    }
                    res.data = new
                    {
                        p.ID,
                        p.Name,
                        p.MenuID,
                        AddDate = Utility.GetDateFormat(p.AddDate, 1),
                        Context = p.Context.Replace("src=\"/UploadFiles", "src=\"" + Utility.WEB_IMAGES_URL + "/UploadFiles"),    // 图片路径替换
                        p.IsEnable,
                        PoliceProjectMenuModel,
                        PoliceGrade
                    };
                }
                else
                {
                    res.msg = "参数丢失";
                }
            }
            else
            {
                res.data = new
                {
                    ID = 0,
                    Name = "",
                    proID=0,
                    MenuID = 0,
                    AddDate = DateTime.Now,
                    IsEnable = false,
                    Context = "",
                    PoliceProjectMenuModel = new List<PoliceprojectMenuModel>()
                };
                res.msg = "无数据";
            }
            if (res.data != null)
            {
                res.success = true;
            }
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 任务列表展示
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="ProjectMenuID">栏目id</param>
        /// <param name="IsEnable">状态</param>
        /// <param name="Name">标题</param>
        /// <returns></returns>
        [HttpGet("List")]
        public async Task<ApiResult<IEnumerable<PoliceProModel>>> List(int pageIndex, int pageSize,int? ProjectMenuID, bool? IsEnable, string Name,int? pId)
        {
            var res = new ApiResult<IEnumerable<PoliceProModel>>();
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/Policeproject/List"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (pageIndex == 0)
                pageIndex = 1;

            var parm = db.Select
               .WhereIf(ProjectMenuID != null && ProjectMenuID != 0, a => dbpp.Select.As("s").Where(s => s.ProjectID == a.ID && s.ProjectMenuID == ProjectMenuID).Any())
               .WhereIf(!string.IsNullOrWhiteSpace(Name), a => a.Name.Contains(Name))
               .WhereIf(IsEnable != null, a => a.IsEnable == IsEnable)
               .WhereIf(pId!=null,a=>a.MenuID==pId);
            var list = db.GetPages(parm, new Common.PageParm(pageIndex,pageSize), "Sorting,ID DESC");
            res.success = true;
            List<PoliceProModel> list2 = new List<PoliceProModel>();
            PoliceProModel pm;
            if (list.DataSource != null)
            {
                foreach (var item in list.DataSource)
                {
                    pm = IMapper.Map<PoliceProModel>(item);
                    var menuList = dbm.Select.Where(a => dbpp.Select.As("b").Where(b => b.ProjectMenuID == a.ID && b.ProjectID == item.ID).Any()).ToList();
                    if (menuList != null)
                    {
                        List<PoliceprojectMenuModel> vm2 = new List<PoliceprojectMenuModel>();
                        foreach (var item2 in menuList)
                        {
                            vm2.Add(new PoliceprojectMenuModel
                            {
                                Names = item2.Names,
                                MenuID = item2.ID
                            });
                        }
                        pm.ProjectMenu = vm2;
                    }
                    list2.Add(pm);
                }
                res.data = list2;
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
            }
            else
            {
                res.msg = "参数丢失";
                res.code = (int)ApiEnum.Status;
            }
            return await Task.Run(() => res);
        }


        /// <summary>
        /// 栏目数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetMenuLevel")]
        public async Task<ApiResult<IEnumerable<ELTreeViewModel>>> GetMenuLevel()
        {
            var res = new ApiResult<IEnumerable<ELTreeViewModel>>();
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/Policeproject/GetMenuLevel"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            List<ELTreeViewModel> MenuView = new List<ELTreeViewModel>();
            var data = dbm.Select.Where(o => o.IsEnable == true).ToList();
            ELTreeViewModel eLTreeViewModel = new ELTreeViewModel();
            if (data != null)
            {
                // 如果要看iview tree版,这里要改成Tree2ViewModel
                MenuView.Add(new ELTreeViewModel
                {
                    id = 0,
                    label = "栏目",
                    children = APIHelper.GetChildrenP(data, 0)
                });
                res.data = MenuView;
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
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        public ApiResult<string> Update([FromBody] PoliceprojectModel vm)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<string>() { code = (int)ApiEnum.ParameterError };
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/Policeproject/Update"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return res;
            }
            if (!string.IsNullOrWhiteSpace(vm.Name) && vm.ID != 0)
            {
                bool IsAdd = false;
                if (vm.ID > 0)
                {
                    Policeproject m = db.Select.Where(a => a.ID == vm.ID).ToOne();
                    if (m != null)
                    {
                        m.Name = vm.Name;
                        m.AddDate = C.DateTimes(vm.AddDate);
                        m.Sorting = vm.Sorting;
                        m.IsEnable = vm.IsEnable;
                        m.Context = vm.Context;
                        m.MenuID = vm.MenuID;
                        try
                        {
                            res.success = db.UpdateDiy.SetSource(m).ExecuteAffrows() > 0;
                            // 处理文章栏目
                            if (res.success)
                            {
                                res.code = (int)ApiEnum.Status;
                                // 先删除
                                if (!IsAdd)
                                {
                                    dbpp.Delete(a => a.ProjectID == m.ID);
                                    dbs.Delete(a => a.PID == C.String(m.ID));
                                }
                                // 再添加
                                if (vm.policeGrades!=null)
                                {
                                    foreach (var item in vm.policeGrades)
                                    {
                                        if (item.ID!=0)
                                        {
                                            item.PID = C.String(m.ID);  
                                            dbs.Insert(item);
                                        }
                                    }
                                }
                                else
                                {
                                    res.msg = "修改失败";
                                    res.code = (int)ApiEnum.Status;
                                }
                                if (vm.MenuID != 0)
                                {
                                    List<PoliceprojectMenu_Policeproject> list = new List<PoliceprojectMenu_Policeproject>();

                                    list.Add(new PoliceprojectMenu_Policeproject
                                    {
                                        ProjectID = m.ID,
                                        ProjectMenuID = C.Int(vm.MenuID)
                                    });

                                    dbpp.Insert2(list);
                                    res.msg = "修改成功";
                                }
                                else
                                {
                                    res.msg = "修改失败";
                                    res.code = (int)ApiEnum.Status;
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
                        res.msg = "参数丢失";
                        res.code = (int)ApiEnum.Status;
                    }
                }
                else
                    res.msg = "参数丢失";
            }
            else
            {
                res.msg = "请正确填写选择的id";
            }
            return res;
        }


        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public ApiResult<string> Add(PoliceprojectModel vm)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<string>() { code = (int)ApiEnum.ParameterError };
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/Policeproject/Add"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return res;
            }
            if (!string.IsNullOrWhiteSpace(vm.Name) && vm.ID != 0)
            {
                Policeproject m = new Policeproject();
                m.ID = vm.ID;
                m.Name = vm.Name;
                m.AddDate = C.DateTimes(vm.AddDate);
                m.Sorting = vm.Sorting;
                m.IsEnable = vm.IsEnable;
                m.Context = vm.Context;
                m.MenuID = vm.MenuID;
                try
                {
                    m.ID = db.Insert3(m);
                    res.success = m.ID > 0;
                    // 处理栏目
                    if (res.success)
                    {
                        if (vm.policeGrades!=null)
                        {
                            foreach (var item in vm.policeGrades)
                            {
                                if (item.ID!=0)
                                {
                                    item.PID = C.String(vm.ID);
                                    if (dbs.Insert(item).ID > 0)
                                    {
                                        res.msg = "添加成功";
                                    }
                                    else
                                    {
                                        res.msg = "分数列表添加失败";
                                        res.success = false;
                                        res.code = (int)ApiEnum.Status;
                                    }
                                }
                            }
                        }
                        if (vm.MenuID > 0)
                        {
                            List<PoliceprojectMenu_Policeproject> list = new List<PoliceprojectMenu_Policeproject>();
                            
                                list.Add(new PoliceprojectMenu_Policeproject
                                {
                                    ProjectID = m.ID,
                                    ProjectMenuID = C.Int(vm.MenuID)
                                });

                                if (dbpp.Insert2(list) > 0)
                                {
                                    res.msg = "添加成功";
                                }
                                else
                                {
                                    res.msg = "栏目添加失败";
                                    res.success = false;
                                    res.code = (int)ApiEnum.Status;
                                }
                            }

                        }
                        else
                        {
                            res.msg = "活动添加失败";
                            res.success = false;
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
                res.msg = "参数丢失";
            return res;
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpDelete("SetIsEnable")]
        public async Task<ApiResult<string>> SetIsEnable(string ID)
        {
            var res = new ApiResult<string>();
            //判断有无权限
            if (!GetAccess(1, "/api/Admin/Policeproject/SetIsEnable"))
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
                    if (db.UpdateDiy.Set(a => a.IsEnable, false)
                        .Where(a => a.ID == Convert.ToInt32(item)).ExecuteAffrows()
                          > 0)
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
