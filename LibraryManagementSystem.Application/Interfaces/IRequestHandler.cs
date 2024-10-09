namespace LibraryManagementSystem.Application.Interfaces;


// represent both commands & Queries    
// Queries return response TResponse
// Commands return void or simple status response
public interface IRequest<TResponse> { }
public interface IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request);
}