using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Model.Models
{    /// <summary>
     /// 菜单表
     /// </summary>
    public class Menu
    {
        ////唯一键标识,不然数据库表字段会根据实体重新生成
        [Column(IsIdentity = true)]
        public int id { get; set; }
        /// <summary>
        /// 权限标识
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string label { get; set; }
        /// <summary>
        /// 路由名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 菜单图标
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool open { get; set; }
        /// <summary>
        /// 顺序
        /// </summary>
        public int orderNum { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public int ? parentId { get; set; }
        /// <summary>
        /// 菜单的父级名称
        /// </summary>
        public string parentName { get; set; }
        /// <summary>
        /// 路由地址
        /// </summary>
        public string path { get; set; }
        /// <summary>
        /// 类型 (0 目录 1菜单，2按钮)
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime updateTime { get; set; }
        /// <summary>
        /// 路由页面对应的路径
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
    }
}
