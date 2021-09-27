using FreeSql.DataAnnotations;
using System;

namespace FreeBlog.Model.Models
{
    /// <summary>
    /// 用户表
    /// </summary>
    public class UserInfo
    {
        ////唯一键标识,不然数据库表字段会根据实体重新生成
        [Column(IsIdentity = true)]
         public int id { get; set; }
        /// <summary>
        /// 状态 1未审核 2已审核 3禁用 4被标记删除
        /// </summary>
        public int State { get; set; }
        public bool accountNonExpired { get; set; }

        /// <summary>
        /// 账号未锁定
        /// </summary>
        public  bool accountNonLocked { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string authorities { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool credentialsNonExpired { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public int deptId { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string depName { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
       public bool enabled { get; set; }

        /// <summary>
        /// 是否是管理员：1是，0不是
        /// </summary>
        public string isAdmin { get; set; }

        /// <summary>
        /// 登录名称
        /// </summary>
        public string loginName { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string nickName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }

        public string postId { get; set; }
        public string postName { get; set; }

        public string sex { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime updateTime { get; set; }
        public string userName { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }
 
    }
}