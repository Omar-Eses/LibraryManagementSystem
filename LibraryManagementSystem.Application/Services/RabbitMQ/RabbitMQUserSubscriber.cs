using System.Text;
using AutoMapper;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Services.Commands.UserCommandsHandlers;
using LibraryManagementSystem.Domain.Helpers;
using LibraryManagementSystem.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LibraryManagementSystem.Application.Services.RabbitMQ;

public class RabbitMQUserSubscriber<T> : IRabbitMQUserSubscriber<T>
{
    private IConnection _connection;
    private IModel _channel;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;
    private readonly IRedisCacheService _redisCacheService;
    private readonly IConfiguration _configuration;

    public RabbitMQUserSubscriber(IServiceProvider serviceProvider, IMapper mapper,
        IRedisCacheService redisCacheService, IConfiguration configuration)
    {

        _serviceProvider = serviceProvider;
        _mapper = mapper;
        _redisCacheService = redisCacheService;
        _configuration = configuration;
    }

    public async Task ConsumeMessageFromQueueAsync()
    {
        var rabbit = _configuration.GetSection("RabbitMQSettings");
        var factory = new ConnectionFactory()
        {
            HostName = rabbit["HostName"],
            Port = int.Parse(rabbit["Port"]),
            UserName = rabbit["UserName"],
            Password = rabbit["Password"]
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel(); // create model

        _channel.QueueDeclare(queue: CommonVariables.userQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
        _channel.QueueBind(queue: CommonVariables.userQueue,
            exchange: CommonVariables.exchangeName,
            routingKey: CommonVariables.routingKey
        );
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var messageJson = Encoding.UTF8.GetString(body);
            await ProcessMessageAsync(messageJson);
            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };
        _channel.BasicConsume(queue: CommonVariables.userQueue, autoAck: false, consumer: consumer);
    }

    public async Task ProcessMessageAsync(string message)
    {
        // TODO : separate the logic from the common service

        using var scope = _serviceProvider.CreateScope();
        if (string.IsNullOrEmpty(message)) return;
        Console.WriteLine(message);
        var dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();

        var userMessage = JsonConvert.DeserializeObject<User>(message);
        // replace cw with logging into a file

        if (userMessage is null) return;
        if (userMessage.Id != 0 && userMessage.Username == null)
        {
            var deleteUserCommand = _mapper.Map<DeleteUserCommand>(userMessage);
            var userToDelete = await dispatcher.Dispatch<DeleteUserCommand, User>(deleteUserCommand);
            Console.WriteLine($"User deleted: {userToDelete.Username}");
            await _redisCacheService.RemoveCacheDataAsync($"LibraryCacheUser_{userToDelete.Id}");
        }
        else if (userMessage.Id != 0 && userMessage.Username != null)
        {
            var updateUserCommand = _mapper.Map<UpdateUserCommand>(userMessage);
            var userToUpdate = await dispatcher.Dispatch<UpdateUserCommand, User>(updateUserCommand);
            Console.WriteLine($"User updated: {userToUpdate.Username}");
            await _redisCacheService.UpdateCacheDataAsync("LibraryCacheUser_{userToUpdate.Id}", userToUpdate);
        }
        else if (userMessage.Id == 0)
        {
            var createUserCommand = _mapper.Map<CreateUserCommand>(userMessage);
            var userToCreate = await dispatcher.Dispatch<CreateUserCommand, User>(createUserCommand);
            Console.WriteLine($"User created: {userToCreate.Username}");
            await _redisCacheService.SetCacheDataAsync(key: $"LibraryCacheUser_{userToCreate.Id}", data: userToCreate);
        }
    }
}