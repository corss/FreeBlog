
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
namespace FreeBlog.Api.Extensions
{
    public class ExceptionHandlerMidd
    {
        private readonly RequestDelegate _next;
        

        public ExceptionHandlerMidd(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception e)
        {
            if (e == null) return;

            await WriteExceptionAsync(context, e).ConfigureAwait(false);
        }

        private static async Task WriteExceptionAsync(HttpContext context, Exception e)
        {
            if (e is UnauthorizedAccessException)
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            else if (e is Exception)
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonConvert.SerializeObject((new ApiResponse(StatusCode.CODE500, e.Message)).MessageModel)).ConfigureAwait(false);
        }

    }

}
