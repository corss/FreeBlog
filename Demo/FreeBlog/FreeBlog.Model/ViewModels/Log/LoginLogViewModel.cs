using System;
using System.Collections.Generic;
using System.Text;

namespace FreeBlog.Model.ViewModels.Log
{
    public class LoginLogViewModel
    {
        public int ID { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 触发的用户ID
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public string AddDate { get; set; }
        /// <summary>
        /// 触发的系统
        /// </summary>
        public string Os { get; set; }
        /// <summary>
        /// 触发的浏览器
        /// </summary>
        public string Browser { get; set; }
        /// <summary>
        /// 分辨率宽度 ResolutionWidth
        /// </summary>
        public int R_W { get; set; }
        /// <summary>
        /// 分辨率高度 ResolutionHeight
        /// </summary>
        public int R_H { get; set; }
    }
}
