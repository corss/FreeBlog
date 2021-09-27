using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeBlog.Api.ApiGroup
{
    /// <summary>
    /// 系统分组枚举值
    /// </summary>
    public enum ApiGroupNames
    {
        /// <summary>
        /// 登录认证
        /// </summary>
        [GroupInfo(Title = "登录认证，首页", Description = "登录认证相关接口", Version = "v1")]
        AdminAuth,
        /// <summary>
        /// 台账
        /// </summary>
        [GroupInfo(Title = "台账", Description = "台账相关接口", Version = "v1")]
        SBook,
        /// <summary>
        /// 后台系统管理
        /// </summary>
        [GroupInfo(Title = "后台系统管理", Description = "后台系统管理接口", Version = "v1")]
        AdminSystem,
        /// <summary>
        /// 干警管理
        /// </summary>
        [GroupInfo(Title = "干警管理", Description = "干警管理接口", Version = "v1")]
        AdminPolice,
    }
}
