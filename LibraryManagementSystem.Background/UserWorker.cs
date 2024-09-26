using LibraryManagementSystem.CommonKernel.Interfaces;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Background
{
    public class UserWorker : BackgroundService
    {
        private readonly ILogger<UserWorker> _logger;
        private readonly IRabbitMQUserSubscriber<User> _rabbitMQSubscriber;
        public UserWorker(ILogger<UserWorker> logger, IRabbitMQUserSubscriber<User> rabbitMQSubscriber)
        {
            _logger = logger;
            _rabbitMQSubscriber = rabbitMQSubscriber;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _rabbitMQSubscriber.ConsumeMessageFromQueueAsync();
            }
        }
    }
}
