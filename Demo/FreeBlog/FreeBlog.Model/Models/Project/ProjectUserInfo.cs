using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Model.Models.Project
{
    /// <summary>
    /// 项目成员
    /// </summary>
    [Table(Name="t_projectuser")]
    public class ProjectUserInfo
    {
        [Column(IsIdentity = true)]
        public int ID { get; set; }
        /// <summary>
        /// 项目
        /// </summary>
        public string ProjectID { get; set; }
        /// <summary>
        /// 节点
        /// </summary>
        public int ProjectNodeID { get; set; }
        /// <summary>
        /// 成员
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// 角色 1.组长 2.组员
        /// </summary>
        public int Role { get; set; }
        /// <summary>
        /// 比例
        /// </summary>
        public decimal Proportion { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddDate { get; set; }
        /// <summary>
        /// 顺序
        /// </summary>
        public int Sorting { get; set; }
    }
}
