using FreeBlog.IService;
using FreeBlog.Model.Models.PoliceModel;
using FreeBlog.Service.Base;

namespace FreeBlog.Service
{
    public class PoliceForceService : BaseRepository<PoliceForce>, IPoliceForceService
    {
        public PoliceForceService(IFreeSql fsql) : base(fsql)
        {
        }


    }
}
