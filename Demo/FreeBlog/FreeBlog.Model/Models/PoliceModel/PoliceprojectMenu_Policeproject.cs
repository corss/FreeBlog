using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FreeBlog.Model.Models.PoliceModel
{
    /// <summary>
    /// 干警项目和任务关联表
    /// </summary>
   public  class PoliceprojectMenu_Policeproject
    {
        [Column(IsIdentity = true)]
        public int ID { get; set; }
        /// <summary>
        /// 干警项目ID
        /// </summary>
        public int ProjectMenuID { get; set; }
        /// <summary>
        /// 分配任务ID
        /// </summary>
        public int ProjectID { get; set; }
    }
}
