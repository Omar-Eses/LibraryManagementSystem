using LibraryManagementSystem.CommonKernel.Services;
using LibraryManagementSystem.Services.Commands.UserCommandsHandlers;
using StackExchange.Redis;

namespace LibraryManagementSystem.Background
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMQSubscriber<CreateUserCommand> _subscriber;
        public Worker(ILogger<Worker> logger, RabbitMQSubscriber<CreateUserCommand> subscriber)
        {
            _logger = logger;
            _subscriber = subscriber;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _subscriber.ConsumeMessageFromQueue();

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
