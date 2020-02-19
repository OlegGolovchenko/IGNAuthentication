using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IGNAuthentication.Data;
using IGNAuthentication.Domain;
using IGNAuthentication.Domain.Interfaces;

namespace IGNLogin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");
            var adminAccessCode = Environment.GetEnvironmentVariable("ADMIN_CODE");
            var msSqlConnectionString = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION_STRING");
            IDataProvider dataConn;
            if (string.IsNullOrWhiteSpace(connectionString) && string.IsNullOrWhiteSpace(msSqlConnectionString))
            {
                dataConn = null;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(msSqlConnectionString))
                {
                    dataConn = new MsSqlDataProvider(connectionString);
                }
                else
                {
                    dataConn = new MySqlDataProvider(msSqlConnectionString);
                }
            }
            var repo = new IGNAuthentication.Domain.ServiceProvider(dataConn);
            services.AddSingleton(repo);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
