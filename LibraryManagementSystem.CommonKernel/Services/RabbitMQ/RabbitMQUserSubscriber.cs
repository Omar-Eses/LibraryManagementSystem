using AutoMapper;
using LibraryManagementSystem.CommonKernel.Interfaces;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Services.Commands.UserCommandsHandlers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace LibraryManagementSystem.CommonKernel.Services;

public class RabbitMQUserSubscriber<T> : IRabbitMQUserSubscriber<T>
{
    private IConnection _connection;
    private IModel _channel;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;


    public RabbitMQUserSubscriber(IServiceProvider serviceProvider, IMapper mapper)
    {
        _serviceProvider = serviceProvider;
        _mapper = mapper;
    }

    public async Task ConsumeMessageFromQueueAsync()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest",
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel(); // create model

        Console.WriteLine("[*] waiting for logs");
        Console.WriteLine(_channel.ToString());
        // => QueueDeclare()
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
        using (var scope = _serviceProvider.CreateScope())
        {
            if (message == null) return;
            Console.WriteLine(message);
            var dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();

            var userMessage = JsonConvert.DeserializeObject<User>(message);

            if (string.IsNullOrEmpty(message))
                return;
            if (userMessage.Id != 0 && userMessage.Username == null)
            {
                var deleteUserCommand = _mapper.Map<DeleteUserCommand>(userMessage);
                var deletedUser = await dispatcher.Dispatch<DeleteUserCommand, User>(deleteUserCommand);
                Console.WriteLine($"User deleted: {deletedUser.Username}");
            }
            else if (userMessage.Id != 0 && userMessage.Username != null)
            {
                var updateUserCommand = _mapper.Map<UpdateUserCommand>(userMessage);
                var updatedUser = await dispatcher.Dispatch<UpdateUserCommand, User>(updateUserCommand);
                Console.WriteLine($"User updated: {updatedUser.Username}");
            }
            else if (userMessage.Id == 0)
            {
                CreateUserCommand userToDelete = JsonConvert.DeserializeObject<CreateUserCommand>(message);
                var createUserCommand = _mapper.Map<CreateUserCommand>(userMessage);
                var createdUser = await dispatcher.Dispatch<CreateUserCommand, User>(createUserCommand);
                Console.WriteLine($"User created: {createdUser.Username}");
            }
        }
    }
}
