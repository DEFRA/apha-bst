using Apha.BST.Application.Mappings;
using Apha.BST.DataAccess.Data;
using Apha.BST.Web.Mappings;
using Apha.BST.Web.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

namespace Apha.BST.Web.Extensions
{
    public static class ProgramExtension
    {
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;

            // Database
            services.AddDbContext<BstContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("BSTConnectionString")
                ?? throw new InvalidOperationException("Database Connection string 'BSTConnectionString' not found.")));

            // AutoMapper
            services.AddAutoMapper(typeof(EntityMapper).Assembly);
            services.AddAutoMapper(typeof(ViewModelMapper));

            // MVC
            services.AddControllersWithViews();

            // Application services
            services.AddApplicationServices();

            // Authentication
            services.AddAuthenticationServices(configuration);

            // HTTP Context
            services.AddHttpContextAccessor();

            // Health checks
            services.AddHealthChecks();
        }

        public static void ConfigureMiddleware(this WebApplication app)
        {
            // Health checks endpoint
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => false
            });

            //// Error handling            
            app.UseExceptionHandler("/Error");
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

           

            // Default route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }
    }
}