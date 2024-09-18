using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services.Queries;

public class GetUserByIdQuery : IRequest<User>
{
    public long Id { get; set; }
}

public class GetUserQueryHandler(LMSContext context) : IRequestHandler<GetUserByIdQuery, User>
{
    public async Task<User> Handle(GetUserByIdQuery request) =>
        await context.Users.FindAsync(request.Id)
        ?? throw new Exception("User not found");
}