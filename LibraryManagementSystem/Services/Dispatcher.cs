using LibraryManagementSystem.Interfaces;

namespace LibraryManagementSystem.Services;

public interface IDispatcher
{
    Task<TResponse> Dispatch<TRequest, TResponse>(TRequest request)
        where TRequest : IRequest<TResponse>?;
}

public class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    public async Task<TResponse> Dispatch<TRequest, TResponse>(TRequest request)
        where TRequest : IRequest<TResponse>
    {
        var handler = serviceProvider.GetService<IRequestHandler<TRequest, TResponse>>();
        if (handler == null) throw new Exception($"Handler for {typeof(TRequest).Name} not found.");

        return await handler.Handle(request);
    }
}