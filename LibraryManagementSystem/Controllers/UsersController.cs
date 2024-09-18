using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Commands.UserCommandsHandlers;
using LibraryManagementSystem.Services.Queries;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IDispatcher dispatcher) : ControllerBase
    {
        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserCommand command)
        {
            var user = await dispatcher.Dispatch<CreateUserCommand, User>(command);
            return Ok(user);
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var query = new GetAllUsersQuery();
            var users = await dispatcher.Dispatch<GetAllUsersQuery, IEnumerable<User>>(query);
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
        public async Task<IActionResult> PutUser(long id, UpdateUserCommand command)
        {
            if (id != command.Id) return BadRequest("User ID mismatch.");
            var result = await GetUserIfExists(id);
            if (result is NotFoundResult) return result;

            await dispatcher.Dispatch<UpdateUserCommand, User>(command);

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var result = await GetUserIfExists(id);
            if (result is NotFoundResult) return result;

            var command = new DeleteUserCommand { UserId = id };
            await dispatcher.Dispatch<DeleteUserCommand, User>(command);

            return NoContent();
        }

        private async Task<IActionResult> GetUserIfExists(long id)
        {
            var user = await dispatcher.Dispatch<GetUserByIdQuery, User>(new GetUserByIdQuery { Id = id });
            if (user == null) return NotFound();

            return Ok(user); // Return the user object if found
        }
    }
}