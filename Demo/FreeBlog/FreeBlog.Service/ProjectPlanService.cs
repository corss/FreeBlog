using FreeBlog.IService;
using FreeBlog.Model.Models.Project;
using FreeBlog.Service.Base;

namespace FreeBlog.Service
{
    public class ProjectPlanService : BaseRepository<ProjectPlanInfo>, IProjectPlanService
    {
        public ProjectPlanService(IFreeSql fsql) : base(fsql)
        {
        }


    }
}
