using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Services.Commands.UserCommandsHandlers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace LibraryManagementSystem.CommonKernel.Services;

public class RabbitMQSubscriber<T>
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IDispatcher _dispatcher;

    public RabbitMQSubscriber(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;

        // connection creation & configuration
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest",
            VirtualHost = "/",
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: CommonVariables.queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
    }


    public void ConsumeMessageFromQueue()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            await ProcessMessageAsync(message);

            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };
        _channel.BasicConsume(queue: CommonVariables.queueName,
            autoAck: false,
            consumer: consumer
        );
    }
    private async Task ProcessMessageAsync(string message)
    {
        var jsonObject = JsonConvert.DeserializeObject<dynamic>(message);
        string commandType = jsonObject["Type"].ToString();
        switch (commandType)
        {
            case nameof(CreateUserCommand):
                Console.WriteLine("Processing CreateUserCommand");
                var createUserCommand = JsonConvert.DeserializeObject<CreateUserCommand>(message);
                await _dispatcher.Dispatch<CreateUserCommand, User>(createUserCommand);
                break;

            case nameof(UpdateUserCommand):
                Console.WriteLine("Processing UpdateUserCommand");
                var updateUserCommand = JsonConvert.DeserializeObject<UpdateUserCommand>(message);
                await _dispatcher.Dispatch<UpdateUserCommand, User>(updateUserCommand);
                break;

            case nameof(DeleteUserCommand):
                Console.WriteLine("Processing DeleteUserCommand");
                var deleteUserCommand = JsonConvert.DeserializeObject<DeleteUserCommand>(message);
                await _dispatcher.Dispatch<DeleteUserCommand, User>(deleteUserCommand);
                break;

            default:
                // Handle unknown command type
                Console.WriteLine($"Unknown command type: {commandType}");
                break;
        }
    }
}
