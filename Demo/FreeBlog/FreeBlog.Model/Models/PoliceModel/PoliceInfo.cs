
using FreeSql.DataAnnotations;
using System;


namespace FreeBlog.Model.Models.PoliceModel
{
    /// <summary>
    /// 干警列表
    /// </summary>
    public class PoliceInfo
    {

        [Column(IsIdentity = true)]
        public int ID { get; set; }

        /// <summary>
        /// 干警名称
        /// </summary>
        public string Names { get; set; }
        /// <summary>
        /// 唯一个人编号
        /// </summary>
        public int OID { get; set; }
        /// <summary>
        /// 所属队伍
        /// </summary>
        public int PFID { get; set; }
        /// <summary>
        /// 角色 1.组长 2.组员
        /// </summary>
        public int PFRole { get; set; }
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
        /// <summary>
        /// 志愿者服务分
        /// </summary>
        public int GradeF { get; set; }
        /// <summary>
        /// 学习强国分（1000=1志愿者服务分）
        /// </summary>
        public int Grade { get; set; }
    }
}
