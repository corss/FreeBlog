using System;
using System.Collections.Generic;
using System.Text;
namespace FreeBlog.Model.ViewModels.Api
{
    public class APIProUserViewModel
    {
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
        /// 顺序
        /// </summary>
        public int Sorting { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
    }
}
