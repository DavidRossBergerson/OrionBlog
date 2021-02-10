using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrionBlog.Data;
using OrionBlog.Models;
using OrionBlog.Services;
using OrionBlog.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrionBlog
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
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(DataUtility.GetConnectionString(Configuration)));
            services.AddDatabaseDeveloperPageExceptionFilter();

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddIdentity<BlogUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddDefaultUI() 
                .AddDefaultTokenProviders()
               .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddControllersWithViews();
            services.AddRazorPages();

            //This is how I register a custom class as a service
            services.AddTransient<ISlugService, BasicSlugService>();

            //Register our new BasicImageService
            services.AddTransient<IImageService, BasicImageService>();

           


            //Services needed to send emails
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.AddScoped<IEmailSender, EmailService>();

            //Authentication using 3rd parties

            services.AddAuthentication()
               .AddGitHub(options =>
               {
                   options.ClientId = "d9c638d25a053950e4c7";
                   options.ClientSecret = "0eded162399e542d48c9c57bc1ce63b5323137db";
                   options.AccessDeniedPath = "/AccessDeniedPathInfo";
               })
                //.AddFacebook(options =>
                //{
                //    options.AppId = "";
                //    options.AppSecret = "";
                //    options.AccessDeniedPath = "/AccessDeniedPathInfo";
                //})
               .AddGoogle(options =>
               {
                   options.ClientId = "74897765760-g8llrhic7d2b8cc8jve0q89palhabp8n.apps.googleusercontent.com";
                   options.ClientSecret = "aze_wdJxLfuOCo5Xm4Y8HeBY";
                   options.AccessDeniedPath = "/AccessDeniedPathInfo";
               });
               

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
                name: "slugRoute",
                pattern: "BlogPost/TheDetails/{slug}",
                defaults: new { controller = "CategoryPosts", action = "Details" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
