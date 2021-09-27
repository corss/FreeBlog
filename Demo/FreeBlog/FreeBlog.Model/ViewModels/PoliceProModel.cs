using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Model.ViewModels
{
    public class PoliceProModel
    {
        public int ID { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 栏目
        /// </summary>
        public List<PoliceprojectMenuModel> ProjectMenu { get; set; }
        /// <summary>
        /// 活动内容
        /// </summary>
        public string Context { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public string AddDate { get; set; }
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
