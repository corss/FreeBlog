using Autofac;
using FreeBlog.Api.Extensions;
using FreeBlog.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeBlog.Web
{
    public class Startup
    {

        public static IFreeSql Fsql { get; private set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Fsql = new FreeSql.FreeSqlBuilder()
   .UseConnectionString(FreeSql.DataType.SqlServer, "Server=.;Database=templatebase_coreapi;Trusted_Connection=Yes;Connect Timeout=90;")
   .UseAutoSyncStructure(true)//自动同步实体结构【开发环境必备】，FreeSql不会扫描程序集，只有CRUD时才会生成表。
   .UseLazyLoading(true)
   .UseNoneCommandParameter(true)
   .Build();//请务必定义成 Singleton 单例模式
        }

        public IConfiguration Configuration { get; }
        [System.Obsolete]
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //注入
            services.AddSingleton<IFreeSql>(Fsql);
            services.AddFreeDbContext<TemplateContext>(options => options.UseFreeSql(Fsql));
            services.AddControllersWithViews();
            // 启用Cookie
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(opt => { opt.LoginPath = new PathString("/Index/Login/"); });

            // 启用session
            services.AddSession();
            services.AddControllers().AddControllersAsServices(); //控制器当做实例创建

        }
        /// <summary>
        /// Autofac注入
        /// </summary>
        /// <param name="containerBuilder"></param>
        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule<ConfigureAutofac>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // 开发模式
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // session
            app.UseSession();

            // web基本静态文件的中间件 没有这个无法解析图片
            app.UseStaticFiles();

            // 把HTTP的请求转为HTTPS
            app.UseHttpsRedirection();

            // 路由中间件
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});

                // MVC形式
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Main}/{action=Index}/{id?}");
            });
        }
    }
}
