using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shorty.Config;
using Shorty.Models;
using Shorty.Services;
using Shorty.Services.Impl;
using Shorty.Services.Impl.LinkIdGeneratorService;
using Shorty.Services.Impl.LinksNormalizationService;
using Shorty.Services.Impl.LinksService;

namespace Shorty
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
            DbSettings = configuration.GetSection("DBSettings").Get<DBSettings>();
        }

        private IConfiguration Configuration { get; }

        private IWebHostEnvironment Env { get; }
        
        public DBSettings DbSettings { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services
                .AddControllersWithViews()
                .AddApplicationPart(typeof(Startup).Assembly);

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            // Entity Framework (https://github.com/MaratBR/ASP_SampleAPI_NoIdentity/blob/master/ASP_SampleAPI_NoIdentity/Startup.cs#L32)
            services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = Configuration.GetConnectionString("Default");
                if (DbSettings.Type == DBSettings.DBType.Sqlite)
                    options.UseSqlite(connectionString);
                else if (DbSettings.Type == DBSettings.DBType.Mssql)
                    options.UseSqlServer(connectionString);
                else if (DbSettings.Type == DBSettings.DBType.InMemory)
                    options.UseInMemoryDatabase("Shorty");
                else
                    throw new NotImplementedException();
            });

            services.AddSingleton(
                Configuration.GetSection("SharedConfiguration").Get<SharedConfiguration>()
                );

            services.AddScoped<ILinksService, EntityLinksService>();
            //services.AddScoped<ILinkIdGeneratorService, RngIdGeneratorService>();
            services.AddScoped<ILinkIdGeneratorService, HashIdGeneratorService>();
            
            // ???????????????????????? url
            services.AddSingleton<ILinksNormalizationService>(new UriBuilderNormalizationService());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
