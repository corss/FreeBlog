
using FreeSql.DataAnnotations;
using System;

namespace FreeBlog.Model.Models
{
    /// <summary>
    /// 台账表
    /// </summary>
    public class StandingBook
    {
        ////唯一键标识,不然数据库表字段会根据实体重新生成
        [Column(IsIdentity = true)]
        public int ID { get; set; }
        /// <summary>
        /// 所属任务ID
        /// </summary>
        public int FID { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public int DepartmentID { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [Column(DbType = "nvarchar(MAX) NOT NULL")]
        public string Names { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        public string file { get; set; }
        /// <summary>
        /// 顺序
        /// </summary>
        public int Sorting { get; set; }
        /// <summary>
        /// 内容
        /// </summary>

        [Column(DbType = "nvarchar(MAX) NOT NULL")]
        public string Content { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddDate { get; set; }
    }
}
