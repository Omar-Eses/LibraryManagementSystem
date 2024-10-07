using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Helpers;
using LibraryManagementSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Services.Queries;

public class ValidateUserCredentialsQuery : IRequest<User>
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class ValidateUserCredentialsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<ValidateUserCredentialsQuery, User>
{
    private readonly TimeSpan _cacheDuration = CommonVariables.CacheExpirationTime;

    public async Task<User> Handle(ValidateUserCredentialsQuery request) => await context.Users
                                                                                .FirstOrDefaultAsync(u =>
                                                                                    u.Email == request.Email &&
                                                                                    u.HashedPassword ==
                                                                                    request.Password) ??
                                                                            throw new Exception("Invalid credentials");
}