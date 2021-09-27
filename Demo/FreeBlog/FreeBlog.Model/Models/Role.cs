using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Model.Models
{    /// <summary>
     /// 角色
     /// </summary>
    public class Role
    {
        ////唯一键标识,不然数据库表字段会根据实体重新生成
        [Column(IsIdentity = true)]
        public int id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public string name { get; set; }
        public string remark { get; set; }
        /// <summary>
        /// 顺序
        /// </summary>
        [Column(InsertValueSql = "0")]
        public int Sorting { get; set; }
        /// <summary>
        /// 菜单权限
        /// </summary>
        public string Menus { get; set; }
        /// <summary>
        /// 栏目权限
        /// </summary>
        public string ArticleMenus { get; set; }
        /// <summary>
        /// 接口权限
        /// </summary>
        //@ApiModelProperty(hidden = true)
        //@JsonIgnore
        //[ApiModelProperty(hidden = true)]
        public string Modules { get; set; }
    }
}
