using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Model.Models.Log
{
    /// <summary>
    /// 日志父类
    /// </summary>
    public class Log
    {
        [Column(IsIdentity = true)]
        public int ID { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 触发的用户ID
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// 触发的项目ID
        /// </summary>
        public string ProjectID { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddDate { get; set; }
    }
}
