namespace LibraryManagementSystem.Application.Interfaces;

public interface IRabbitMQUserSubscriber<T>
{
    Task ConsumeMessageFromQueueAsync();
    Task ProcessMessageAsync(string message);
}
