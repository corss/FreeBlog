using FreeBlog.IService;
using FreeBlog.Model.Models.PoliceModel;
using FreeBlog.Service.Base;

namespace FreeBlog.Service
{
    public class PoliceService : BaseRepository<PoliceInfo>, IPoliceService
    {
        public PoliceService(IFreeSql fsql) : base(fsql)
        {
        }


    }
}
