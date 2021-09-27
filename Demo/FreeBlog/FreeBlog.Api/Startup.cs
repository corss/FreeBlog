using Autofac;
using FreeBlog.Api.ApiGroup;
using FreeBlog.Api.Extensions;
using FreeBlog.Api.Filter;
using FreeBlog.Common;
using FreeBlog.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Profiling.Storage;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Api
{
    public class Startup
    {
        public static IFreeSql Fsql { get; private set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Fsql = new FreeSql.FreeSqlBuilder()
   .UseConnectionString(FreeSql.DataType.SqlServer, AppSettings.app("DbConnection", "ConnectionString"))
   .UseAutoSyncStructure(true)//�Զ�ͬ��ʵ��ṹ�����������ر�����FreeSql����ɨ����򼯣�ֻ��CRUDʱ�Ż����ɱ�
   .UseLazyLoading(true)
   .UseNoneCommandParameter(true)
   .Build();//����ض���� Singleton ����ģʽ
        }

        public IConfiguration Configuration { get; }



        //�˷���������ʱ���á�ʹ�ô˷�������������ӷ���
        public void ConfigureServices(IServiceCollection services)
        {

            // ��������
            services.AddCors(c =>
            {
                // ���ò���
                c.AddPolicy("LimitRequests", policy =>
                {
                    // ֧�ֶ�������˿ڣ�ע��˿ںź�Ҫ��/б�ˣ�����localhost:8000/���Ǵ��
                    // http://127.0.0.1:1818 �� http://localhost:1818 �ǲ�һ���ģ�����д����
                    policy
                    .SetIsOriginAllowed((x) => true)//������������                   
                    .AllowAnyHeader()//��������ͷ
                    .AllowAnyMethod()//�������ⷽ��
                    .AllowCredentials();//ָ������cookie
                });
            });
            services.AddDistributedMemoryCache();//����session֮ǰ����������ڴ�
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            // ����session
            services.AddSession(options =>
            {
                options.IdleTimeout = System.TimeSpan.FromSeconds(2000);//����session�Ĺ���ʱ��
                //options.Cookie.HttpOnly = false;//���������������ͨ��js��ø�cookie��ֵ
            });
            // ע�����
            services.AddControllers().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.PropertyNamingPolicy = null;  // ���ؽӿڲ������շ�������
            });
            //Mapperӳ��
            services.AddAutoMapperSetup();

            //ע��
            services.AddSingleton<IFreeSql>(Fsql);
            services.AddFreeDbContext<TemplateContext>(options => options.UseFreeSql(Fsql));

            //services.AddControllers();
            // Swgger
            services.AddSwaggerGen(options =>
            {
                //����ApiGroupNames����ö��ֵ���ɽӿ��ĵ���Skip(1)����ΪEnum��һ��FieldInfo�����õ�һ��Intֵ
                typeof(ApiGroupNames).GetFields().Skip(1).ToList().ForEach(f =>
                {
                    //��ȡö��ֵ�ϵ�����
                    var info = f.GetCustomAttributes(typeof(GroupInfoAttribute), false).OfType<GroupInfoAttribute>().FirstOrDefault();
                    options.SwaggerDoc(f.Name, new OpenApiInfo
                    {
                        Title = info?.Title,
                        Version = info?.Version,
                        Description = info?.Description
                    });
                });
                //û�м����Եķֵ����NoGroup��
                options.SwaggerDoc("NoGroup", new OpenApiInfo
                {
                    Title = "Ĭ�Ϸ���"
                });
                //�жϽӿڹ����ĸ�����
                options.DocInclusionPredicate((docName, apiDescription) =>
                {
                    if (docName == "NoGroup")
                    {
                        //������ΪNoGroupʱ��ֻҪû�����ԵĶ����������
                        return string.IsNullOrEmpty(apiDescription.GroupName);
                    }
                    else
                    {
                        return apiDescription.GroupName == docName;
                    }
                });

                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//��ȡӦ�ó�������Ŀ¼�����ԣ����ܹ���Ŀ¼Ӱ�죬������ô˷�����ȡ·����
                var xmlPath = Path.Combine(basePath, "Api.xml");//������Ǹո����õ�xml�ļ���
                var xmlPaths = Path.Combine(basePath, "FreeBlog.Model.xml");//������Ǹո����õ�xml�ļ���
                options.IncludeXmlComments(xmlPath, true);//Ĭ�ϵĵڶ���������false�������controller��ע�ͣ��ǵ��޸�
                options.IncludeXmlComments(xmlPaths, true);//Ĭ�ϵĵڶ���������false�������controller��ע�ͣ��ǵ��޸�
                #region Token�󶨵�ConfigureServices
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) ֱ�����¿�������Bearer {token}��ע������֮����һ���ո�\"",
                    Name = "Authorization",//jwtĬ�ϵĲ�������
                    In = ParameterLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
                    Type = SecuritySchemeType.ApiKey
                });
                // ������Ȩ��
                options.OperationFilter<AddResponseHeadersFilter>();
                options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                //��header�����token,���ݵ���̨
                options.OperationFilter<SecurityRequirementsOperationFilter>();
                #endregion

            });

            // 1.����Ȩ���ԡ����Բ�����controller�У�д��� roles 
            // controller д�� [Authorize(Policy = "Admin")]
            services.AddAuthorization(options =>
            {
                // �û�
                options.AddPolicy("User", policy => policy.RequireRole("User").Build());
                // ����Ա
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin", "User"));
            });
            #region ���ڶ�����������֤����
            //��ȡ�����ļ�
            var keyByteArray = Encoding.ASCII.GetBytes(JwtHelper.Secret);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // ������֤����
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,  // ��Կ
                ValidateIssuer = true,
                ValidIssuer = JwtHelper.Issuer, //������
                ValidateAudience = true,
                ValidAudience = JwtHelper.Audience,//������
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),
                RequireExpirationTime = true,
            };

            //2.1����֤����core�Դ��ٷ�JWT��֤
            // ����Bearer��֤
            // ����Bearer��֤
            services.AddAuthentication(o => {
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = nameof(ApiResponseHandler);
                o.DefaultForbidScheme = nameof(ApiResponseHandler);
            })
             // ���JwtBearer����
             .AddJwtBearer(o =>
             {
                 o.TokenValidationParameters = tokenValidationParameters;
                 o.Events = new JwtBearerEvents
                 {
                     //OnChallenge = context =>
                     //{
                     //    context.Response.Headers.Add("Token-Error", context.ErrorDescription);
                     //    return Task.CompletedTask;
                     //},
                     OnAuthenticationFailed = context =>
                     {
                         // ������ڣ����<�Ƿ����>��ӵ�������ͷ��Ϣ��
                         if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                         {
                             context.Response.Headers.Add("Token-Expired", "true");
                         }
                         return Task.CompletedTask;
                     }
                 };
             })
             .AddScheme<AuthenticationSchemeOptions, ApiResponseHandler>(nameof(ApiResponseHandler), o => { }); ;
            #endregion

            //ȫ���쳣����
            services.AddControllers(o =>
            {
                o.Filters.Add(typeof(GlobalExceptionsFilter));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            //services.AddControllers().AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.IgnoreNullValues = true;
            //});

            services.AddMiniProfiler(options =>
            {
                options.RouteBasePath = "/profiler";//ע�����·��Ҫ���±� index.html �ű������е�һ�£�
                (options.Storage as MemoryCacheStorage).CacheDuration = TimeSpan.FromMinutes(10);

            });
        }



        /// <summary>
        /// Autofacע��
        /// </summary>
        /// <param name="containerBuilder"></param>
        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule<ConfigureAutofac>();
        }
        //�˷���������ʱ���á�ʹ�ô˷�������HTTP����ܵ���
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseMiddleware<Filter.CorsMiddleware>();
            // ����ģʽ
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // ���� Swagger �м�� 
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                //����ApiGroupNames����ö��ֵ���ɽӿ��ĵ���Skip(1)����ΪEnum��һ��FieldInfo�����õ�һ��Intֵ
                typeof(ApiGroupNames).GetFields().Skip(1).ToList().ForEach(f =>
                {
                    //��ȡö��ֵ�ϵ�����
                    var info = f.GetCustomAttributes(typeof(GroupInfoAttribute), false).OfType<GroupInfoAttribute>().FirstOrDefault();
                    options.SwaggerEndpoint($"/swagger/{f.Name}/swagger.json", info != null ? info.Title : f.Name);

                });
                options.SwaggerEndpoint("/swagger/NoGroup/swagger.json", "Ĭ�Ϸ���");
                options.DocExpansion(DocExpansion.None); //->�ӿ��ĵ������ʱ�Զ��۵�
            });
            // session
            app.UseSession();
            // web������̬�ļ����м�� û������޷�����ͼƬ
            app.UseStaticFiles();
            // ���ش�����
            app.UseStatusCodePages();//�Ѵ����뷵��ǰ̨��������404
            app.UseHttpsRedirection();
            // ·���м��
            app.UseRouting();
            //��� Cors �����м��
            app.UseCors("LimitRequests");
            // �ȿ�����֤
            app.UseAuthentication();
            // Ȼ������Ȩ�м��
            app.UseAuthorization();
            app.UseMiniProfiler();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
