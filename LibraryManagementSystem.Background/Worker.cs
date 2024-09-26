using LibraryManagementSystem.CommonKernel.Interfaces;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Background
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IRabbitMQUserSubscriber<User> _rabbitMQSubscriber;
        public Worker(ILogger<Worker> logger, IRabbitMQUserSubscriber<User> rabbitMQSubscriber)
        {
            _logger = logger;
            _rabbitMQSubscriber = rabbitMQSubscriber;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _rabbitMQSubscriber.ConsumeMessageFromQueueAsync();
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
