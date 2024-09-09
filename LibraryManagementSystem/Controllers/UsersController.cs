using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    // GET: api/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
    {
        return Ok(await _usersService.GetAllUsersAsync());
    }

    // GET: api/Users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Users>> GetUsers(long id) =>
        await _usersService.GetUserByIdAsync(id) != null ? Ok() : NotFound();
   

    // PUT: api/Users/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUsers(long id, Users users)
    {
        if (!await _usersService.UpdateUserAsync(id, users))
        {
            return BadRequest();
        }

        return NoContent();
    }

    // POST: api/Users
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<Users>> PostUsers(Users users)
    {
        var newUser = await _usersService.AddUserAsync(users);
        return CreatedAtAction("GetUsers", new { id = newUser.Id }, newUser);
    }

    // DELETE: api/Users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUsers(long id)
    {
        if (!await _usersService.DeleteUserAsync(id))
            return NotFound();


        return NoContent();
    }

    private IActionResult UsersExists(long id)
    {
        if (_usersService.UserExists(id))
            return Ok($"Author {id} exists");
        return NotFound();
    }
}
