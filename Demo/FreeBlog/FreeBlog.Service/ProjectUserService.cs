using FreeBlog.IService;
using FreeBlog.Model.Models.Project;
using FreeBlog.Service.Base;

namespace FreeBlog.Service
{
    public class ProjectUserService : BaseRepository<ProjectUserInfo>, IProjectUserService
    {
        public ProjectUserService(IFreeSql fsql) : base(fsql)
        {
        }


    }
}
