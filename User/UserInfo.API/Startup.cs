using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using NLog.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.IO;
using User.DBUtility;
using User.IRepository;
using User.Repository;

namespace UserInfo.API
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IPersistedGrantRepositoryService, PersistedGrantRepositoryService>();
            services.AddTransient<IUserService, UserService>();

            #region Swagger API文档生成
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "用户系统API", Version = "v1" });
                var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "UserInfo.API.xml");
                c.IncludeXmlComments(filePath);
            });
            #endregion

            services.AddEnyimMemcached(c =>
            {
                Configuration.GetSection("enyimMemcached").Bind(c);

            });



            ICollection<string> lst = new List<string>();
            lst.Add("User");
            services.AddAuthorization();
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration.GetSection("Authority:url").Value;
                    options.RequireHttpsMetadata = false;
                    options.AllowedScopes = lst;
                    //options.ApiName = "api1";
                    //options.ApiSecret = "secret";
                });


            services.AddMvc();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            DBConnection.UsersSystem = Configuration.GetConnectionString("UserDB");

            loggerFactory.AddNLog();
            loggerFactory.ConfigureNLog(env.ContentRootPath + "/nlog.config");


            app.UseDeveloperExceptionPage();
            app.UseEnyimMemcached();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region Swagger API文档生成
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "用户系统 API V1 Docs");
            });
            #endregion


            app.UseAuthentication();

            app.UseMvc();
        }


    }
}
