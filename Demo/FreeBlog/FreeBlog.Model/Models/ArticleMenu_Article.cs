

using FreeSql.DataAnnotations;

namespace FreeBlog.Model.Models
{
    /// <summary>
    /// 任务和项目关联表
    /// </summary>
    public class ArticleMenu_Article
    {
        [Column(IsIdentity = true)]
        public int ID { get; set; }
        /// <summary>
        /// 评测项目栏目ID
        /// </summary>
        public int ArticleMenuID { get; set; }
        /// <summary>
        /// 分配任务ID
        /// </summary>
        public int ArticleID { get; set; }
    }
}
