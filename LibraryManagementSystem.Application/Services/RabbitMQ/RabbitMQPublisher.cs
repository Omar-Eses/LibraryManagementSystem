using LibraryManagementSystem.Application.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using LibraryManagementSystem.Domain.Helpers;
using Microsoft.Extensions.Configuration;

namespace LibraryManagementSystem.Application.Services.RabbitMQ;

public class RabbitMqPublisher<T> : IRabbitMQPublisher<T>
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqPublisher(IConfiguration configuration)
    {
        // connection creation & configuration
        var rabbit = configuration.GetSection("RabbitMQSettings");
        var factory = new ConnectionFactory()
        {
            HostName = rabbit["HostName"],
            Port = int.Parse(rabbit["Port"]),
            UserName = rabbit["UserName"],
            Password = rabbit["Password"]
        };
        factory.Ssl = new SslOption
        {
            Enabled = false,
            ServerName = factory.HostName,

        };
        Console.WriteLine("here is host name => => => " + factory.HostName);
        Console.WriteLine("here is Port => => => " + factory.Port);
        Console.WriteLine("here is user  => => => " + factory.UserName);
        Console.WriteLine("here is pass  => => => " + factory.Password);
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: CommonVariables.userQueue,
           durable: true,
           exclusive: false,
           autoDelete: false,
           arguments: null
       );
        _channel.QueueBind(queue: CommonVariables.userQueue, exchange: CommonVariables.exchangeName, routingKey: CommonVariables.routingKey);
    }
    public async Task PublishMessageToQueueAsync(T message)
    {
        var messageJson = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(messageJson);
        // Publish the message to the queue as a task concurrently
        try
        {
            await Task.Run(() =>
            {
                _channel.BasicPublish(
                    exchange: CommonVariables.exchangeName,
                    routingKey: CommonVariables.routingKey,
                    basicProperties: null,
                    body: body
                );
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
