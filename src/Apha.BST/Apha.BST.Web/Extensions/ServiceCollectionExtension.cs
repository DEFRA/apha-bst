using Apha.BST.Application.Interfaces;
using Apha.BST.Application.Services;
using Apha.BST.Core.Interfaces;
using Apha.BST.DataAccess.Repositories;

namespace Apha.BST.Web.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddServices();
            services.AddRepositories();
            return services;
        }
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Add your application services here
            services.AddScoped<IPersonsService, PersonsService>();
            services.AddScoped<ISiteService, SiteService>();
            services.AddScoped<IAuditLogService, AuditLogService > ();
            services.AddScoped<ITrainingService, TrainingService>();
            return services;
        }
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Add your data access services here
            services.AddScoped<IPersonsRepository, PersonsRepository>();
            services.AddScoped<ISiteRepository, SiteRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<ITrainingRepository, TrainingRepository>();

            return services;
        }
    }
}
