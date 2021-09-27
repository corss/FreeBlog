using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Model.Models.PoliceModel
{
    /// <summary>
    /// 分数列表
    /// </summary>
    public class PoliceGrade
    {
        ////唯一键标识,不然数据库表字段会根据实体重新生成
        [Column(IsIdentity = true)]
        public int ID { get; set; }
        /// <summary>
        ///分数来源所属活动表ID，0表示由分数管理添加
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 分数所属干警唯一编号
        /// </summary>
        public int OID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 志愿服务分数
        /// </summary>
        public int Grade { get; set; }
        /// <summary>
        /// 学习强国分数
        /// </summary>
        public int GradeC { get; set; }
        /// <summary>
        ///添加时间，即年度
        /// </summary>
        public DateTime AddDate { get; set; }
    }
}
