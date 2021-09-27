using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FreeBlog.Api.ApiGroup;
using FreeBlog.Api.Helper;
using FreeBlog.Common;
using FreeBlog.IService;
using FreeBlog.Model;
using FreeBlog.Model.Models;
using FreeBlog.Model.ViewModels.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Pdf;
using Spire.Xls;

namespace FreeBlog.Api.Controllers.Admin.Info
{
    /// <summary>
    /// 评测项目管理
    /// </summary>
    [Produces("application/json")]
    [Route("api/Admin/[controller]")]
    [Authorize(Policy = "Admin")]
    [ApiGroup(ApiGroupNames.SBook)]
    public class ArticleMenuController : AdminBaseController
    {
        private readonly IArticleMenuService articleMenuDB;
        private readonly IRoleService dbrole;
        private readonly IUserService dbuser;
        private readonly IModuleService dbmo;
        public ArticleMenuController(IArticleMenuService articleMenuDB,IUserService dbuser,IRoleService dbrole,IModuleService dbmo):base(dbrole,dbuser,dbmo)
        {
            this.articleMenuDB = articleMenuDB;
            this.dbrole = dbrole;
            this.dbuser = dbuser;
            this.dbmo = dbmo;
        }
        /// <summary>
        /// 测评项目列表
        /// </summary>
        /// <param name="BeginDate">开始时间</param>
        /// <param name="EndDate">结束时间</param>
        /// <param name="State">状态</param>
        /// <returns></returns>
        [HttpGet("List")]
        public async Task<ApiResult<IEnumerable<ELTreeViewModel>>> List(string BeginDate = null, string EndDate = null, int? State = null)
        {

            // 以接口的形式返回数据
            var res = new ApiResult<IEnumerable<ELTreeViewModel>>();
            //判断接口权限
            if (!GetAccess(1,"/api/ArticleMenu/Article/List"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            List<ELTreeViewModel> list = new List<ELTreeViewModel>();
            var parm = articleMenuDB.Select
                .WhereIf(State != null, m => m.State == State)
                .WhereIf(!string.IsNullOrWhiteSpace(BeginDate) && !string.IsNullOrWhiteSpace(EndDate),
                m => m.AddDate > C.DateTimes(BeginDate) && m.AddDate < C.DateTimes(EndDate));
            var data = articleMenuDB.GetWhere(parm).OrderBy(a => a.Sorting);
            ELTreeViewModel eLTreeViewModel = new ELTreeViewModel();
            if (data != null)
            {
                // 如果要看iview tree版,这里要改成Tree2ViewModel
                list.Add(new ELTreeViewModel
                {
                    label = "根目录",
                    children = APIHelper.GetChildren(data, 0)
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
            //判断接口权限
            if (!GetAccess(1,"/api/ArticleMenu/Article"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (ID > 0)
            {
                ArticleMenu m = articleMenuDB.Select.Where(a => a.ID == ID).ToOne();
                if (m != null)
                {
                    res.success = true;
                    res.data = new
                    {
                        m.ParentID,
                        m.Names,
                        m.ENames,
                        m.Sorting
                    };
                }
                else
                {
                    res.data = new { ParentID = 0, Names = "", ENames = "", Sorting = "" };
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
        /// <param name="ENames">存英文名或副标题</param>
        /// <param name="Sorting">顺序</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<ApiResult<string>> Item(int ParentID, string Names, string ENames, int Sorting)
        {
            var res = new ApiResult<string>();
            //判断接口权限
            if (!GetAccess(1,"/api/ArticleMenu/Add"))
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
            else
            {
                ArticleMenu m = new ArticleMenu();
                ArticleMenu am = articleMenuDB.Select.Where(a => a.ID == ParentID).ToOne();
                if (ParentID == 0 || am != null)
                {
                    m.Names = Names;
                    m.ENames = ENames;
                    m.Sorting = Sorting;
                    m.ParentID = ParentID;
                    m.State = 2;
                    m.AddDate = DateTime.Now;
                    try
                    {
                        res.success = articleMenuDB.Insert3(m) > 0;
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
        /// <param name="ENames">存英文名或副标题</param>
        /// <param name="Sorting">顺序</param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<ApiResult<string>> Item(int ID, int ParentID, string Names, string ENames, int Sorting)
        {
            var res = new ApiResult<string>();
            //判断接口权限
            if (!GetAccess(1,"/api/ArticleMenu/Update"))
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
            else
            {
                try
                {
                    ArticleMenu m = articleMenuDB.Select.Where(a => a.ID == ID).ToOne();
                    ArticleMenu am = articleMenuDB.Select.Where(a => a.ID == ParentID).ToOne();
                    if (ParentID == 0 || am != null)
                    {
                        if (m != null)
                        {
                            m.Names = Names;
                            m.ENames = ENames;
                            m.Sorting = Sorting;
                            res.success = articleMenuDB.UpdateDiy.SetSource(m).ExecuteAffrows() > 0;
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
            //判断接口权限
            if (!GetAccess(1,"/api/ArticleMenu/ModifyParentID"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (ID > 0)
            {
                res.success = articleMenuDB.UpdateDiy.Set(a => a.ParentID, ParentID).Where(a => a.ID == ID).ExecuteAffrows() > 0;
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
            //判断接口权限
            if (!GetAccess(1,"/api/ArticleMenu/Enable"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (!string.IsNullOrWhiteSpace(ID))
            {
                string[] array = ID.Trim(',').Split(',');
                int i = 0;
                int State = 1;
                foreach (string item in array)
                {
                    if (articleMenuDB.UpdateDiy.Set(a => a.State, State).Where(a => a.ID == Convert.ToInt32(item)).ExecuteAffrows() > 0)
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
