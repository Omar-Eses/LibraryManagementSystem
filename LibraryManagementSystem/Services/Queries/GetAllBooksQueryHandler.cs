using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services.Queries;

public class GetAllBooksQuery : IRequest<IEnumerable<Book>> { }
public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, IEnumerable<Book>>
{
    private readonly LMSContext _context;

    public GetAllBooksQueryHandler(LMSContext context) => _context = context;

    public async Task<IEnumerable<Book>> Handle(GetAllBooksQuery query) => await _context.Books.ToListAsync();
}
