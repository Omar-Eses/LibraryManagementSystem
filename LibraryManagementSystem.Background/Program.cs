using LibraryManagementSystem.Background;
using LibraryManagementSystem.CommonKernel;
using LibraryManagementSystem.CommonKernel.Interfaces;
using LibraryManagementSystem.CommonKernel.Services;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddLibraryManagementSystemModule(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IDispatcher, Dispatcher>();
builder.Services.AddSingleton<IRabbitMQUserSubscriber<User>, RabbitMQUserSubscriber<User>>();
builder.Services.AddStackExchangeRedisCache(
   options =>
   {

       options.Configuration = "localhost:6379";
       options.InstanceName = "LibraryCache";
   }
);
builder.Services.AddSingleton<IConnectionFactory>(sp =>
new ConnectionFactory
{
    HostName = "localhost",
    Port = 5672,
    UserName = "guest",
    Password = "guest",
});
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
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
