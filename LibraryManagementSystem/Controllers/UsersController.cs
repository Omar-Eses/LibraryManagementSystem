using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Commands.UserCommandsHandlers;
using LibraryManagementSystem.Services.Queries;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IDispatcher dipatcher) : ControllerBase
    {
        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserCommand command)
        {
            var user = await dipatcher.Dispatch<CreateUserCommand, User>(command);
            return Ok(user);
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var query = new GetAllUsersQuery();
            var users = await dipatcher.Dispatch<GetAllUsersQuery, IEnumerable<User>>(query);
            return Ok(users);
        }


        // GET: api/Users/5
        [HttpGet("{id:long}")]
        public async Task<ActionResult<User>> GetUser(long id)
        {
            var query = new GetUserByIdQuery { Id = id };
            var user = await dipatcher.Dispatch<GetUserByIdQuery, User>(query);

            if (user == null) return NotFound();

            return Ok(user);
        }

        // PUT: api/Users/5
        [HttpPut("{id:long}")]
        public async Task<IActionResult> PutUser(long id, UpdateUserCommand command)
        {
            if (id != command.Id) return BadRequest("User ID mismatch.");

            await dipatcher.Dispatch<UpdateUserCommand, User>(command);

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var command = new DeleteUserCommand { UserId = id };
            await dipatcher.Dispatch<DeleteUserCommand, User>(command);

            return NoContent();
        }

        private async Task<IActionResult> UsersExists(long id)
        {
            var user = await dipatcher.Dispatch<GetUserByIdQuery, User>(new GetUserByIdQuery { Id = id });
            if (user == null) return NotFound();

            return Ok(user);
        }
    }
}