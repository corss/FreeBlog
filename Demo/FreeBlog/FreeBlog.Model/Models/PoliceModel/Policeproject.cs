using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Model.Models.PoliceModel
{
    /// <summary>
    /// 干警活动
    /// </summary>
    public class Policeproject
    {
        ////唯一键标识,不然数据库表字段会根据实体重新生成
        [Column(IsIdentity = true)]
        public int ID { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 活动所属项目ID
        /// </summary>
       public int MenuID { get; set; }
        /// <summary>
        /// 活动内容
        /// </summary>
       [Column(DbType = "nvarchar(MAX) NOT NULL")]
        public string Context { get; set; }
        /// <summary>
        /// 活动日期
        /// </summary>
        public DateTime AddDate { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 顺序
        /// </summary>
        public int Sorting { get; set; }
    }
}
