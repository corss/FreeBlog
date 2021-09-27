using FreeBlog.IService.Base;
using FreeBlog.Model.Models;
using System;

namespace FreeBlog.IService
{
    public interface IUserService : IRepository<UserInfo>
    {
        /// <summary>
        /// 获取用户显示名称
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public string GetNames(UserInfo u);
    }
}
