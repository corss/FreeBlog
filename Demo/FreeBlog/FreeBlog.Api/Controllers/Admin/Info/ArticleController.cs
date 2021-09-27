using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FreeBlog.Api.ApiGroup;
using FreeBlog.Api.Helper;
using FreeBlog.Common;
using FreeBlog.IService;
using FreeBlog.Model.Models;
using FreeBlog.Model.ViewModels;
using FreeBlog.Model.ViewModels.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FreeBlog.Api.Controllers.Admin.Info
{
    /// <summary>
    /// 任务分配
    /// </summary>
    [Produces("application/json")]
    [Route("api/Admin/[controller]")]
    [Authorize(Policy = "Admin")]
    [ApiGroup(ApiGroupNames.SBook)]
    public class ArticleController : AdminBaseController
    {
        private readonly IArticleService db;
        private readonly IArticleMenuService articleMenuDB;
        private readonly IMenuService MenuDB;
        private readonly IArticleMenu_ArticleService articleMenu_ArticleService;
        private readonly IMapper IMapper;
        private readonly IUserService dbuser;
        private readonly IRoleService dbrole;
        private readonly IModuleService dbmo;
        public ArticleController(IArticleService articleService, IArticleMenuService articleMenuDB, IArticleMenu_ArticleService articleMenu_ArticleService, 
            IMapper IMapper, IMenuService MenuDB, IUserService dbuser,IRoleService dbrole,IModuleService dbmo) :base(dbrole,dbuser,dbmo)
        {
            this.db = articleService;
            this.articleMenuDB = articleMenuDB;
            this.articleMenu_ArticleService = articleMenu_ArticleService;
            this.IMapper = IMapper;
            this.MenuDB = MenuDB;
            this.dbuser = dbuser;
            this.dbrole = dbrole;
        }
        /// <summary>
        /// 查看详情,输入相应ID查询对应ID的内容，除超级管理员（系统管理员）外登陆，只能查看自己部门的任务分配详情内容
        /// </summary>
        /// <param name="ID">任务表ID</param>
        /// <returns></returns>
        [HttpGet("{ID}")]
        public async Task<ApiResult<object>> Item(int ID)
        {
            var res = new ApiResult<object>();
            //判断有无权限
            if (!GetAccess(1,"/api/Admin/Article"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (ID > 0)
            {
                // 获取登录用户数据
                int UserID = GetUserID();
                Article m = new Article();
                if (UserID > 0)
                {
                    UserInfo userInfo = dbuser.Select.Where(s => s.id == UserID).ToOne();
                    if (userInfo.RoleId == 1)
                    {
                        m = db.Select.Where(a => a.ID == ID).ToOne();
                    }
                    else
                    {
                        m = db.Select.Where(a => a.ID == ID && a.DepartmentID== userInfo.deptId).ToOne();
                    }
                }
                if (m != null)
                {
                    List<ArticleMenu_Article> list2 = articleMenu_ArticleService.Select.Where(a => a.ArticleID == ID).ToList();
                    List<ArticleMenuViewModel> Articlemenumodel = new List<ArticleMenuViewModel>();
                    if (list2 != null)
                    {
                        foreach (var item in list2)
                        {
                            ArticleMenu articmenus = articleMenuDB.Select.Where(a => a.ID == item.ArticleMenuID).ToOne();
                            if (articmenus != null)
                            {
                                Articlemenumodel.Add(new ArticleMenuViewModel
                                {
                                    id = item.ArticleMenuID,
                                    Names = articmenus.Names
                                });
                            }
                            else
                            {
                                res.msg = "参数丢失";
                            }
                        }

                    }
                    if (m.Contents == null)
                    {
                        m.Contents = "";
                    }
                    res.data = new
                    {
                        m.ID,
                        m.Title,
                        AddDate = Utility.GetDateFormat(m.AddDate, 1),
                        Contents = m.Contents.Replace("src=\"/UploadFiles", "src=\"" + Utility.WEB_IMAGES_URL + "/UploadFiles"),    // 图片路径替换
                        m.DepartmentID,
                        m.FileUrl,
                        m.State,
                        m.Synopsis,
                        m.Sorting,
                        FileSize = m.FileSize + "",
                        FileFormat = m.FileFormat + "",
                        EndDate = Utility.GetDateFormat(m.EndDate, 1),
                        CompleteDate = Utility.GetDateFormat(m.CompleteDate, 1),
                        m.IsComplete,
                        m.Month,
                        m.Numth,
                        m.NumMonth,
                        m.NumYear,
                        Articlemenumodel
                    };
                    //}
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
                    Title = "",
                    AddDate = DateTime.Now,
                    Contents = "",
                    Source = "",
                    ImgUrl = "",
                    Type = 1,
                    FileUrl = "",
                    Keyword = "",
                    Sorting = 1,
                    IsTop = false,
                    State = 2,
                    Synopsis = "",
                    PageView = 0,
                    Author = "",
                    FileSize = "",
                    FileFormat = "",
                    EndDate = DateTime.Now,
                    CompleteDate = DateTime.Now,
                    IsComplete=false,
                    Articlemenumodel = new List<ArticleMenuModel>()
                };
                res.msg = "无数据";
            }
            if (res.data != null)
            {
                res.success = true;
                res.msg = "成功获取！";
            }
            

            return await Task.Run(() => res);
        }

        /// <summary>
        /// 任务列表展示,分页查看，默认是15条数据一页，超级管理员(系统管理员)能查看所有的任务列表,其他角色只能查看自己部门的任务列表,
        /// 通过查看是否完成得到代办列表，通过查看是否与其获得逾期列表
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="ArticleMenuID">栏目id</param>
        /// <param name="State">状态</param>
        /// <param name="Title">标题</param>
        /// <param name="IsComplete">是否完成</param>
        /// <param name="IsOvertime">是否逾期</param>
        /// <returns></returns>
        [HttpGet("List")]
        public async Task<ApiResult<IEnumerable<ArticleViewModel>>> List(int pageIndex,  int pageSize, int? ArticleMenuID, int? State, string Title,bool ? IsComplete,bool? IsOvertime)
        {
            var res = new ApiResult<IEnumerable<ArticleViewModel>>();
            //判断权限
            if (!GetAccess(1,"/api/Admin/Article/List"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (pageIndex == 0)
                pageIndex = 1;

            var parm = db.Select
                .WhereIf(ArticleMenuID != null && ArticleMenuID != 0, a => articleMenu_ArticleService.Select.As("s")
                .Where(s => s.ArticleID == a.ID && s.ArticleMenuID == ArticleMenuID).Any())
                .WhereIf(State != null && State != 0, a => a.State == State)
                .WhereIf(IsComplete != null, a => a.IsComplete == IsComplete)
                .WhereIf(IsOvertime!=null && IsOvertime==true, a => a.IsComplete==true &&a.CompleteDate>a.EndDate)
                .WhereIf(IsOvertime != null && IsOvertime == false, a => a.CompleteDate < a.EndDate)
                .WhereIf(!string.IsNullOrWhiteSpace(Title), a => a.Title.Contains(Title));
            // 获取登录用户数据
            int UserID = GetUserID();
            if (UserID > 0)
            {
                UserInfo userInfo = dbuser.Select.Where(s => s.id == UserID).ToOne();
                if (userInfo.RoleId != 1)
                {
                    parm = parm.Where(a => a.DepartmentID == userInfo.deptId);
                }

            }
            var list = db.GetPages(parm, new Common.PageParm(pageIndex,pageSize), "Sorting,ID DESC");
            res.success = true;
            List<ArticleViewModel> list2 = new List<ArticleViewModel>();
            ArticleViewModel vm;
            if (list.DataSource != null)
            {
                foreach (var item in list.DataSource)
                {
                    vm = IMapper.Map<ArticleViewModel>(item);

                    // 查询栏目
                    var menuList = articleMenuDB.Select.Where(a => articleMenu_ArticleService.Select.As("b").Where(b => b.ArticleMenuID == a.ID && b.ArticleID == item.ID).Any()).ToList();
                    if (menuList != null)
                    {
                        List<ArticleMenuModel> vm2 = new List<ArticleMenuModel>();
                        foreach (var item2 in menuList)
                        {
                            vm2.Add(new ArticleMenuModel
                            {
                                Names = item2.Names,
                                ArticleMenuID = item2.ID
                            });
                        }
                        vm.ArticleMenu = vm2;
                    }
                    list2.Add(vm);
                }
                res.data = list2;
                if (res.data != null && res.data.Count() > 0)
                {
                    res.success = true;
                    res.index = pageIndex;
                    res.count = list.TotalCount;
                    res.size = list.PageSize;
                    res.pages = list.TotalPages;
                    res.msg = "成功获取！";
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
        /// 栏目数据，评测项目管理栏目
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetMenuLevel")]
        public async Task<ApiResult<IEnumerable<ELTreeViewModel>>> GetMenuLevel()
        {
            var res = new ApiResult<IEnumerable<ELTreeViewModel>>();
            //判断权限
            if (!GetAccess(1,"/api/Admin/Article/GetMenuLevel"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            List<ELTreeViewModel> MenuView = new List<ELTreeViewModel>();
            var data = articleMenuDB.Select.Where(o => o.State == 2).ToList();
            ELTreeViewModel eLTreeViewModel = new ELTreeViewModel();
            if (data != null)
            {
                // 如果要看iview tree版,这里要改成Tree2ViewModel
                MenuView.Add(new ELTreeViewModel
                {
                    id = 0,
                    label = "栏目",
                    children = APIHelper.GetChildren(data, 0)
                });
                res.data = MenuView;
                if (res.data != null && res.data.Count() > 0)
                {
                    res.success = true;
                    res.msg = "成功获取！";
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
        /// 修改任务分配的列表
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        public ApiResult<string> Update([FromBody] ArticlePostViewModel vm)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<string>() { code = (int)ApiEnum.ParameterError };
            //判断有无权限
            if (!GetAccess(1,"/api/Admin/Article/Update"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return res;
            }
            if (!string.IsNullOrWhiteSpace(vm.Title) && !string.IsNullOrWhiteSpace(vm.ArticleMenuIDs))
            {
                bool IsAdd = false;
                if (vm.ID > 0)
                {
                    Article m = db.Select.Where(a => a.ID == vm.ID).ToOne();
                    if (m != null)
                    {
                        m.DepartmentID = vm.DepartmentID;
                        m.ID = vm.ID;
                        m.Title = vm.Title;
                        m.AddDate = C.DateTimes(vm.AddDate);
                        m.FileUrl = vm.FileUrl;
                        m.Sorting = vm.Sorting;
                        m.State = vm.State;
                        m.Synopsis = vm.Synopsis;
                        m.Contents = vm.Contents;
                        m.FileSize = m.FileSize + "";
                        m.FileFormat = m.FileFormat + "";
                        m.EndDate = C.DateTimes(vm.EndDate);
                        m.CompleteDate = C.DateTimes(vm.CompleteDate);
                        m.IsComplete = vm.IsComplete;
                        m.Month = vm.Month;
                        m.NumMonth = vm.NumMonth;
                        m.Numth = vm.Numth;
                        m.NumYear = vm.NumYear;
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
                                    articleMenu_ArticleService.Delete(a => a.ArticleID == m.ID);
                                }
                                // 再添加
                                string[] array = vm.ArticleMenuIDs.Split(',');
                                if (array.Length > 0)
                                {
                                    List<ArticleMenu_Article> list = new List<ArticleMenu_Article>();
                                    foreach (string item in array)
                                    {
                                        list.Add(new ArticleMenu_Article
                                        {
                                            ArticleID = m.ID,
                                            ArticleMenuID = C.Int(item)
                                        });
                                    }
                                    articleMenu_ArticleService.Insert2(list);
                                    res.msg = "修改成功";
                                }
                            }
                            else
                            {
                                res.msg = "修改失败";
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
        public ApiResult<string> Add([FromBody]ArticlePostViewModel vm)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<string>() { code = (int)ApiEnum.ParameterError };
            //判断有无权限
            if (!GetAccess(1,"/api/Admin/Article/Add"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return res;
            }
            if (!string.IsNullOrWhiteSpace(vm.Title) && !string.IsNullOrWhiteSpace(vm.ArticleMenuIDs))
            {
                Article m = new Article();
                m.DepartmentID = vm.DepartmentID;
                m.Title = vm.Title;
                m.AddDate = C.DateTimes(vm.AddDate);
                m.FileUrl = vm.FileUrl;
                m.Sorting = vm.Sorting;
                m.State = vm.State;
                m.Synopsis = vm.Synopsis;
                m.Contents = vm.Contents;
                m.AddDate = C.DateTimes(m.AddDate);
                m.FileSize = m.FileSize + "";
                m.FileFormat = m.FileFormat + "";
                m.EndDate = C.DateTimes(vm.EndDate);
                m.CompleteDate = C.DateTimes(vm.CompleteDate);
                m.IsComplete = vm.IsComplete;
                m.Month = vm.Month;
                m.NumMonth = vm.NumMonth;
                m.Numth = vm.Numth;
                m.NumYear = vm.NumYear;
                try
                {


                    m.ID = db.Insert3(m);
                    res.success = m.ID > 0;
                    // 处理栏目
                    if (res.success)
                    {
                        string[] array = vm.ArticleMenuIDs.Split(',');
                        if (array.Length > 0)
                        {
                            List<ArticleMenu_Article> list = new List<ArticleMenu_Article>();
                            foreach (string item in array)
                            {
                                list.Add(new ArticleMenu_Article
                                {
                                    ArticleID = m.ID,
                                    ArticleMenuID = C.Int(item)
                                });
                            }
                            if (articleMenu_ArticleService.Insert2(list) > 0)
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
                        res.msg = "文章添加失败";
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
        [HttpDelete("SetState")]
        public async Task<ApiResult<string>> SetState(string ID)
        {
            int State = 3;
            var res = new ApiResult<string>();
            //判断有无权限
            if (!GetAccess(1,"/api/Admin/Article/SetState"))
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
                    if (db.UpdateDiy.Set(a => a.State, State)
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
