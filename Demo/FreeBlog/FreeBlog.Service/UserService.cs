using FreeBlog.IService;
using FreeBlog.Model.Models;
using FreeBlog.Service.Base;
using System;

namespace FreeBlog.Service
{
    public class UserService : BaseRepository<UserInfo>, IUserService
    {
        public UserService(IFreeSql fsql) : base(fsql)
        {
        }

        public string GetNames(UserInfo u)
        {
            if (!String.IsNullOrWhiteSpace(u.nickName))
                return u.nickName;
            if (!String.IsNullOrWhiteSpace(u.loginName))
                return u.loginName;
            if (!String.IsNullOrWhiteSpace(u.userName))
                return u.userName;
            return u.id.ToString();
        }
    }
}
