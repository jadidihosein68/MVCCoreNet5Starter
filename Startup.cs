using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MVCCoreNet5Starter.Models;
using MVCCoreNet5Starter.Models.Constants;

namespace MVCCoreNet5Starter
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _appSettings = this.Configuration.Get<AppSettings>();
        }
        public AppSettings _appSettings { get; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();


            
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";

            }).AddCookie("Cookies")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "Cookies";
                    options.MetadataAddress = _appSettings.openIDSettings.ADFSDiscoveryDoc;
                    options.RequireHttpsMetadata = false;
                    options.ClientId = _appSettings.openIDSettings.ClientId;
                    options.ClientSecret = _appSettings.openIDSettings.ClientSecret;
                    options.SaveTokens = true;
                    options.SignedOutRedirectUri = _appSettings.openIDSettings.PostredirectURI;
                    options.ResponseMode = OpenIDConst.ResponseMode;
                    options.ResponseType = OpenIDConst.ResponseType;
                    options.Scope.Add(OpenIDConst.Scopeopenid);
                    options.Scope.Add(OpenIDConst.Scoperoles);
                    options.UseTokenLifetime = false;
                });
            

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
