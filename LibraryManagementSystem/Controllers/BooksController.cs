using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Services.Queries;
using LibraryManagementSystem.Services.Commands.BookCommandsHandlers;

namespace LibraryManagementSystem.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{

    private readonly IDispatcher _dispatcher;

    public BooksController(IDispatcher dipatcher) => _dispatcher = dipatcher;


    // POST: api/Books
    [Authorize(Policy = PermissionTypes.CanAddBook)]
    [HttpPost]
    public async Task<ActionResult<Book>> PostBook(CreateBookCommand command)
    {
        await _dispatcher.Dispatch<CreateBookCommand, Book>(command);
        return Ok();
    }

    // GET: api/Books
    [Authorize(Policy = PermissionTypes.CanGetBook)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
    {
        var query = new GetAllBooksQuery();
        var books = await _dispatcher.Dispatch<GetAllBooksQuery, IEnumerable<Book>>(query);
        return Ok(books);
    }

    // GET: api/Books/5
    [Authorize(Policy = PermissionTypes.CanGetBook)]
    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBook(long id)
    {
        var query = new GetBookQueryById { Id = id };
        var book = await _dispatcher.Dispatch<GetBookQueryById, Book>(query);

        if (book == null) return NotFound();
        return Ok(book);
    }

    // PUT: api/Books/5
    [Authorize(Policy = PermissionTypes.CanEditBook)]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBook(long id, UpdateBookCommand command)
    {
        if (id != command.Id) return BadRequest();

        await _dispatcher.Dispatch<UpdateBookCommand, Book>(command);
        return NoContent();
    }



    // DELETE: api/Books/5
    [Authorize(Policy = PermissionTypes.CanDeleteBook)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooks(long id)
    {
        var command = new DeleteBookCommand { BookId = id };
        await _dispatcher.Dispatch<DeleteBookCommand, Book>(command);

        return NoContent();
    }

    private async Task<IActionResult> BooksExists(long id)
    {
        var book = await _dispatcher.Dispatch<GetBookQueryById, Book>(new GetBookQueryById { Id = id });
        if (book == null) return NotFound();

        return Ok(book);
    }
}
