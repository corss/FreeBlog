using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Model.ViewModels
{
   public class PoliceGradeModel
    {
        /// <summary>
        /// 干警唯一ID
        /// </summary>
        public int OID { get; set; }
        /// <summary>
        /// 参加活动ID
        /// </summary>
        public  string Pid { get; set; }
        /// <summary>
        /// 参加活动名称
        /// </summary>
        public string Pname { get; set; }
        /// <summary>
        /// 干警名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 爱国分数
        /// </summary>
        public int GradeC { get; set; }
        /// <summary>
        /// 志愿者分数
        /// </summary>
        public  int Grade { get; set; }
    }
}
