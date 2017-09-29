using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.Collections.Generic;
using User.DBUtility;
using User.IdentityServer.Core;
using User.IRepository;
using User.Repository;

namespace User.IdentityServer
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

            services.AddTransient<IPersistedGrantRepositoryService, PersistedGrantRepositoryService>();
            services.AddTransient<IUserService, UserService>();

            ConfigAuth(services);


            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            DBConnection.UsersSystem = Configuration.GetConnectionString("UserDB");

            loggerFactory.AddNLog();
            loggerFactory.ConfigureNLog(env.ContentRootPath + "/nlog.config");

            app.UseDeveloperExceptionPage();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseIdentityServer();

            app.UseMvc();
        }



        private void ConfigAuth(IServiceCollection services)
        {
            var build = services.AddIdentityServer()
            .AddDeveloperSigningCredential()
             .AddInMemoryApiResources(new List<ApiResource>
             {
                   new ApiResource("User", "用户系统"),
                   new ApiResource("IOT", "IOTAPP"),
             })
             .AddInMemoryClients(new List<Client>
             {
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowOfflineAccess=true,
                    AccessTokenLifetime=3600*6,
                    SlidingRefreshTokenLifetime=1296000,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "User",
                        "IOT"
                    }
                }
             });
            build.AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();
            build.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();
        }
    }
}
