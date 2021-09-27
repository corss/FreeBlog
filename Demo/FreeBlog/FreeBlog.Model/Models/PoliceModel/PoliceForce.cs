using FreeSql.DataAnnotations;
using System;


namespace FreeBlog.Model.Models.PoliceModel
{
    /// <summary>
    /// 干警队伍
    /// </summary>
    public class PoliceForce
    {
        [Column(IsIdentity = true)]
        public int ID { get; set; }

        /// <summary>
        /// 队伍名称
        /// </summary>
        public string Names { get; set; }
        /// <summary>
        /// 队长ID
        /// </summary>
        public int PID { get; set; }

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
