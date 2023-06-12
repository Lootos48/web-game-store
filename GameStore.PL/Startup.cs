using Autofac;
using GameStore.BLL.MappingProfiles;
using GameStore.DAL.Context;
using GameStore.DAL.MappingProfiles;
using GameStore.PL.Configurations;
using GameStore.PL.DependencyInjections;
using GameStore.PL.Extensions;
using GameStore.PL.HostedServices;
using GameStore.PL.MappingProfiles;
using GameStore.PL.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace GameStore.PL
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IServiceCollection Services { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = Configuration.Get<ApplicationSettings>();

            services.AddSingleton(appSettings.DownloadingFileSettings);
            services.AddSingleton(appSettings.MongoIntegrationSettings);
            services.AddSingleton(appSettings.GuestCookieSettings);

            services.Configure<ApplicationSettings>(Configuration);

            services
                .AddMongoDb(appSettings.MongoDbSettings)
                .ConfigureNorthwindDatabase();

            services
                .AddDbContext<GameStoreContext>(options => options.UseSqlServer(appSettings.ConnectionStrings.GameStoreSQLEXPRESS))
                .AddScoped<DbContext, GameStoreContext>();

            services.AddAutoMapper(
                typeof(EntityDomainMapperProfile), 
                typeof(PresentationLayerMapperProfile), 
                typeof(BusinessMappingProfile));

            services
                .AddHostedService<LaunchCacheService>()
                .AddHostedService<PaymentTimeOutService>();

            services
                .AddMemoryCache()
                .AddResponseCaching();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/User/Login");
                    options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/User/access-denied");
                });

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.SetDefaultCulture("en-US");

                options.AddSupportedCultures("uk-UA", "ru-RU", "en-US");
                options.AddSupportedUICultures("uk-UA", "ru-RU", "en-US");
                options.FallBackToParentUICultures = true;

                options
                    .RequestCultureProviders
                    .Remove(typeof(AcceptLanguageHeaderRequestCultureProvider));
            });

            services.AddMvc()
                .AddRazorRuntimeCompilation()
                .AddDataAnnotationsLocalization()
                .AddViewLocalization()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            Services = services;
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            Assembly[] gameStoreAssemblies = GameStoreAssembliesCollector.CollectAssemblies();
            DependencyInjectionConfigurator.RegisterDependencies(builder, gameStoreAssemblies, Services);
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
                app.UseExceptionHandler("/Shared/Errors/Error");
                app.UseHsts();
            }

            app.UseRequestLocalization();

            app
                .UseMiddleware<RequestLocalizationCookiesMiddleware>()
                .UseMiddleware<RequestLoggingMiddleware>()
                .UseMiddleware<ErrorHandlingMiddleware>();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseResponseCaching();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                       name: "default",
                       pattern: "{controller=Games}/{action=GetAllGames}/{id?}");
            });
        }
    }
}
