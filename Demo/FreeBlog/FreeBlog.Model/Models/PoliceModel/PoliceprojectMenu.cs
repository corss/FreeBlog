using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Model.Models.PoliceModel
{
    /// <summary>
    /// 干警项目
    /// </summary>
   public  class PoliceprojectMenu
    {
        ////唯一键标识,不然数据库表字段会根据实体重新生成
        [Column(IsIdentity = true)]
        public int ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Names { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public int ParentID { get; set; }
        /// <summary>
        /// 负责小队ID
        /// </summary>
        public int Fid { get; set; }
        /// <summary>
        /// 顺序
        /// </summary>
        public int Sorting { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddDate { get; set; }
    }
}
