
using FreeSql.DataAnnotations;

namespace FreeBlog.Model.Models
{
    /// <summary>
    /// 部门表
    /// </summary>
    public class Department
    {
        [Column(IsIdentity = true)]
        public int id { get; set; }
        /// <summary>
        /// 部门地址
        /// </summary>
        public string deptAddress { get; set; }
        /// <summary>
        /// 部门编码
        /// </summary>
        public string deptCode { get; set; }
        /// <summary>
        /// 部门电话
        /// </summary>
        public string deptPhone { get; set; }
        /// <summary>
        /// 上级部门id集合
        /// </summary>
        public string likeId { get; set; }
        /// <summary>
        /// 部门经理
        /// </summary>
        public string manager { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public  bool open { get; set; }
        /// <summary>
        /// 顺序
        /// </summary>
        public int orderNum { get; set; }
        /// <summary>
        /// 上级部门名称
        /// </summary>
        public string parentName { get; set; }
        /// <summary>
        /// 上级部门id
        /// </summary>
        public int pid { get; set; }
    }
}
