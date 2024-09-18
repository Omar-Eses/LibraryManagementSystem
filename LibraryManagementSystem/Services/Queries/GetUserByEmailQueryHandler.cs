using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services.Queries;

public class GetUserByEmailQuery : IRequest<bool>
{
    public string Email { get; set; }
}

public class GetUserByEmailQueryHandler(LMSContext context) : IRequestHandler<GetUserByEmailQuery, bool>
{
    public async Task<bool> Handle(GetUserByEmailQuery request)
    {
        var emailExists = await context.Users
            .AnyAsync(user => user.Email == request.Email);

        return emailExists;
    }
}