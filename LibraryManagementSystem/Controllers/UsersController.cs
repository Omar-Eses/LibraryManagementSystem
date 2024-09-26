using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Commands.UserCommandsHandlers;
using LibraryManagementSystem.Services.Queries;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.CommonKernel.Interfaces;

namespace LibraryManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;
        private readonly IRabbitMQPublisher<UpdateUserCommand> _rabbitMQUpdatePublisher;
        private readonly IRabbitMQPublisher<DeleteUserCommand> _rabbitMQDeletePublisher;
        public UsersController(
        IDispatcher dispatcher,
        IRabbitMQPublisher<UpdateUserCommand> rabbitMQUpdatePublisher,
        IRabbitMQPublisher<DeleteUserCommand> rabbitMQDeletePublisher)
        {
            _dispatcher = dispatcher;
            _rabbitMQUpdatePublisher = rabbitMQUpdatePublisher;
            _rabbitMQDeletePublisher = rabbitMQDeletePublisher;
        }
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var query = new GetAllUsersQuery();
            var users = await _dispatcher.Dispatch<GetAllUsersQuery, IEnumerable<User>>(query);
            return Ok(users);
        }


        // GET: api/Users/5
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetUser(long id)
        {
            var result = await GetUserIfExists(id);
            return result is NotFoundResult ? result : Ok(result);
        }

        // PUT: api/Users/5
        [HttpPut("{id:long}")]
        public async Task<IActionResult> PutUser(UpdateUserCommand command)
        {
            var result = await GetUserIfExists(command.Id);
            if (result is NotFoundResult) return result;

            await _rabbitMQUpdatePublisher.PublishMessageToQueueAsync(command);
            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
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
}