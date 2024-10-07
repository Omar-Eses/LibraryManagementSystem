using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Application.Services.Commands.UserCommandsHandlers;
using LibraryManagementSystem.Application.Services.Queries;
using LibraryManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    private readonly IRabbitMQPublisher<UpdateUserCommand> _rabbitMQUpdatePublisher;
    private readonly IRabbitMQPublisher<DeleteUserCommand> _rabbitMQDeletePublisher;
    private readonly ILogger<UsersController> _logger;
    public UsersController(
    IDispatcher dispatcher,
    IRabbitMQPublisher<UpdateUserCommand> rabbitMQUpdatePublisher,
    IRabbitMQPublisher<DeleteUserCommand> rabbitMQDeletePublisher,
    ILogger<UsersController> logger)
    {
        _dispatcher = dispatcher;
        _rabbitMQUpdatePublisher = rabbitMQUpdatePublisher;
        _rabbitMQDeletePublisher = rabbitMQDeletePublisher;
        _logger = logger;
    }
    // GET: api/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        _logger.LogInformation("Getting all users Request");
        var query = new GetAllUsersQuery();
        var users = await _dispatcher.Dispatch<GetAllUsersQuery, IEnumerable<User>>(query);
        return Ok(users);
    }


    // GET: api/Users/5
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetUser(long id)
    {
        _logger.LogInformation($"Getting user with {id} Request");
        var result = await GetUserIfExists(id);
        return result is NotFoundResult ? result : Ok(result);
    }

    // PUT: api/Users/5
    [HttpPut("{id:long}")]
    public async Task<IActionResult> PutUser(UpdateUserCommand command)
    {
        _logger.LogInformation($"Updating user with {command.Id} Request");
        var result = await GetUserIfExists(command.Id);
        if (result is NotFoundResult) return result;

        await _rabbitMQUpdatePublisher.PublishMessageToQueueAsync(command);
        return NoContent();
    }

    // DELETE: api/Users/5
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteUser(long id)
    {
        _logger.LogInformation($"Deleting user with {id} Request");
        var result = await GetUserIfExists(id);
        if (result is NotFoundResult) return result;

        var command = new DeleteUserCommand { Id = id };
        await _rabbitMQDeletePublisher.PublishMessageToQueueAsync(command);
        return NoContent();
    }

    private async Task<IActionResult> GetUserIfExists(long id)
    {
        var user = await _dispatcher.Dispatch<GetUserByIdQuery, User>(new GetUserByIdQuery { Id = id });
        if (user == null) return NotFound();

        return Ok(user); // Return the user object if found
    }
}