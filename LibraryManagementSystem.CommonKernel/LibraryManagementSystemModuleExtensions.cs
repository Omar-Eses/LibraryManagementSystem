using System.Reflection;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using LibraryManagementSystem.CommonKernel.Interfaces;
using LibraryManagementSystem.CommonKernel.Services;

namespace LibraryManagementSystem.CommonKernel;

public static class LibraryManagementSystemModuleExtensions
{

    public static IServiceCollection AddLibraryManagementSystemModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDispatcher, Dispatcher>();
        services.AddRequestHandlers(Assembly.GetExecutingAssembly());

        var redisSettings = configuration.GetSection("RedisCacheSettings");
        services.AddStackExchangeRedisCache(
            options =>
            {
                options.Configuration = redisSettings["ConnectionString"]; // Fix method to use indexer
                options.InstanceName = redisSettings["InstanceName"]; // Fix method to use indexer
            }
        );
        Console.WriteLine(@$"CS: {redisSettings["ConnectionString"]} - IN: {redisSettings["InstanceName"]}");
        services.AddScoped<IRedisCacheService, RedisCacheService>();

        var connectionString = configuration.GetConnectionString("LibraryManagementSystemContext");

        services.AddDbContext<LMSContext>(opt =>
           opt.UseNpgsql(
                connectionString ?? throw new InvalidOperationException("Connection string 'LibraryManagementSystemContext' not found.")
            )
        );

        return services;
    }
    public static IServiceCollection AddRequestHandlers(this IServiceCollection services, Assembly assembly)
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
