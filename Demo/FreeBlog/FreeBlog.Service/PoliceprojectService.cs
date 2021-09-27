using FreeBlog.IService;
using FreeBlog.Model.Models.PoliceModel;
using FreeBlog.Service.Base;

namespace FreeBlog.Service
{
    public class PoliceprojectService : BaseRepository<Policeproject>, IPoliceprojectService
    {
        public PoliceprojectService(IFreeSql fsql) : base(fsql)
        {
        }


    }
}
