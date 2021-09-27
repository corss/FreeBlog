using System;
using System.Collections.Generic;
using System.Text;

namespace FreeBlog.Common
{
    /// <summary>
    /// API 返回JSON字符串
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        public string msg { get; set; } = "执行成功！";
        /// <summary>
        /// 状态码
        /// </summary>
        public int code { get; set; } = 200;
    }
    /// <summary>
    /// API 返回JSON字符串
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResult<T> : ApiResult
    {
        /// <summary>
        /// 数据集
        /// </summary>
        public T data { get; set; }
        /// <summary>
        /// 分页页码
        /// </summary>
        public int index { get; set; }
        /// <summary>
        /// 分页的总数据
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 每页数量
        /// </summary>
        public int size { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int pages { get; set; }
    }
}
