using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Services.Commands.UserCommandsHandlers;
using LibraryManagementSystem.Services.Queries;

namespace LibraryManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController(IConfiguration config, IDispatcher dispatcher) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("user-signup")]
    public async Task<IActionResult> UserSignUp([FromBody] CreateUserCommand command)
    {
        var emailExists =
            await dispatcher.Dispatch<GetUserByEmailQuery, User>(new GetUserByEmailQuery { Email = command.Email });
        if (emailExists != null)
            return BadRequest("Email is already registered.");
        var newUser = await dispatcher.Dispatch<CreateUserCommand, User>(command);
        var token = await GenerateJwtToken(newUser);

        return Created("user", new { token });
    }

    [AllowAnonymous]
    [HttpPost("author-signup")]
    public async Task<IActionResult> AuthorSignUp([FromBody] CreateUserCommand command)
    {
        // Ensure email is unique before creating a new author
        var emailExists =
            await dispatcher.Dispatch<GetUserByEmailQuery, User>(new GetUserByEmailQuery { Email = command.Email });
        if (emailExists != null)
            return BadRequest("Email is already registered.");

        var newAuthor = await dispatcher.Dispatch<CreateUserCommand, User>(command);

        var token = await GenerateJwtToken(newAuthor);
        return Created("author", new { token });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login login)
    {
        var user = await dispatcher.Dispatch<ValidateUserCredentialsQuery, User>(new ValidateUserCredentialsQuery
        {
            Email = login.Email,
            Password = login.Password
        });

        if (user == null) return Unauthorized("Invalid email or password");

        var token = await GenerateJwtToken(user);
        return Ok(new { token });
    }

    private async Task<string> GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var userPermissions =
            await dispatcher.Dispatch<GetUserPermissionsQuery, List<Permission>>(new GetUserPermissionsQuery
            { userId = user.Id });

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("userId", user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };
        claims.AddRange(userPermissions.Select(permission => new Claim("permission", permission.PermissionName)));

        var token = new JwtSecurityToken(
            issuer: config["JwtSettings:Issuer"],
            audience: config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}