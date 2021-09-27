using FreeBlog.Api.ApiGroup;
using FreeBlog.Common;
using FreeBlog.IService;
using FreeBlog.Model.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Spire.Xls;
using System.Linq;
using FreeBlog.Model;
using System.Drawing;
using System.Drawing.Imaging;
using Spire.Pdf;
using Microsoft.AspNetCore.Authorization;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;

namespace FreeBlog.Api.Controllers.Admin.Info
{
    /// <summary>
    /// 台账管理
    /// </summary>
    [Produces("application/json")]
    [Route("api/Admin/[controller]")]
    [ApiGroup(ApiGroupNames.SBook)]

    [Authorize(Policy = "Admin")]
    public class StandingBookController : AdminBaseController
    {
        private readonly IStandingBookService db;
        private readonly IFreeSql freeSql;
        private readonly IArticleMenu_ArticleService articleMenu_Article;
        private readonly IArticleService articleService;
        private readonly IArticleMenuService articleMenuService;
        private readonly IUserService userService;
        private readonly IUserService dbuser;
        private readonly IRoleService dbRole;
        private readonly IModuleService dbmo;
        public StandingBookController(IStandingBookService db, IFreeSql freeSql, IArticleMenuService articleMenuService,
            IArticleMenu_ArticleService articleMenu_Article, IArticleService articleService, IUserService userService
            , IUserService dbuser, IRoleService dbRole,IModuleService dbmo):base(dbRole,dbuser,dbmo)
        {
            this.db = db;
            this.freeSql = freeSql;
            this.articleService = articleService;
            this.articleMenu_Article = articleMenu_Article;
            this.articleMenuService = articleMenuService;
            this.userService = userService;
            this.dbuser = dbuser;
            this.dbRole = dbRole;
            this.dbmo = dbmo;
        }



        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        //[Authorize(Roles = "system")]
        [HttpGet(template: "List2")]
        public async Task<ApiResult<List<StandingBook>>> List2()
        {
            var res = new ApiResult<List<StandingBook>>() { code = (int)ApiEnum.ParameterError };
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/StandingBook/List2"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (GetUserID()==1)
            {
                res.success = true;
                res.code = (int)ApiEnum.Status;
                res.data = db.Select.Where(a => true).ToList();
            }
            else
            {
                int Did = dbuser.Select.Where(a => a.id == GetUserID()).ToOne().deptId;
                res.success = true;
                res.code = (int)ApiEnum.Status;
                res.data = db.Select.Where(a => a.DepartmentID== Did).ToList();
            }
            return await Task.Run(() => res);
        }

        /// <summary>
        /// /列表显示待分页
        /// </summary>
        /// <param name="pageIndex">第几页</param>
        /// <param name="title">标题</param>
        /// <returns></returns>
        [HttpGet("List")]
        public async Task<ApiResult<IEnumerable<StandingBook>>> List(int pageIndex, string title)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<IEnumerable<StandingBook>>() { code = (int)ApiEnum.Status };
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/StandingBook/List"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (pageIndex == 0) pageIndex = 1;
            //DBPage dp = new DBPage(pageIndex, 15);
            PagedInfo<StandingBook> data = null;
            if (GetUserID() == 1)
            {
                 data = db.GetPages(db.Select.WhereIf(!string.IsNullOrEmpty(title), m => m.Names.Contains(title)), new PageParm(pageIndex), "Sorting,ID DESC");
            }
            else
            {
                int Did = dbuser.Select.Where(a => a.id == GetUserID()).ToOne().deptId;
                 data = db.GetPages(db.Select.WhereIf(!string.IsNullOrEmpty(title), m => m.Names.Contains(title)).Where(a => a.DepartmentID == Did), new PageParm(pageIndex), "Sorting,ID DESC");
            }
            res.success = true;
            res.data = data.DataSource;
            res.index = pageIndex;
            res.count = data.TotalCount;
            res.size = data.PageSize;
            return await Task.Run(() => res);
        }
        /// <summary>
        /// /添加
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        [HttpPost("add")]
        //[ApiExplorerSettings(IgnoreApi = true)] //隐藏接口
        public async Task<ApiResult> Add(StandingBook w)
        {
            // 以接口的形式返回数据
            var res = new ApiResult { code = (int)ApiEnum.ParameterError };
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/StandingBook/add"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (string.IsNullOrWhiteSpace(w.Names))
            {
                res.msg = "请填写标题";
            }
            else
            {
                try
                {
                    StandingBook m = new StandingBook();
                    m.FID = w.FID;
                    m.Names = w.Names;
                    m.Content = w.Content;
                    m.Sorting = w.Sorting;
                    m.AddDate = w.AddDate;
                    m.DepartmentID = w.DepartmentID;
                    //判断是否为超级管理员
                    if (GetUserID() == 1)
                    {
                        m.DepartmentID = w.DepartmentID;
                    }
                    else
                    {
                        int Did = dbuser.Select.Where(a => a.id == GetUserID()).ToOne().deptId;
                        m.DepartmentID = Did;
                    }
                    if (w.file != null)
                    {
                        //如有图片上传则保存到本地
                        if (w.file.Contains("base64"))
                        {
                            string path = "UploadFiles/advert/";
                            string path2 = Utility.HostAddress + "advert\\";
                            m.file = ImagesUtility.Base64StringToFile(w.file, path2, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                            if (m.file != "")
                                m.file = path + m.file;
                        }
                    }
                    res.success = db.Insert(m).ID > 0;
                    if (res.success)
                    {
                        res.code = (int)ApiEnum.Status;
                        res.msg = "成功获取！";
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
        /// 删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpDelete("{ID}")]
        public ApiResult Delete(int ID)
        {
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/StandingBook"))
            {
                return new ApiResult { code = (int)ApiEnum.Unauthorized, msg = ApiEnum.Unauthorized.GetEnumText()};
            }
            if (ID > 0)
                return new ApiResult { code = (int)ApiEnum.Status, success = freeSql.Delete<StandingBookController>(ID).ExecuteAffrows() > 0 ,msg="删除成功"};
            return new ApiResult { code = (int)ApiEnum.ParameterError, msg = "数据丢失" };
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        //[ApiExplorerSettings(IgnoreApi = true)] //隐藏接口
        //public async Task<ApiResult<string>> Update(Role m)
        public async Task<ApiResult> Update(StandingBook m)
        {
            // 以接口的形式返回数据
            var res = new ApiResult();
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/StandingBook/Update"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (string.IsNullOrWhiteSpace(m.Names))
            {
                res.msg = "请填写标题";
                res.code = (int)ApiEnum.ParameterError;
            }
            else
            {
                try
                {
                    // 获取登录用户数据
                    int UserID = GetUserID();
                    if (UserID > 0)
                    {
                        UserInfo userInfo = userService.Select.Where(s => s.id == UserID).ToOne();
                        if (userInfo.RoleId != 1)
                        {
                            m.DepartmentID = userInfo.deptId;
                        }

                    }
                    res.success = freeSql.GetRepository<StandingBook>().Update(m) > 0;
                    if (res.success)
                    {
                        res.msg = "修改成功";
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
        /// 部分编辑
        /// </summary>
        /// <param name="ID">ID</param>
        /// <param name="Names">名称</param>
        /// <param name="Sorting">顺序</param>
        /// <returns></returns>
        [HttpPatch("SomeUpdate")]
        //[ApiExplorerSettings(IgnoreApi = true)] //隐藏接口
        //public async Task<ApiResult<string>> Update(Role m)
        public async Task<ApiResult<string>> Update(int ID, string Names, int Sorting)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<string>() { code = (int)ApiEnum.ParameterError };
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/StandingBook/SomeUpdata"))
            {
                res.code = (int)ApiEnum.Unauthorized;
                res.msg = ApiEnum.Unauthorized.GetEnumText();
                return await Task.Run(() => res);
            }
            if (string.IsNullOrWhiteSpace(Names))
            {
                res.msg = "请填写标题";
            }
            else
            {
                try
                {
                    StandingBook m = db.Select.Where(s => s.ID == ID).First();
                    if (m != null)
                    {
                        m.Names = Names;
                        m.Sorting = Sorting;
                        res.success = db.UpdateDiy.SetSource(m).ExecuteAffrows() > 0;
                        if (res.success)
                        { res.code = (int)ApiEnum.Status;
                            res.msg = "编辑成功";
                        }
                    }
                    else
                        res.msg = "查询失败";
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
        /// 上传附件，返回的response是上传图片的相对连接
        /// </summary>
        /// <returns></returns>
        [HttpPut("File")]
        public async Task<MessageModel<string>> InsertPicture([FromServices] IWebHostEnvironment environment, IFormFileCollection fileif)
        {
 
            var data = new MessageModel<string>();
            //判断接口权限
            if (!GetAccess(1, "/api/Admin/StandingBook/File"))
            {
                data.msg = ApiEnum.Unauthorized.GetEnumText();
                data.status=(int)ApiEnum.Unauthorized;
                return data;
            }
            string path = string.Empty;
            string foldername = "article";
            IFormFileCollection files = null;
            // 获取提交的文件
            //files = Request.Form.Files;
            files = fileif;
            // 获取附带的数据
            var max_ver = Request.Form["max_ver"].ObjToString();
            if (files == null || !files.Any()) { data.msg = "请选择上传的文件。"; return data; }
            //格式限制
            var allowType = new string[] { "image/jpg", "image/png", "image/jpeg", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/pdf" };

            string folderpath = Path.Combine(environment.WebRootPath, foldername);
            //创建文件夹
            if (!Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath);
            }
            if (files.Any(c => allowType.Contains(c.ContentType)))
            {
                //判断文件大小
                if (files.Sum(c => c.Length) <= 1024 * 1024 * 4)
                {
                    List<string> strpathlist = new List<string>();

                    foreach (var item in files)
                    {
                        string strpath = Path.Combine(foldername, DateTime.Now.ToString("MMddHHmmss") + Path.GetFileName(item.FileName));
                        strpathlist.Add(strpath);
                        path = Path.Combine(environment.WebRootPath, strpath);

                        using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            await item.CopyToAsync(stream);
                        }
                        //判断是否为EXCEL
                        if (item.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                        {
                            Workbook book = new Workbook();
                            book.LoadFromFile(path);
                            Worksheet sheet = book.Worksheets[0];

                            sheet.SaveToImage(Path.Combine(environment.WebRootPath, strpath.Split('.')[0] + ".jpg"));
                        }
                        //判断是否pdfC文档
                        else if (item.ContentType == "application/pdf")
                        {
                            //初始化PdfDocument实例
                            PdfDocument doc = new PdfDocument();

                            //加载PDF文档
                            doc.LoadFromFile(path);

                            //遍历PDF每一页
                            for (int i = 0; i < doc.Pages.Count; i++)
                            {
                                //将PDF页转换成bitmap图形
                                Image bmp = doc.SaveAsImage(i);
                                //将bitmap图形保存为png图片
                                string fileName = string.Format("{0}Page-{1}.png", item.FileName.Split('.')[0], i + 1);
                                string strpathfile = Path.Combine(foldername, DateTime.Now.ToString("MMddHHmmss") + Path.GetFileName(fileName));
                                bmp.Save(Path.Combine(environment.WebRootPath, strpathfile), ImageFormat.Png);
                            }
                        }
                    }
                    data = new MessageModel<string>()
                    {
                        response = string.Join(",", strpathlist.ToArray()),
                        msg = "上传成功",
                        success = true,
                    };
                    return data;
                }
                else
                {
                    data.msg = "文件过大";
                    return data;
                }
            }
            else
            {
                data.msg = "文件格式错误";
                return data;

            }

        }



        /// <summary>
        /// a标签页面跳转导出docx,根据部门判断下载内容
        /// </summary>
        /// <param name="FID">评测项目ID</param>
        /// <param name="environment"></param>
        /// <returns></returns>
        [HttpGet("Export")]
        public IActionResult Export( int FID, [FromServices] IWebHostEnvironment environment)
        {
            if (!GetAccess(1, "/api/Admin/StandingBook/Export"))
            {
                return null;
            }
            var SandList = new List<StandingBook>();
            // 获取登录用户数据
            int UserID = GetUserID();
            if (UserID > 0)
            {
                UserInfo userInfo = userService.Select.Where(s => s.id == UserID).ToOne();
                if (userInfo.RoleId == 1)
                {
                    SandList = db.Select.Where(s => s.FID == FID).ToList();
                }
                else
                {
                    SandList = db.Select.Where(s => s.FID == FID && s.DepartmentID == userInfo.deptId).ToList();
                }
            }
            //文件是生成在内存中
            string fileName = $"{Guid.NewGuid().ToString()}.doc";
            //store in memory rather than pysical directory
            var stream = new MemoryStream();
            //创建文档
            Document doc = new Document();
            //添加section
            Section s = doc.AddSection();
            //使用域代码自定义目录
            TableOfContent toc = new TableOfContent(doc, "{\\o \"1-3\" \\h \\z \\u}");
            Paragraph p = s.AddParagraph();
            p.Items.Add(toc);
            p.AppendFieldMark(FieldMarkType.FieldSeparator);
            p.AppendText("目录");
            p.AppendFieldMark(FieldMarkType.FieldEnd);
            doc.TOC = toc;
            //读取内容
            //var SandList = db.Select.Where(s => s.FID == FID).ToList();
            //添加目录域
            Paragraph paragraph = s.AddParagraph();
            //设置段落中行距
            paragraph.Format.LineSpacing = 28.5f;
            paragraph.AppendTOC(1, SandList.Count);
            //设置BreakType为分页
            paragraph.AppendBreak(BreakType.PageBreak);
            var ArticleS = articleService.Select.Where(a => a.ID == FID).ToOne();
            Paragraph paras = s.AddParagraph();
            TextRange textRange = paras.AppendText(ArticleS.Title);
            textRange.CharacterFormat.FontNameFarEast = "华西宋体";
            textRange.CharacterFormat.FontSize = 22;
            paras.ApplyStyle(BuiltinStyle.Heading1);
            paras.Format.LineSpacing = 28.5f;
            //添加文本到paragraph，设置BreakType为分页
            paras.AppendBreak(BreakType.PageBreak);
            foreach (var item in SandList)
            {
                Paragraph para2s = s.AddParagraph();
                TextRange textRange2 = para2s.AppendText(item.Names);
                textRange2.CharacterFormat.FontNameFarEast = "华西宋体";
                textRange2.CharacterFormat.FontSize = 22;
                textRange2.CharacterFormat.Bold = false;
                textRange2.CharacterFormat.Italic = false;
                para2s.ApplyStyle(BuiltinStyle.Heading2);
                TextRange tr2 = para2s.AppendText(null);
                para2s.Format.LineSpacing = 28.5f;
                Paragraph para3s = s.AddParagraph();
                TextRange textRange3 = para3s.AppendText(item.Content);
                textRange3.CharacterFormat.FontNameFarEast = "华西宋体";
                textRange3.CharacterFormat.FontSize = 16;
                para3s.Format.LineSpacing = 28.5f;
                try
                {
                    if (item.file != null)
                    {
                        Image image = Image.FromFile(Path.Combine(environment.WebRootPath, item.file));
                        DocPicture picture = para3s.AppendPicture(image);
                    }
                }
                catch (Exception ex)
                {
                    
                }
               
                para3s.AppendBreak(BreakType.PageBreak);
            }
            //更新目录域
            doc.UpdateTableOfContents();

            //设置页脚
            HeaderFooter footer = s.HeadersFooters.Footer;
            Paragraph footerPara = footer.AddParagraph();
            //添加字段类型为页码，添加当前页、分隔线以及总页数
            footerPara.AppendField("页码", FieldType.FieldPage);
            footerPara.AppendText(" / ");
            footerPara.AppendField("总页数", FieldType.FieldNumPages);
            footerPara.Format.HorizontalAlignment = HorizontalAlignment.Center;
            doc.SaveToStream(stream, Spire.Doc.FileFormat.Docx);
            stream.Position = 0;
            return File(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
        }


    }
}
