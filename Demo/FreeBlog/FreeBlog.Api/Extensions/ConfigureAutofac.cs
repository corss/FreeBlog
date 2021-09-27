using Autofac;
using System.IO;
using System.Reflection;

namespace FreeBlog.Api.Extensions
{
    public class ConfigureAutofac : Autofac.Module
    {
        /// <summary>
        /// 加载调用
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);

            //var cacheType = new List<Type>();
            //if (Appsettings.app(new string[] { "AppSettings", "RedisCaching", "Enabled" }).ObjToBool())
            //{
            //    builder.RegisterType<CacheAOP>();
            //    cacheType.Add(typeof(CacheAOP));
            //}

            //程序集1注入
            var ServicesDllFile = Path.Combine(basePath, "FreeBlog.Service.dll");//获取注入项目绝对路径
            var assemblysServices = Assembly.LoadFile(ServicesDllFile);//直接采用加载文件的方法
            builder.RegisterAssemblyTypes(assemblysServices)
                .AsImplementedInterfaces();
                //.EnableInterfaceInterceptors()//对目标类型启用接口拦截。
                //.InterceptedBy(cacheType.ToArray());//允许将拦截器服务的列表分配给注册。

            //#region
            //var controllersTypesInAssembly = typeof(Startup).Assembly.GetExportedTypes()
            //.Where(type => typeof(Microsoft.AspNetCore.Mvc.ControllerBase).IsAssignableFrom(type)).ToArray();
            //builder.RegisterTypes(controllersTypesInAssembly).PropertiesAutowired();
            //#endregion

            base.Load(builder);
        }
    }
}
