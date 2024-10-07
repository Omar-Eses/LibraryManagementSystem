using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Models;

namespace LibraryManagementSystem.Background
{
    public class UserWorker(ILogger<UserWorker> logger, IRabbitMQUserSubscriber<User> rabbitMqSubscriber)
        : BackgroundService
    {
        private readonly ILogger<UserWorker> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await rabbitMqSubscriber.ConsumeMessageFromQueueAsync();
            }
        }
    }
}