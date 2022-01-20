using EmployeeManagment.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using EmployeeManagment.Security;

namespace EmployeeManagment
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }
        private bool AuthorizeAccess(AuthorizationHandlerContext context)
        {
            return context.User.IsInRole("Admin") &&
                    context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
                    context.User.IsInRole("Super Admin");
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) // Configures Services
        {
            // Setting up mvc
            // gettings from secrets.json
            services.AddDbContextPool<AppDbContext>
                (options => options.UseSqlServer(_config.GetConnectionString("EmployeeDbConnection")));
            //Identity Helps with log ins, register, etc...
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                
                options.SignIn.RequireConfirmedEmail = true;
                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

            })
                .AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders()
                .AddTokenProvider<CustomEmailConfirmationTokenProvider
                <ApplicationUser>>("CustomEmailConfirmation");// We add Identity (one from DbContext class)

            services.Configure<IdentityOptions>(options => 
            { 
                // Configuring how complex password can be
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 0;
            });

            services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromHours(5));

            services.Configure<CustomEmailConfirmationTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromDays(3));
            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddXmlSerializerFormatters();
            //services.AddControllersWithViews();
            services.AddMvc(config => config.EnableEndpointRouting = false);

            services.AddAuthentication()
                .AddGoogle(options => // To specify client id and client secret
                {
                    options.ClientId =
                    "732760462946-j42kqm8ff8o5vej49t452ifupn588ish.apps.googleusercontent.com";
                    options.ClientSecret = "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";
                }).AddFacebook(options =>
                {
                    options.AppId = "448660383415559";
                    options.AppSecret = "ee3322c4f0d1474b5605028e9e80af68";
                });
                
            
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });


            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteRolePolicy",
                    policy => policy.RequireClaim("Delete Role"));


                options.AddPolicy("EditRolePolicy", policy =>
                    policy.AddRequirements(new ManageAdminRolesAndClaimsRequirment()));



                options.AddPolicy("AdminRolePolicy",
                    policy => policy.RequireRole("Admin"));
            }); // Claims


            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
            services.AddSingleton
                <IAuthorizationHandler, CanEditOnlyOtherAdminRolesClaimsHandler>();
            services.AddSingleton
                <IAuthorizationHandler, SuperAdminHandler>();
            services.AddSingleton<DataProtectionPurposeStrings>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        // Configures our application request proccessing pipeline 
        {
            if (env.IsDevelopment())
            {  
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error"); // If theres an exception on production envirnonment 
                // Use Error Controller


                app.UseStatusCodePagesWithReExecute("/Error/{0}"); // 0 detects error
            }

            app.UseStaticFiles(); // First Static Files then UseMvc
            /* app.UseMvcWithDefaultRoute();*/ // Looks for Home Controller and Will reverse the pipeline

            //Before mvc middleware
            app.UseAuthentication(); 

            app.UseMvc(routes =>
              routes.MapRoute("default", "{controller=Home}/{action=index}/{id?}"));


          
        
        }
    }
}
