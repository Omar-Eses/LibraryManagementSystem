using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Application.Services.Commands.BookCommandsHandlers;
using LibraryManagementSystem.Application.Services.Queries;
using LibraryManagementSystem.Domain.Helpers;
using LibraryManagementSystem.Domain.Models;

namespace LibraryManagementSystem.API.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController(IDispatcher dispatcher) : ControllerBase
{
    // POST: api/Books
    [Authorize(Policy = PermissionTypes.CanAddBook)]
    [HttpPost]
    public async Task<ActionResult<Book>> PostBook(CreateBookCommand command)
    {
        await dispatcher.Dispatch<CreateBookCommand, Book>(command);
        return Created();
    }

    // GET: api/Books
    [Authorize(Policy = PermissionTypes.CanGetBook)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
    {
        var query = new GetAllBooksQuery();
        var books = await dispatcher.Dispatch<GetAllBooksQuery, IEnumerable<Book>>(query);
        return Ok(books);
    }

    // GET: api/Books/5
    [Authorize(Policy = PermissionTypes.CanGetBook)]
    [HttpGet("{id:long}")]
    public async Task<ActionResult<Book>> GetBook(long id)
    {
        var query = new GetBookQueryById { Id = id };
        var book = await dispatcher.Dispatch<GetBookQueryById, Book>(query);

        if (book == null) return NotFound();
        return Ok(book);
    }

    // PUT: api/Books/5
    [Authorize(Policy = PermissionTypes.CanEditBook)]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> PutBook(long id, [FromBody] UpdateBookCommand command)
    {
        if (id != command.Id) return BadRequest("Id mismatch");

        var bookExists = await dispatcher.Dispatch<GetBookQueryById, Book?>(new GetBookQueryById { Id = id });
        if (bookExists == null) return NotFound($"Book with ID {id} not found");

        await dispatcher.Dispatch<UpdateBookCommand, Book>(command);

        return NoContent();
    }


    // DELETE: api/Books/5
    [Authorize(Policy = PermissionTypes.CanDeleteBook)]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteBooks(long id)
    {
        var book = await dispatcher.Dispatch<GetBookQueryById, Book?>(new GetBookQueryById { Id = id });
        if (book == null) return NotFound();

        var test = await BookExists(id);
        if (!test) return NotFound();


        // Dispatch the delete command
        var command = new DeleteBookCommand { BookId = id };
        await dispatcher.Dispatch<DeleteBookCommand, Book>(command);

        return NoContent();
    }

    private async Task<bool> BookExists(long id)
    {
        var book = await dispatcher.Dispatch<GetBookQueryById, Book?>(new GetBookQueryById { Id = id });
        if (book == null) return false;

        return true;
    }
}