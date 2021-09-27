using FreeBlog.IService;
using FreeBlog.Model.Models;
using FreeBlog.Model.ViewModels;
using FreeBlog.Web.Controllers.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeBlog.Web.Controllers
{
    public class MainController : BaseController
    {

        private readonly IRoleService roleDB;
        private readonly IMenuService menuDB;
        public MainController(IMenuService menuDB, IRoleService roleDB)
        {
            this.menuDB = menuDB;
            this.roleDB = roleDB;
        }
        // GET: MainController
        public ActionResult Index()
        {
            //logger.LogWarning("进入首页");
            ViewBag.Title = "新版管理系统";

            // 获取登录用户数据
            UserInfo user = GetUserInfo();
            if (user != null && user.RoleID > 0)
            {
                Role role = roleDB.Select.Where(u=>u.ID==user.RoleID).ToOne();
                if (role != null)
                {
                    // 后台菜单
                    List<MenuViewModel> list = new List<MenuViewModel>();
                    //var menuList = menuDB.QueryList(null, null, true);
                    var menuList = menuDB.Select.Where(a => a.IsEnable).ToList();
                    IEnumerable<Menu> list2 = menuList;
                    if (list2 != null && list2.Count() > 0)
                    {
                        list2 = list2.OrderBy(a => a.Sorting);
                        IEnumerable<Menu> mList1 = list2.Where(a => a.FID == 0);
                        if (mList1 != null && mList1.Count() > 0)
                        {
                            foreach (var item in mList1)
                            {
                                if (role.ID == 1 || role.Menus.Contains("," + item.ID + ","))
                                {
                                    list.Add(new MenuViewModel
                                    {
                                        name = item.Names,
                                        url = item.Url,
                                        icon = item.Icon,
                                        childMenus = GetMenu(item.ID, list2, role),
                                    });
                                }
                            }
                        }
                        // 菜单列表序列化后放到前端执行反序列化后渲染
                        ViewBag.Menu = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                        // 获取昵称
                        ViewBag.NickName = user.NickName;
                    }
                }
            }
            return View();
        }
        public ActionResult Welcome()
        {
            ViewBag.Title = "欢迎页面";
            return View();
        }
        public ActionResult Index2()
        {
            ViewBag.Title = "管理系统";
            // 后台菜单
            List<MenuViewModel> list = new List<MenuViewModel>();
            //var menuList = menuDB.QueryList(null, null, true);
            var parm = menuDB.Select;
            parm.Where(m => m.IsEnable);
            var menuList = menuDB.GetWhere(parm);
            IEnumerable<Menu> list2 = menuList;
            if (list2 != null && list2.Count() > 0)
            {
                list2 = list2.OrderBy(a => a.Sorting);
                IEnumerable<Menu> mList1 = list2.Where(a => a.FID == 0);
                if (mList1 != null && mList1.Count() > 0)
                {
                    foreach (var item in mList1)
                    {
                        list.Add(new MenuViewModel
                        {
                            name = item.Names,
                            url = item.Url,
                            icon = item.Icon,
                            childMenus = GetMenu(item.ID, list2, new Role() { ID = 1 }),
                        });
                    }
                }
                // 菜单列表序列化后放到前端执行反序列化后渲染
                ViewBag.Menu = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            }
            return View();
        }
        /// <summary>
        /// 获取菜单列表的子项
        /// </summary>
        /// <param name="FID"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        public IEnumerable<MenuViewModel> GetMenu(int FID, IEnumerable<Menu> list2, Role role)
        {
            List<MenuViewModel> list = null;
            IEnumerable<Menu> mList1 = list2.Where(a => a.FID == FID);
            if (mList1 != null && mList1.Count() > 0)
            {
                list = new List<MenuViewModel>();
                foreach (var item in mList1)
                {
                    if (role.ID == 1 || role.Menus.Contains("," + item.ID + ","))
                    {
                        list.Add(new MenuViewModel
                        {
                            name = item.Names,
                            url = item.Url,
                            icon = "",
                            childMenus = GetMenu(item.ID, list2, role),
                        });
                    }
                }
            }
            return list;
        }
    }
}
