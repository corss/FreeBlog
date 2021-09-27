using FreeBlog.Model.Models.PoliceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Model.ViewModels.Api
{
    public  class PoliceprojectModel
    {
        public int ID { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 分数列表
        /// </summary>
        public List<PoliceGrade> policeGrades { get; set; }
        /// <summary>
        /// 活动所属项目ID
        /// </summary>
        public int MenuID { get; set; }
        /// <summary>
        /// 活动内容
        /// </summary>
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
