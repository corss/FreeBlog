
using Microsoft.AspNetCore.Builder;

namespace FreeBlog.Api.Extensions
{
    public static class MiddlewareHelpers
    {
        /// <summary>
        /// 异常处理中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseExceptionHandlerMidd(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlerMidd>();
        }

    }
}
