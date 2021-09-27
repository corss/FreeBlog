using FreeBlog.IService;
using FreeBlog.Model.Models.PoliceModel;
using FreeBlog.Service.Base;

namespace FreeBlog.Service
{
    public class PoliceprojectMenuService : BaseRepository<PoliceprojectMenu>, IPoliceprojectMenuService
    {
        public PoliceprojectMenuService(IFreeSql fsql) : base(fsql)
        {
        }


    }
}
