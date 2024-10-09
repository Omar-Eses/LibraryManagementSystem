using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Application.Services.Commands.UserCommandsHandlers;
using LibraryManagementSystem.Application.Services.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using LibraryManagementSystem.Application.Services.Redis;
using LibraryManagementSystem.Domain.Models;
using RabbitMQ.Client;

namespace LibraryManagementSystem.Application;

public static class LmsApplicationModule
{
    public static IServiceCollection ConfigureLmsApplication(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IDispatcher, Dispatcher>();
        services.AddRequestHandlers(Assembly.GetExecutingAssembly());

        var redisSettings = configuration.GetSection("RedisCacheSettings");
        var rabbitMqSettings = new RabbitMQSettings();
        services.AddStackExchangeRedisCache(
           options =>
           {
               options.Configuration = redisSettings["ConnectionString"];
               options.InstanceName = redisSettings["InstanceName"];
           }
        );
        var connectionFactory = new ConnectionFactory();
        configuration.GetSection("RabbitMQSettings").Bind(connectionFactory);
        connectionFactory.Ssl = new SslOption
        {
            Enabled = false,
            ServerName = connectionFactory.HostName,

        };
        services.AddSingleton<IRedisCacheService, RedisCacheService>();
        services.AddScoped<IRabbitMQPublisher<CreateUserCommand>, RabbitMqPublisher<CreateUserCommand>>();
        services.AddScoped<IRabbitMQPublisher<DeleteUserCommand>, RabbitMqPublisher<DeleteUserCommand>>();
        services.AddScoped<IRabbitMQPublisher<UpdateUserCommand>, RabbitMqPublisher<UpdateUserCommand>>();

        return services;
    }

    private static IServiceCollection AddRequestHandlers(this IServiceCollection services, Assembly assembly)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
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