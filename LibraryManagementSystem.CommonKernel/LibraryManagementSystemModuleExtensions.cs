using System.Reflection;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using LibraryManagementSystem.CommonKernel.Interfaces;
using LibraryManagementSystem.Services.Commands.UserCommandsHandlers;
using LibraryManagementSystem.CommonKernel.Services.RabbitMQ;
using LibraryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.CommonKernel.Services.Redis;

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

                options.Configuration = redisSettings["ConnectionString"];
                options.InstanceName = redisSettings["InstanceName"];
            }
        );
        var connectionString = "server=localhost;database=LibraryManagementSystemContext;username=postgres;password=sqladmin123!@#";
        services.AddDbContext<LMSContext>(opt =>
            opt.UseNpgsql(
                connectionString ?? throw new InvalidOperationException("Connection string 'LibraryManagementSystemContext' not found.")
            )
        );
        services.AddSingleton<IRedisCacheService, RedisCacheService>();

        Console.WriteLine(@$"CS: {redisSettings["ConnectionString"]} - IN: {redisSettings["InstanceName"]}");
        services.AddScoped<IRabbitMQPublisher<CreateUserCommand>, RabbitMQPublisher<CreateUserCommand>>();
        services.AddScoped<IRabbitMQPublisher<DeleteUserCommand>, RabbitMQPublisher<DeleteUserCommand>>();
        services.AddScoped<IRabbitMQPublisher<UpdateUserCommand>, RabbitMQPublisher<UpdateUserCommand>>();
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
