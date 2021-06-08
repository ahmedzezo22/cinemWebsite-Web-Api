using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cinemaApp.Api.Admin;
using cinemaApp.Api.Data;
using cinemaApp.Api.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using cinemaApp.Api.HomeRepository;

namespace cinemaApp.Api
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
            });
            services.AddDbContext<DataContext>(op=>{
                op.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddIdentity<AppUser,AppRole>(op=>{
                op.Password.RequireDigit=false;
                op.Password.RequireLowercase=false;
                op.Password.RequiredLength=6;
                op.Password.RequiredUniqueChars=0;
                op.Password.RequireUppercase=false;
                op.Password.RequireNonAlphanumeric=false;
                op.SignIn.RequireConfirmedEmail=true;
                op.Lockout.MaxFailedAccessAttempts=5;
                op.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromMinutes(10);
            })
            .AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();
            //Add cookies Authentication
             services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
           .AddCookie(options =>
           {
               options.Cookie.HttpOnly = true;
               options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
               options.SlidingExpiration = true;
               options.LogoutPath = "/Account/Logout";
               options.Cookie.SameSite = SameSiteMode.Lax;
               options.Cookie.IsEssential = true;
           });
             services.AddCors();
            services.AddControllers().AddNewtonsoftJson(options =>
           options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddTransient<IAdminRepository,AdminRepo>();
            services.AddTransient<IHomeRepo,HomeRepo>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "cinemaApp.Api", Version = "v1" });
            });
       
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "cinemaApp.Api v1"));
            }
        // app.UseCors(Global.CORS_ALLOW_ALL_POLICY_NAME);
            
             app.UseHttpsRedirection();
             app.UseCors(x => x.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials()
                    );
            app.UseRouting();
           app.UseDefaultFiles();
            app.UseAuthentication();
            app.UseCookiePolicy();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
