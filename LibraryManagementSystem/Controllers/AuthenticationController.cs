using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController(IUsersService usersService, IPermissionsServices permissionsServices) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("user-signup")]
    public async Task<IActionResult> UserSignUp([FromBody] User user)
    {
        if (await usersService.EmailExistsAsync(user.Email))
            return BadRequest("Email is already registered.");
        
        //TODO :check this
        var newUser = await usersService.AddUserAsync(user, new List<long> { 1, 2, 3, 4, 5, 6 }); // Assign default permissions

        var token = await permissionsServices.GenerateJwtToken(newUser);

        return Created("user", new { token = token });
    }

    [AllowAnonymous]
    [HttpPost("author-signup")]
    public async Task<IActionResult> AuthorSignUp([FromBody] User user)
    {
        if (await usersService.EmailExistsAsync(user.Email))
            return BadRequest("Email is already registered.");
        var newUser = await usersService.AddUserAsync(user, new List<long> { 3, 4, 5, 6 });

        var token = await permissionsServices.GenerateJwtToken(newUser);

        return Created("user", new { token = token });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login login)
    {
        var user = await usersService.GetUserByEmailAsync(login.Email);
        if (user == null || user.HashedPassword != login.Password)
            return Unauthorized("Invalid email or password");

        var token = await permissionsServices.GenerateJwtToken(user);

        return Ok(new { token = token });
    }



    //check
    private async Task<bool> ValidateUserCredentials(string email, string password)
    {
        User? user = await usersService.GetUserByEmailAsync(email);
        if (user == null || user.HashedPassword != password)
            return false;

        return true;
    }

    
}
