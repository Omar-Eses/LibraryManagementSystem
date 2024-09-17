using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services.Queries;

public class GetUserByIdQuery : IRequest<User>
{
    public long Id { get; set; }
}

public class GetUserQueryHandler : IRequestHandler<GetUserByIdQuery, User>
{
    private readonly LMSContext _context;
    public GetUserQueryHandler(LMSContext context) => _context = context;

    public async Task<User> Handle(GetUserByIdQuery request) =>
        await _context.Users.FindAsync(request.Id) ?? throw new Exception("User not found");
}