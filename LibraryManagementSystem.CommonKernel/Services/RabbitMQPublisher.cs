﻿using LibraryManagementSystem.CommonKernel.Interfaces;
using LibraryManagementSystem.Helpers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace LibraryManagementSystem.CommonKernel.Services;

public class RabbitMQPublisher<T> : IRabbitMQPublisher<T>
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQPublisher()
    {
        // connection creation & configuration
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };
        // new ssl option
        factory.Ssl = new SslOption
        {
            Enabled = false,
            ServerName = factory.HostName,

        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: CommonVariables.exchangeName, type: ExchangeType.Direct, durable: true);
        _channel.QueueDeclare(queue: CommonVariables.queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
        _channel.QueueBind(queue: CommonVariables.queueName, exchange: CommonVariables.exchangeName, routingKey: CommonVariables.routingKey);
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
                _channel.BasicPublish(exchange: CommonVariables.exchangeName,
                    routingKey: CommonVariables.routingKey,
                    basicProperties: null, body: body
                );
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
