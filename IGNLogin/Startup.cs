using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IGNAuthentication.Domain.Interfaces.Services;
using IGNAuthentication.Domain.Services;
using IGNQuery.Interfaces;
using IGNQuery.MySql;
using IGNQuery.SqlServer;
using Microsoft.AspNetCore.HttpOverrides;

namespace IGNLogin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private MySqlDataProvider InitMySqlDataProvider(ILogger logger,string activationEmail)
        {
            var mysqldp = new MySqlDataProvider(activationEmail);
            mysqldp.queryToOutput = true;
            return mysqldp;
        }

        private MsSqlDataProvider InitMsSqlDataProvider(ILogger logger, string activationEmail)
        {
            var mssqldp = new MsSqlDataProvider(activationEmail);
            mssqldp.queryToOutput = true;
            return mssqldp;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");
            var adminAccessCode = Environment.GetEnvironmentVariable("ADMIN_CODE");
            var msSqlConnectionString = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION_STRING");
            var activationMail = Environment.GetEnvironmentVariable("ACTIVATION_MAIL");
            if (string.IsNullOrWhiteSpace(connectionString) && string.IsNullOrWhiteSpace(msSqlConnectionString))
            {
            }
            else
            {
                if (string.IsNullOrWhiteSpace(msSqlConnectionString))
                {
                    services.AddSingleton<IDataProvider, MySqlDataProvider>(sp => InitMySqlDataProvider(sp.GetService<ILogger>(),activationMail));
                }
                else
                {
                    services.AddSingleton<IDataProvider, MsSqlDataProvider>(sp => InitMsSqlDataProvider(sp.GetService<ILogger>(), activationMail));
                }
            }
            services.AddScoped<IUserService, UserService>(sp => new UserService(sp.GetService<IDataProvider>()));
            services.AddScoped<ILicenseService, LicenseService>(sp => new LicenseService(sp.GetService<IDataProvider>()));
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
            }).AddJwtBearer(x =>
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
            services.AddControllers();
            services.AddRazorPages();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseForwardedHeaders();
            app.UseCors();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
