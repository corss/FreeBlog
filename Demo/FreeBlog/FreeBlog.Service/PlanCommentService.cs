using FreeBlog.IService;
using FreeBlog.Model.Models.Project;
using FreeBlog.Service.Base;

namespace FreeBlog.Service
{
    public class PlanCommentService : BaseRepository<PlanCommentInfo>, IPlanCommentService
    {
        public PlanCommentService(IFreeSql fsql) : base(fsql)
        {
        }


    }
}
