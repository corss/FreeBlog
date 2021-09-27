using FreeBlog.IService;
using FreeBlog.Model.Models;
using FreeBlog.Service.Base;

namespace FreeBlog.Service
{
    public class ArticleMenu_ArticleService : BaseRepository<ArticleMenu_Article>, IArticleMenu_ArticleService
    {
        public ArticleMenu_ArticleService(IFreeSql fsql) : base(fsql)
        {
        }
    }
}
