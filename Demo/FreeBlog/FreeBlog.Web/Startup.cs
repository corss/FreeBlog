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
   .UseAutoSyncStructure(true)//�Զ�ͬ��ʵ��ṹ�����������ر�����FreeSql����ɨ����򼯣�ֻ��CRUDʱ�Ż����ɱ�
   .UseLazyLoading(true)
   .UseNoneCommandParameter(true)
   .Build();//����ض���� Singleton ����ģʽ
        }

        public IConfiguration Configuration { get; }
        [System.Obsolete]
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //ע��
            services.AddSingleton<IFreeSql>(Fsql);
            services.AddFreeDbContext<TemplateContext>(options => options.UseFreeSql(Fsql));
            services.AddControllersWithViews();
            // ����Cookie
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(opt => { opt.LoginPath = new PathString("/Index/Login/"); });

            // ����session
            services.AddSession();
            services.AddControllers().AddControllersAsServices(); //����������ʵ������

        }
        /// <summary>
        /// Autofacע��
        /// </summary>
        /// <param name="containerBuilder"></param>
        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule<ConfigureAutofac>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // ����ģʽ
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // session
            app.UseSession();

            // web������̬�ļ����м�� û������޷�����ͼƬ
            app.UseStaticFiles();

            // ��HTTP������תΪHTTPS
            app.UseHttpsRedirection();

            // ·���м��
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});

                // MVC��ʽ
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Main}/{action=Index}/{id?}");
            });
        }
    }
}
