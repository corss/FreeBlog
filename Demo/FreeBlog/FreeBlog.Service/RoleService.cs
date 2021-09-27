using FreeBlog.IService;
using FreeBlog.Model.Models;
using FreeBlog.Service.Base;
using System;

namespace FreeBlog.Service
{
    public class RoleService : BaseRepository<Role>, IRoleService
    {
        public RoleService(IFreeSql fsql) : base(fsql)
        {
        }


    }
}
