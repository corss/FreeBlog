using FreeBlog.IService;
using FreeBlog.Model.Models;
using FreeBlog.Service.Base;


namespace FreeBlog.Service
{
    public class ArticleService : BaseRepository<Article>, IArticleService
    {
        public ArticleService(IFreeSql fsql) : base(fsql)
        {
        }
    }
}
