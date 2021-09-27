using FreeBlog.IService;
using FreeBlog.Model.Models.PoliceModel;
using FreeBlog.Service.Base;

namespace FreeBlog.Service
{
    public class PoliceGradeService : BaseRepository<PoliceGrade>, IPoliceGradeService
    {
        public PoliceGradeService(IFreeSql fsql) : base(fsql)
        {
        }


    }
}
