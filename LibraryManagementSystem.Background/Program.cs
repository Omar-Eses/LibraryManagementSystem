using LibraryManagementSystem.Application;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Application.Services.RabbitMQ;
using LibraryManagementSystem.Background;
using LibraryManagementSystem.Domain.Models;
using LibraryManagementSystem.Infrastructure.Data;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureLmsApplication(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IDispatcher, Dispatcher>();
builder.Services.AddSingleton<IRabbitMQUserSubscriber<User>, RabbitMQUserSubscriber<User>>();
builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

var redisCacheSettings = builder.Configuration.GetSection("RedisCacheSettings");
builder.Services.AddStackExchangeRedisCache(
  options =>
  {
      options.Configuration = redisCacheSettings["ConnectionString"];
      options.InstanceName = redisCacheSettings["InstanceName"];
  }
);

//check this 
builder.Services.AddSingleton<IConnection>(sp =>
{
    var connectionFactory = sp.GetRequiredService<IConnectionFactory>();
    return connectionFactory.CreateConnection();
});
builder.Services.AddSingleton<IModel>(sp =>
{
    var connection = sp.GetRequiredService<IConnection>();
    return connection.CreateModel();
});
builder.Services.AddHostedService<UserWorker>();

var host = builder.Build();
host.Run();
