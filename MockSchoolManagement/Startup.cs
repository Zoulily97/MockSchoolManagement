using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MockSchoolManagement.DataRepositories;
using MockSchoolManagement.Infrastructure;
using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement
{
    public class Startup
    {
        private IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //策略结合声明授权
            services.AddAuthorization(option =>
            {
                option.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role"));
                option.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));
                //策略结合多个角色进行授权
                option.AddPolicy("SuperAdminPolicy", policy => policy.RequireRole("Admin", "User", "SupperManager"));
                option.AddPolicy("EditRolePolicy", policy => policy.RequireClaim("Edit Role","true"));
            }
            );

            //         services.AddAuthorization(options =>
            //         {
            //             options.AddPolicy("EditRolePolicy", 
            //                 policy => policy.RequireAssertion(context =>
            //                 context.User.IsInRole("Admin") &&
            //                 context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
            //                 context.User.IsInRole("Super Admin")
            //));
            //         });

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("EditRolePolicy", 
            //        policy => policy.RequireClaim("Edit Role","true"));
            //});


            services.AddDbContextPool<AppDbContext>(
                options => options.UseSqlServer(_configuration.GetConnectionString("MockStudentDBConnection")));
            //全局使用authorize授权
            services.AddControllersWithViews(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                              .RequireAuthenticatedUser()
                                              .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            }).AddXmlSerializerFormatters();

            services.AddScoped<IStudentRepository, SQLStudentRepository>();

            services.AddIdentity<ApplicationUser, IdentityRole>()
               .AddEntityFrameworkStores<AppDbContext>();
            services.Configure<IdentityOptions>(
                options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequiredUniqueChars = 3;
                    options.Password.RequireUppercase = false;

                });



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //否则显示友好的错误页面
            else if (env.IsStaging() || env.IsProduction() || env.IsEnvironment("UAT"))
            {
                app.UseExceptionHandler("/Error");
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            //身份验证中间件
            app.UseAuthentication();

            app.UseRouting();

            //授权中间件
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });


        }

        //授权访问
        private bool AuthorizeAccess(AuthorizationHandlerContext context)
        {
            return context.User.IsInRole("Admin") &&
                    context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
                    context.User.IsInRole("Super Admin");
        }
    }
}
