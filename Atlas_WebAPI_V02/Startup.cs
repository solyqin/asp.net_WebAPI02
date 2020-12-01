using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas_WebAPI_V02.Log;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Atlas_WebAPI_V02
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// log4net 仓储库; logger 仓库，主要用于日志对象组织结构的维护
        /// </summary>
        public static ILoggerRepository repository { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //log注入ILoggerHelper
            services.AddSingleton<ILoggerHelper, LoggerHelper>();
            //log4net
            repository = LogManager.CreateRepository("Atlas_WebAPI_V01");//项目名称
            XmlConfigurator.Configure(repository, new System.IO.FileInfo("log4net.config"));//指定配置文件，
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
