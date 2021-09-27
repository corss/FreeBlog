using FreeBlog.Common;
using FreeBlog.Common.Security;
using FreeBlog.IService;
using FreeBlog.Model.Models;
using FreeBlog.Model.ViewModels;
using FreeBlog.Web.Controllers.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FreeBlog.Web.Controllers
{
    public class UserController : BaseController
    {

        private readonly IUserService db;
        private readonly IRoleService roleDb;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UserController(IUserService db, IRoleService roleDb, IWebHostEnvironment _webHostEnvironment)
        {
            this.db = db;
            this.roleDb = roleDb;
            this._webHostEnvironment = _webHostEnvironment;
        }
        /// <summary>
        /// 系统管理员
        /// </summary>
        /// <returns></returns>
        public IActionResult SysIndex()
        {
            ViewBag.Title = "管理员";
            // 角色赋值
            ViewBag.RoleList = JsonConvert.SerializeObject(GetRoleViewModel(0, "全部角色"));
            return View();
        }
        // 获取角色
        public List<BaseViewModel> GetRoleViewModel(int? ID = null, string Title = null)
        {
            var roleList = roleDb.Select.ToList();
            List<BaseViewModel> roles = new List<BaseViewModel>();
            if (ID != null && Title != null)
            {
                roles.Add(new BaseViewModel(ID.Value, Title));
            }
            if (roleList != null)
            {
                foreach (var item in roleList)
                {
                    roles.Add(new BaseViewModel(item.ID, item.Names));
                }
            }
            return roles;
        }

        /// <summary>
        /// 列表显示
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult<IEnumerable<UserViewModel>>> List(int pageIndex, int RoleID, int StateID, string Title, string Phone)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<IEnumerable<UserViewModel>>() { statusCode = (int)ApiEnum.Status };
            if (pageIndex == 0) pageIndex = 1;
            List<UserViewModel> list = new List<UserViewModel>();
            var parm = db.Select
                .WhereIf(!string.IsNullOrEmpty(Title), m => m.FullName.Contains(Title) || m.NickName.Contains(Title))
                .WhereIf(RoleID != 0, m => m.RoleID == RoleID)
                .WhereIf(StateID != 0, m => m.State == StateID)
                .WhereIf(!string.IsNullOrEmpty(Phone), m => m.MobilePhone.Contains(Phone));
            var Paged = db.GetPages(parm, new PageParm(pageIndex), "ID DESC");
            var data = Paged.DataSource;
            List<BaseViewModel> baseView = GetRoleViewModel(0, "0");
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(new UserViewModel
                    {
                        ID = item.ID,
                        UserName = item.UserName,
                        Password = item.Password,
                        HeadPortrait = item.HeadPortrait,
                        NickName = item.NickName,
                        FullName = item.FullName,
                        Position = item.Position,
                        IdCard = item.IdCard,
                        Gender = item.Gender,
                        MobilePhone = item.MobilePhone,
                        QQ = item.QQ,
                        Mail = item.Mail,
                        State = item.State,
                        AddDate = Utility.GetDateFormat(item.AddDate),
                        RoleName = baseView.Find(a => a.ID == item.RoleID).Title
                    });
                }
            }

            res.success = true;
            res.data = list;
            res.index = pageIndex;
            res.count = Paged.TotalCount;
            res.size = Paged.PageSize;
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public async Task<ApiResult<string>> Delete(string ID)
        {
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.Status };
            if (!string.IsNullOrWhiteSpace(ID))
            {
                string[] array = ID.Trim(',').Split(',');
                if (array != null && array.Length > 0)
                {
                    int[] array2 = Array.ConvertAll(array, int.Parse);
                    foreach (int item in array2)
                    {
                        //if (db.ModifyState(item, 4))
                        if (db.UpdateDiy.Set(a=>a.State,4).Where(a => a.ID == item).ExecuteAffrows() > 0)
                            res.count++;
                    }
                    res.success = res.count > 0;
                }
            }
            return await Task.Run(() => res);
        }
        /// <summary>
        /// 详情显示
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult SysItem(int id)
        {
            UserInfo m;
            UserViewModel d;
            if (id > 0)
            {
                d = new UserViewModel();
                m = db.Select.Where(u=>u.ID==id).ToOne();
                if (m != null)
                {
                    d.ID = m.ID;
                    d.UserName = m.UserName;
                    d.Password = m.Password;
                    d.HeadPortrait = m.HeadPortrait;
                    d.NickName = m.NickName;
                    d.FullName = m.FullName;
                    d.Position = m.Position;
                    d.IdCard = Utility.IdCardUnEncrypt(m.IdCard);
                    d.Gender = m.Gender;
                    d.MobilePhone = m.MobilePhone;
                    d.QQ = m.QQ;
                    d.Mail = m.Mail;
                    d.State = m.State;
                    d.RoleID = m.RoleID;
                }
                ViewBag.Title = "修改";
            }
            else
            {
                d = new UserViewModel();
                // 设置默认值用于页面显示
                d.RoleID = 0;
                d.State = 2;
                ViewBag.Title = "添加";
            }
            // 角色赋值
            ViewBag.RoleList = new SelectList(GetRoleViewModel(0, "--请选择--"), "ID", "Title");
            return View(d);
        }
        /// <summary>
        /// 添加编辑同个方法
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult<string>> SysItem(UserViewModel vm)
        {
            // 以接口的形式返回数据
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.ParameterError };
            if (!string.IsNullOrWhiteSpace(vm.UserName))
            {
                try
                {
                    UserInfo m = null;
                    if (vm.ID > 0)
                        m = db.Select.Where(v=>v.ID==vm.ID).ToOne();
                    if (m == null)
                        m = new UserInfo();

                    m.ID = vm.ID;
                    m.UserName = vm.UserName;
                    //m.HeadPortrait = vm.HeadPortrait;
                    m.NickName = vm.NickName;
                    m.FullName = vm.FullName;
                    m.Position = vm.Position;
                    m.IdCard = Utility.IdCardEncrypt(vm.IdCard);
                    m.Gender = vm.Gender;
                    m.MobilePhone = vm.MobilePhone;
                    m.QQ = vm.QQ;
                    m.Mail = vm.Mail;
                    m.State = vm.State;
                    m.RoleID = vm.RoleID;

                    // 设置默认密码
                    if (vm.ID == 0 && string.IsNullOrWhiteSpace(vm.Password))
                        vm.Password = "888888";
                    // 如果设置了密码、就进行加密
                    if (!string.IsNullOrWhiteSpace(vm.Password))
                        m.Password = MD5Encode.GetEncrypt(vm.Password);

                    // 提交
                    if (vm.ID == 0)
                    {
                        m.HeadPortrait = "";
                        m.PiUserID = ""; ;
                        m.LastLoginData = m.AddDate = DateTime.Now;
                        res.success = db.Insert3(m) > 0;
                    }
                    else
                    {
                        res.success = db.UpdateDiy.SetSource(m).ExecuteAffrows() > 0;
                    }
                    if (res.success)
                        res.statusCode = (int)ApiEnum.Status;
                }
                catch (Exception ex)
                {
                    res.statusCode = (int)ApiEnum.Error;
                    res.msg = ApiEnum.Error.GetEnumText() + ex.Message;
                }
            }
            else
                res.msg = "参数丢失";
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public IActionResult ModifyPassword()
        {
            ViewBag.Title = "修改密码";
            return View();
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult<string>> ModifyPassword(string oldPassword, string newPassword)
        {
            var res = new ApiResult<string>() { statusCode = (int)ApiEnum.ParameterError };
            //获取当前用户

          
            var user = db.Select.Where(a=>a.ID == C.Int(AESEncode.CBCDecrypt(GetCookieValue(Utility.USER_COOKIE_KEY), Utility.KEYVAL, Utility.IVVAL))).ToOne();
            //判定原密码是否正确
            if (user.Password == MD5Encode.GetEncrypt(oldPassword))
            {
                //将新密码提交数据库，并清除用户状态
                user.Password = MD5Encode.GetEncrypt(newPassword);
                if (res.success = db.UpdateDiy.SetSource(user).ExecuteAffrows() > 0)
                {
                    res.msg = "修改成功,即将跳转至登录页面";
                    UserClear();
                }
                else
                {
                    res.success = false;
                    res.msg = "修改失败,请重新尝试";
                }
            }
            else
            {
                res.success = false;
                res.msg = "原密码错误";
            }
            return await Task.Run(() => res);
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="list1"></param>
        /// <returns></returns>
        public IActionResult Export(int RoleID, int StateID, string Title, string Phone)
        {
            // 方法1：会在本地生成文件
            //List<SysUserViewModel> list = new List<SysUserViewModel>();
            //var parm = Expressionable.Create<UserInfo>()
            //    .AndIF(!string.IsNullOrEmpty(Title), m => m.FullName.Contains(Title) || m.NickName.Contains(Title))
            //    .AndIF(RoleID != 0, m => m.RoleID == RoleID)
            //    .AndIF(StateID != 0, m => m.State == StateID)
            //    .AndIF(!string.IsNullOrEmpty(Phone), m => m.MobilePhone.Contains(Phone));
            //List<UserInfo> list1 = db.GetWhere(parm.ToExpression());
            //if (list1 != null && list1.Count > 0)
            //{
            //    string sWebRootFolder = _hostingEnvironment.WebRootPath + "\\UploadFiles\\excel\\export";
            //    string sFileName = $"{Guid.NewGuid()}.xlsx";
            //    FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            //    using (ExcelPackage package = new ExcelPackage(file))
            //    {
            //        // 添加worksheet
            //        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");
            //        //添加头
            //        worksheet.Cells[1, 1].Value = "编号";
            //        worksheet.Cells[1, 2].Value = "用户名";
            //        worksheet.Cells[1, 3].Value = "昵称";
            //        worksheet.Cells[1, 4].Value = "姓名";
            //        worksheet.Cells[1, 5].Value = "手机";
            //        //添加值
            //        int a = 2;
            //        foreach (var item in list1)
            //        {
            //            worksheet.Cells["A" + a].Value = a - 1;
            //            worksheet.Cells["B" + a].Value = item.UserName;
            //            worksheet.Cells["C" + a].Value = item.NickName;
            //            worksheet.Cells["D" + a].Value = item.FullName;
            //            worksheet.Cells["E" + a].Value = item.MobilePhone;
            //            a++;
            //        }
            //        package.Save();
            //    }
            //    return File("/UploadFiles/excel/export/" + sFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            //}
            //return null;

            // 方法2：文件是生成在内存中，
            string fileName = $"{Guid.NewGuid().ToString()}.xlsx";
            //store in memory rather than pysical directory
            var stream = new MemoryStream();
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                // add worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");
                // 表头
                worksheet.Cells[1, 1].Value = "用户名";
                worksheet.Cells[1, 2].Value = "昵称";
                worksheet.Cells[1, 3].Value = "姓名";
                worksheet.Cells[1, 4].Value = "手机";

                // 获取数据
                List<UserViewModel> list = new List<UserViewModel>();
                var parm = db.Select
                    .WhereIf(RoleID != 0, m => m.RoleID == RoleID)
                    .WhereIf(StateID != 0, m => m.State == StateID)
                    .WhereIf(!string.IsNullOrEmpty(Title), m => m.FullName.Contains(Title) || m.NickName.Contains(Title))
                    .WhereIf(!string.IsNullOrEmpty(Phone), m => m.MobilePhone.Contains(Phone));
                List<UserInfo> list1 = db.GetWhere(parm);
                if (list1 != null && list1.Count > 0)
                {
                    int a = 2;
                    foreach (var item in list1)
                    {
                        worksheet.Cells["A" + a].Value = item.UserName;
                        worksheet.Cells["B" + a].Value = item.NickName;
                        worksheet.Cells["C" + a].Value = item.FullName;
                        worksheet.Cells["D" + a].Value = item.MobilePhone;
                        a++;
                    }
                }
                package.Save();
            }
            stream.Position = 0;
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ApiResult<IEnumerable<UserViewModel>> Import(IFormFile excelfile)
        {
            var res = new ApiResult<IEnumerable<UserViewModel>>() { statusCode = (int)ApiEnum.Status };
            if (excelfile != null)
            {
                List<UserInfo> userInfos = new List<UserInfo>();    // 更新的列表
                List<UserInfo> userInfos2 = new List<UserInfo>();   // 添加的列表
                UserInfo sysUserView;
                string sWebRootFolder = _webHostEnvironment.WebRootPath + "\\UploadFiles\\excel\\import";
                string sFileName = $"{Guid.NewGuid()}.xlsx";
                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                try
                {
                    using (FileStream fs = new FileStream(file.ToString(), FileMode.Create))
                    {
                        excelfile.CopyTo(fs);
                        fs.Flush();
                    }
                    using (ExcelPackage package = new ExcelPackage(file))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                        int rowCount = worksheet.Dimension.Rows;
                        int ColCount = worksheet.Dimension.Columns;
                        DateTime now = DateTime.Now;
                        string password = MD5Encode.GetEncrypt("888888");
                        for (int row = 2; row <= rowCount; row++)
                        {
                            // 查询账号是否已经存在 - 如果单次数据超过100条建议把所有用户取出来对比
                            sysUserView = db.Select.Where(o => o.UserName == C.String(worksheet.Cells[row, 2].Value)).First();
                            if (sysUserView != null)
                            {
                                userInfos.Add(new UserInfo
                                {
                                    UserName = C.String(worksheet.Cells[row, 1].Value),
                                    NickName = C.String(worksheet.Cells[row, 2].Value),
                                    FullName = C.String(worksheet.Cells[row, 3].Value),
                                    MobilePhone = C.String(worksheet.Cells[row, 4].Value),
                                });
                            }
                            else
                            {
                                userInfos2.Add(new UserInfo
                                {
                                    UserName = C.String(worksheet.Cells[row, 1].Value),
                                    NickName = C.String(worksheet.Cells[row, 2].Value),
                                    FullName = C.String(worksheet.Cells[row, 3].Value),
                                    MobilePhone = C.String(worksheet.Cells[row, 4].Value),
                                    RoleID = 4,    // 用户
                                    State = 2,     // 已审
                                    AddDate = now,
                                    Password = password,
                                });
                            }
                        }
                    }
                    // 新增或更新
                    int addCount = 0, updateCount = 0;
                    if (userInfos.Count > 0)
                    {
                        addCount = db.UpdateDiy.SetSource(userInfos).ExecuteAffrows();
                        res.msg += $"导入{addCount}条，";
                    }
                    if (userInfos2.Count > 0)
                    {
                        updateCount = db.Insert2(userInfos2);
                        res.msg += $"更新{addCount}条，";
                    }
                    // 结果
                    res.success = addCount > 0 || updateCount > 0;
                    if (res.msg != null)
                        res.msg = res.msg.TrimEnd('，');
                }
                catch (Exception ex)
                {
                    res.success = false;
                    res.msg = ApiEnum.Error.GetEnumText() + ex.Message;
                }
            }
            return res;
        }
        /// <summary>
        /// 是否是EXCEL文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private bool isExcelFile(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] b = new byte[4];
            string temstr = "";
            //将文件流读取的文件写入到字节数组
            if (Convert.ToInt32(fs.Length) > 0)
            {
                fs.Read(b, 0, 4);

                fs.Close();

                for (int i = 0; i < b.Length; i++)
                {
                    temstr += Convert.ToString(b[i], 16);
                }
            }

            if (temstr.ToUpper() == "D0CF11E0")
            { return true; }
            else
            { return false; }
        }

        public IActionResult UserImport()
        {
            return View();
        }
    }
}
