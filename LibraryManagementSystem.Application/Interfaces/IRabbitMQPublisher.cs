namespace LibraryManagementSystem.Application.Interfaces;
public interface IRabbitMQPublisher<T>
{
    public Task PublishMessageToQueueAsync(T message);
}