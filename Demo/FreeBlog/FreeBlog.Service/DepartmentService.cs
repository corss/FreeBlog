using FreeBlog.IService;
using FreeBlog.Model.Models;
using FreeBlog.Service.Base;
using System;

namespace FreeBlog.Service
{
    public class DepartmentService : BaseRepository<Department>, IDepartmentService
    {
        public DepartmentService(IFreeSql fsql) : base(fsql)
        {
        }


    }
}
