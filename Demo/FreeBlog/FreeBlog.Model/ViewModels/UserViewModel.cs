using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Model.ViewModels
{
    public class UserViewModel
    {
        public int id { get; set; }
        public string username { get; set; }
        public string loginName { get; set; }
        public string password { get; set; }
        public string authorities { get; set; }
        public string nickName { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public  int deptId { get; set; }
        public  string deptName { get; set; }
        public string createTime { get; set; }
        public string updateTime { get; set; }
        public string isAdmin { get; set; }
        public string permissionList { get; set; }

        public string sex { get; set; } 
        public string postId { get; set; }
        public string postName { get; set; }
        /// <summary>
        /// 对应角色名称
        /// </summary>
        public string RoleName { get; set; }
        public bool enabled { get; set; }
        public bool accountNonExpired { get; set; }
        public bool accountNonLocked { get; set; }
        public bool credentialsNonExpired { get; set; }
    }
}
