using System.Reflection;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace LibraryManagementSystem.CommonKernel
{
    public static class LibraryManagementSystemModuleExtensions
    {
        // TODO: AppSettings appSettings 
        // HINT: public static IServiceCollection AddDatumModule(this IServiceCollection services, Appsettings appSettings)
        public static IServiceCollection AddLibraryManagementSystemModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDispatcher, Dispatcher>();
            services.AddRequesHandlers(Assembly.GetExecutingAssembly());

            var connectionString = configuration.GetConnectionString("LibraryManagementSystemContext");


            services.AddDbContext<LMSContext>(opt =>
               opt.UseNpgsql(
                    connectionString ?? throw new InvalidOperationException("Connection string 'LibraryManagementSystemContext' not found.")
                )
            );


            return services;
        }

        //DONE: Common Method for IRequestHandler
        public static IServiceCollection AddRequesHandlers(this IServiceCollection services, Assembly assembly)
        {
            var handlerTypes = assembly.GetTypes()
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
                .ToList();

            foreach (var handlerType in handlerTypes)
            {
                var handlerInterface = handlerType.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));
                services.AddScoped(handlerInterface, handlerType);
            }
            return services;
        }
    }
}
