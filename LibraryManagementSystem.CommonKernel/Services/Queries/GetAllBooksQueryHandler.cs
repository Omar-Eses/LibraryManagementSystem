using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services.Queries;

public class GetAllBooksQuery : IRequest<IEnumerable<Book>>
{
}

public class GetAllBooksQueryHandler(LMSContext context) : IRequestHandler<GetAllBooksQuery, IEnumerable<Book>>
{
    public async Task<IEnumerable<Book>> Handle(GetAllBooksQuery query) => await context.Books.ToListAsync();
}