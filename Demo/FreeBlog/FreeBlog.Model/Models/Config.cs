using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Model.Models
{
    /// <summary>
    /// 全局配置表
    /// </summary>
    public class Config
    {
        [Column(IsIdentity = true)]
        public int ID { get; set; }
        ///// <summary>
        ///// 是否关闭网站，关闭后管理后台继续可用
        ///// </summary>
        //public bool IsClose { get; set; }
        ///// <summary>
        ///// 是否使用IP限制
        ///// </summary>
        //public bool IPRestrict { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
}
