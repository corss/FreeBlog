using FreeSql.DataAnnotations;


namespace FreeBlog.Model.Models
{
    public class Role_value
    {
        /// <summary>
        /// 编号
        /// </summary>
        [Column(IsIdentity = true)]
        public int Id { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Menu_name { get; set; }
        /// <summary>
        /// 权限类型，Show:显示,Delete:删除,update:更新,Add:新增
        /// </summary>
        public string Action_Type { get; set; }
    }
}
