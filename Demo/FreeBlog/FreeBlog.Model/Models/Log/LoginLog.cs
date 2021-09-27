using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Model.Models.Log
{
    public class LoginLog : Log
    {
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
        /// <summary>
        /// 公司ID
        /// </summary>
        public int CompanyID { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
