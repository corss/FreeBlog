﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;

namespace FreeBlog.Common
{
    public class AppSettings
    {
        public static IConfiguration Configuration { get; set; }
        static AppSettings()
        {
            //ReloadOnChange = true 当appsettings.json被修改时重新加载            
            Configuration = new ConfigurationBuilder()
            .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
            .Build();
        }
        /// <summary>
        /// 封装要操作的字符
        /// </summary>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static string app(params string[] sections)
        {
            try
            {
                var val = string.Empty;
                for (int i = 0; i < sections.Length; i++)
                {
                    val += sections[i] + ":";
                }

                return Configuration[val.TrimEnd(':')];
            }
            catch (Exception)
            {
                return "";
            }

        }

        /// <summary>
        /// 获得配置文件的对象值
        /// </summary>
        /// <param name="jsonPath"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetJson(string jsonPath, string key)
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile(jsonPath).Build(); //json文件地址
            string s = config.GetSection(key).Value; //json某个对象
            return s;
        }
    }
}
