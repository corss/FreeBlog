
using FreeBlog.Common;
using FreeBlog.Model.Models;
using FreeBlog.Model.Models.PoliceModel;
using FreeBlog.Model.ViewModels;
using FreeBlog.Model.ViewModels.Api;
using System.Collections.Generic;
using System.Linq;

namespace FreeBlog.Api.Helper
{
    public static class APIHelper
    {
        // 循环绑定子级
        public static List<ELTreeViewModel> GetChildren(IEnumerable<ArticleMenu> data, int ID)
        {
            var data2 = data.Where(a => a.ParentID == ID);
            if (data2 != null && data2.Count() > 0)
            {
                List<ELTreeViewModel> list = new List<ELTreeViewModel>();
                foreach (var item in data2)
                {
                    list.Add(new ELTreeViewModel
                    {
                        id = item.ID,
                        label = item.Names,
                        children = GetChildren(data, item.ID)
                    });
                }
                return list;
            }
            return null;
        }
        // 循环绑定子级
        public static List<ELTreeViewModel> GetChildrenP(IEnumerable<PoliceprojectMenu> data, int ID)
        {
            var data2 = data.Where(a => a.ParentID == ID);
            if (data2 != null && data2.Count() > 0)
            {
                List<ELTreeViewModel> list = new List<ELTreeViewModel>();
                foreach (var item in data2)
                {
                    list.Add(new ELTreeViewModel
                    {
                        id = item.ID,
                        label = item.Names,
                        children = GetChildrenP(data, item.ID)
                    });
                }
                return list;
            }
            return null;
        }
        /// <summary>
        /// 获取菜单列表的子项
        /// </summary>
        /// <param name="FID"></param>
        /// <param name="list2"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static IEnumerable<MenuViewModel> GetMenu(int FID, IEnumerable<Menu> list2, Role role)
        {
            List<MenuViewModel> list = null;
            IEnumerable<Menu> mList1 = list2.Where(a => a.parentId == FID);
            if (mList1 != null && mList1.Count() > 0)
            {
                list = new List<MenuViewModel>();
                foreach (var item in mList1)
                {
                    if (role.id == 1 || role.Menus.Contains("," + item.id + ","))
                    {
                        var childMenus = GetMenu(item.id, list2, role);
                        List<string> childMenusnull = new List<string>();
                        if (GetMenu(item.id, list2, role) == null)
                        {
                            list.Add(new MenuViewModel
                            {
                                id = item.id,
                                parentId = item.parentId,
                                label = item.label,
                                parentName = item.parentName,
                                code = item.code,
                                path = item.path,
                                name = item.name,
                                url = item.url,
                                orderNum = item.orderNum,
                                type = item.type,
                                icon = item.icon,
                                remark = item.remark,
                                createTime = item.createTime,
                                updateTime = item.updateTime,
                                children = childMenusnull,
                                open = item.open
                            });
                        }
                        else
                        {
                            list.Add(new MenuViewModel
                            {
                                id = item.id,
                                parentId = item.parentId,
                                label = item.label,
                                parentName = item.parentName,
                                code = item.code,
                                path = item.path,
                                name = item.name,
                                url = item.url,
                                orderNum = item.orderNum,
                                type = item.type,
                                icon = item.icon,
                                remark = item.remark,
                                createTime = item.createTime,
                                updateTime = item.updateTime,
                                children = GetMenu(item.id, list2, role),
                                open = item.open
                            });
                        }
                       
                    }
                }
            }
            return list;
        }


        /// <summary>
        /// 获取左菜单子集
        /// </summary>
        /// <param name="FID"></param>
        /// <param name="list2"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static IEnumerable<object> GetMenuList(int FID, IEnumerable<Menu> list2, Role role, bool trueMeun)
        {
            List<object> list = null;
            IEnumerable<Menu> mList1 = list2.Where(a => a.parentId == FID);
            if (mList1 != null && mList1.Count() > 0)
            {
                list = new List<object>();
                foreach (var item in mList1)
                {
                    if (role.id == 1 || role.Menus.Contains("," + item.id + ","))
                    {
                        meta listmeta = new meta();
                        listmeta.title = item.label;
                        listmeta.icon = item.icon;
                        listmeta.roles = item.code.Split(',').ToList();
                        if (item.parentId == 0 && trueMeun)
                        {
                            item.url = "Layout";
                        }
                        if (GetMenuList(item.id, list2, role, trueMeun)==null)
                        {
                            list.Add(new MenuListModelNull
                            {
                                path = item.path,
                                component = item.url,
                                alwaysShow = false,
                                name = item.name,
                                meta = listmeta,
                            });
                        }
                        else
                        {
                            list.Add(new MenuListModel
                            {
                                path = item.path,
                                component = item.url,
                                alwaysShow = item.open,
                                name = item.name,
                                meta = listmeta,
                                children = GetMenuList(item.id, list2, role, trueMeun),
                            });
                        }
                      
                    }

                    
                }
            }
            return list;
        }
        /// <summary>
        /// 获取部门列表的子项
        /// </summary>
        /// <param name="FID"></param>
        /// <param name="list2"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static IEnumerable<object> GetDepartment(int FID, IEnumerable<Department> list2)
        {
            List<object> list = null;
            IEnumerable<Department> mList1 = list2.Where(a => a.pid == FID);
            if (mList1 != null && mList1.Count() > 0)
            {
                list = new List<object>();
                foreach (var item in mList1)
                {
                    if (GetDepartment(item.id, list2) == null)
                    {
                        list.Add(new DepartmentViewModelNull
                        {
                            id = item.id,
                            pid = item.pid,
                            likeId = item.likeId,
                            parentName = item.parentName,
                            manager = item.manager,
                            name = item.name,
                            deptAddress = item.deptAddress,
                            deptCode = item.deptCode,
                            deptPhone = item.deptPhone,
                            orderNum = item.orderNum,
                            open = item.open
                        });
                    }
                    else
                    {
                        list.Add(new DepartmentViewModel
                        {
                            id = item.id,
                            pid = item.pid,
                            likeId = item.likeId,
                            parentName = item.parentName,
                            manager = item.manager,
                            name = item.name,
                            deptAddress = item.deptAddress,
                            deptCode = item.deptCode,
                            deptPhone = item.deptPhone,
                            orderNum = item.orderNum,
                            open = item.open,
                            childMenus = GetDepartment(item.id, list2)
                        });
                    }
                    

                }
            }
            return list;
        }
    }
}
