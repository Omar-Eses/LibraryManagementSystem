using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services.Queries;

public class GetAllUsersQuery : IRequest<IEnumerable<User>> { }
public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<User>>
{
    private readonly LMSContext _context;

    public GetAllUsersQueryHandler(LMSContext context) => _context = context;

    public async Task<IEnumerable<User>> Handle(GetAllUsersQuery query) => await _context.Users.ToListAsync();

}
