using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IGNAuthentication.Data;
using IGNAuthentication.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IGNAuthentication.Domain.Interfaces.Services;
using IGNAuthentication.Domain.Services;

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
            services.AddLogging();
            var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");
            var adminAccessCode = Environment.GetEnvironmentVariable("ADMIN_CODE");
            var msSqlConnectionString = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION_STRING");
            if (string.IsNullOrWhiteSpace(connectionString) && string.IsNullOrWhiteSpace(msSqlConnectionString))
            {
            }
            else
            {
                if (string.IsNullOrWhiteSpace(msSqlConnectionString))
                {
                    services.AddSingleton<IDataProvider, MySqlDataProvider>(sp=>new MySqlDataProvider(sp.GetService<ILogger>()));
                }
                else
                {
                    services.AddSingleton<IDataProvider, MsSqlDataProvider>(sp => new MsSqlDataProvider(sp.GetService<ILogger>()));
                }
            }
            services.AddScoped<IUserService, UserService>(sp => new UserService(sp.GetService<IDataProvider>()));
            var secret = Environment.GetEnvironmentVariable("SECRET");
            var key = Encoding.ASCII.GetBytes(secret);
            services.AddCors(x =>
            {
                x.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                });
            });
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x=>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = Environment.GetEnvironmentVariable("ISSUER"),
                    ValidateAudience = false
                };
            });            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors();
            app.UseAuthentication();
            app.UseMvc();
            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
    }
}
