namespace LibraryManagementSystem.CommonKernel.Interfaces;

public interface IRabbitMQPublisher<T>
{
    public Task PublishMessageToQueueAsync(T message);
}