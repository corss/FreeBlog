using FreeBlog.IService;
using FreeBlog.Model.Models;
using FreeBlog.Service.Base;

namespace FreeBlog.Service
{
    public class ArticleMenuService : BaseRepository<ArticleMenu>, IArticleMenuService
    {
        public ArticleMenuService(IFreeSql fsql) : base(fsql)
        {
        }
    }
}
