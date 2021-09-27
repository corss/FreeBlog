using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Model.Models
{    /// <summary>
     /// 评测项目
     /// </summary>
    public class ArticleMenu
    {
        ////唯一键标识,不然数据库表字段会根据实体重新生成
        [Column(IsIdentity = true)]
        public int ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Names { get; set; }
        /// <summary>
        /// 存英文名或副标题
        /// </summary>
        public string ENames { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public int ParentID { get; set; }
        /// <summary>
        /// 顺序
        /// </summary>
        public int Sorting { get; set; }
        /// <summary>
        /// 状态 1.未审核 2.已审核 3.标记删除
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddDate { get; set; }

    }
}
