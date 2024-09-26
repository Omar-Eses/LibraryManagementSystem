using LibraryManagementSystem.CommonKernel.Interfaces;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services.Queries;

public class ValidateUserCredentialsQuery : IRequest<User>
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class ValidateUserCredentialsQueryHandler(LMSContext context)
    : IRequestHandler<ValidateUserCredentialsQuery, User>
{
    private readonly TimeSpan _cacheDuration = CommonVariables.CacheExpirationTime;
    public async Task<User> Handle(ValidateUserCredentialsQuery request)=> await context.Users
        .FirstOrDefaultAsync(u => u.Email == request.Email && u.HashedPassword == request.Password) ?? throw new Exception("Invalid credentials");
}