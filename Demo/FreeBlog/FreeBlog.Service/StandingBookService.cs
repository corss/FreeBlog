using FreeBlog.IService;
using FreeBlog.Model.Models;
using FreeBlog.Service.Base;

namespace FreeBlog.Service
{
    public class StandingBookService : BaseRepository<StandingBook>, IStandingBookService
    {
        public StandingBookService(IFreeSql fsql) : base(fsql)
        {
        }
    }
}
