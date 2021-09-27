using FreeBlog.IService;
using FreeBlog.Model.Models;
using FreeBlog.Service.Base;

namespace FreeBlog.Service
{
    class Role_valueService:BaseRepository<Role_value>, IRole_valueService
    {
        public Role_valueService(IFreeSql fsql) : base(fsql)
        {
        }
    }
}
