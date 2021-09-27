using FreeBlog.IService;
using FreeBlog.Model.Models;
using FreeBlog.Service.Base;
using System;

namespace FreeBlog.Service
{
    public class MenuService : BaseRepository<Menu>, IMenuService
    {
        public MenuService(IFreeSql fsql) : base(fsql)
        {
        }


    }
}
