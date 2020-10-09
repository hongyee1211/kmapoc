using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using CognitiveSearch.UI.Configuration;
using CognitiveSearch.UI.Infrastructure;
using CognitiveSearch.UI.Services.ARM;
using CognitiveSearch.UI.Services.GraphOperations;
using CognitiveSearch.UI.DAL;
using Microsoft.EntityFrameworkCore;

namespace CognitiveSearch.UI
{
    public class Startup
    {
        public static class DBConnStr
        {
            public static string connStr { get; set; }
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //to comment out if using connection string stored in appsettings.json
            DBConnStr.connStr = configuration["sqlDBConnStr"].ToString();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;

                // Handling SameSite cookie according to https://docs.microsoft.com/en-us/aspnet/core/security/samesite?view=aspnetcore-3.1
                options.HandleSameSiteCookieCompatibility();
            });

            //if using connection string from KeyVault
            services.AddDbContext<FeedbackContext>(options => options.UseSqlServer(DBConnStr.connStr, sqlServerOptions => sqlServerOptions.CommandTimeout(60)));
            services.AddDbContext<SubscribeContext>(options => options.UseSqlServer(DBConnStr.connStr, sqlServerOptions => sqlServerOptions.CommandTimeout(60)));
            services.AddDbContext<StandardContext>(options => options.UseSqlServer(DBConnStr.connStr, sqlServerOptions => sqlServerOptions.CommandTimeout(60)));
            //services.AddDbContext<FeedbackContext>(options => options.UseSqlServer(DBConnStr.connStr, sqlServerOptionsAction => sqlServerOptionsAction.CommandTimeout(60)));

            //if using connection string stored in appsettings.json
            //services.AddDbContext<FeedbackContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), sqlServerOptions => sqlServerOptions.CommandTimeout(60)));
            //services.AddDbContext<SubscribeContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), sqlServerOptions => sqlServerOptions.CommandTimeout(60)));
            //services.AddDbContext<StandardContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), sqlServerOptions => sqlServerOptions.CommandTimeout(60)));

            services.AddOptions();

            // Sign-in users with the Microsoft identity platform
            services.AddMicrosoftIdentityWebAppAuthentication(Configuration)
                    .EnableTokenAcquisitionToCallDownstreamApi(new string[] { Constants.ScopeUserRead })
                    .AddInMemoryTokenCaches();

            // Add APIs
            services.AddGraphService(Configuration);
            //services.AddMSGraphService(Configuration);
            services.AddHttpClient<IArmOperations, ArmOperationService>();

            //services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options => {
                // The following code instructs the ASP.NET Core middleware to use the data in the "groups" claim in the [Authorize] attribute and for User.IsInRole()
                // See https://docs.microsoft.com/en-us/aspnet/core/security/authorization/roles for more info.
            //    options.TokenValidationParameters.RoleClaimType = "groups";
            //});

            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddMicrosoftIdentityUI();
            services.AddRazorPages();

            var apiConfig = new ApiConfig
            {
                Url = "/api"
            };
            services.AddSingleton(apiConfig);

            var orgConfig = new OrganizationConfig
            {
                Name = Configuration["OrganizationName"],
            };
            services.AddSingleton(orgConfig);

            var appConfig = new AppConfig
            {
                ApiConfig = apiConfig,
                Organization = orgConfig,
                Customizable = Configuration["Customizable"].Equals("true", StringComparison.InvariantCultureIgnoreCase)
            };
            services.AddSingleton(appConfig);

            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            services.AddMvc(options => options.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            app.UseMvcWithDefaultRoute();
        }
    }
}