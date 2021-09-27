using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Model.Models.Log
{
    public class ExceptionLog : Log
    {
        [Column(DbType = "nvarchar(MAX)")]
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 是否解决
        /// </summary>
        public bool IsSolve { get; set; }
        /// <summary>
        /// 类型 1.异常 2.文件不存在 3.登录失败 4.其它
        /// </summary>
        public int Type { get; set; }
    }
}
