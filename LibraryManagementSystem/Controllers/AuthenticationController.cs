using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IUsersService _usersService;



    public AuthenticationController(IConfiguration config, IUsersService usersService)
    {
        _usersService = usersService;
        _config = config;

    }

    [AllowAnonymous]
    [HttpPost("user-signup")]
    public async Task<IActionResult> UserSignUp([FromBody] User user)
    {
        if (await _usersService.EmailExistsAsync(user.Email))
            return BadRequest("Email is already registered.");
        var newUser = await _usersService.AddUserAsync(user, new List<long> { 1, 2,3, 4, 5, 6 }); // Assign default permissions

        var token = GenerateJwtToken(newUser);

        return Created("user", new { token = token });
    }

    [AllowAnonymous]
    [HttpPost("author-signup")]
    public async Task<IActionResult> AuthorSignUp([FromBody] User user)
    {
        if (await _usersService.EmailExistsAsync(user.Email))
            return BadRequest("Email is already registered.");
        var newUser = await _usersService.AddUserAsync(user, new List<long> { 3, 4, 5, 6 });

        var token = GenerateJwtToken(newUser);

        return Created("user", new { token = token });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login login)
    {
        var user = await _usersService.GetUserByEmailAsync(login.Email);
        if (user == null || user.HashedPassword != login.Password)
            return Unauthorized("Invalid email or password");

        var token = await GenerateJwtToken(user);

        return Ok(new { token = token });
    }


    private async Task<bool> ValidateUserCredentials(string email, string password)
    {
        User? user = await _usersService.GetUserByEmailAsync(email);
        if (user == null || user.HashedPassword != password)
            return false;

        return true;
    }

    private async Task<string> GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var userPermissions = await _usersService.GetUserPermissionsAsync(user.Id);

        foreach (var permission in userPermissions)
        {
            claims.Add(new Claim("permission", permission.PermissionName));
        }

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
