using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using LibraryManagementSystem.Helpers;

namespace LibraryManagementSystem.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UsersController(IUsersService usersService) : ControllerBase
{
    // GET: api/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers() => Ok(await usersService.GetAllUsersAsync());

    // GET: api/Users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUsers(long id) =>
        await usersService.GetUserByIdAsync(id) != null ? Ok() : NotFound();
   

    // PUT: api/Users/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUsers(long id, User users)
    {
        // TODO : Change the enum to be handled correctly
        var result = await usersService.UpdateUserAsync(id, users, [(long)enumPermissionTypes.CanBorrow, 2, 3]);

        if (!result) return BadRequest();
        return NoContent();
    }

    // POST: api/Users
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<User>> PostUsers(User users)
    {
        // TODO : Change the enum to be handled correctly
        var newUser = await usersService.AddUserAsync(users, [1, 2, 3]);
        return CreatedAtAction("GetUsers", new { id = newUser.Id }, newUser);
    }

    // DELETE: api/Users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUsers(long id)
    {
        var result = await usersService.DeleteUserAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    //TODO : Check the logic
    private async Task<IActionResult> UsersExists(long id)
    {
        if (await usersService.UserExistsAsync(id))
            return Ok($"Author {id} exists");
        return NotFound();
    }
}
