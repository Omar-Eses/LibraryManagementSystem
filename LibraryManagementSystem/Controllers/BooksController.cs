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
    [HttpPut("{Id}")]
    public async Task<IActionResult> PutBook(long Id, [FromBody] UpdateBookCommand command)
    {
        Console.WriteLine($"id {Id} = command.Id {command.Id}");
    //    Console.WriteLine(Id == command.Id ? "Id match" : "Id mismatch");
        if (Id != command.Id) return BadRequest("Id mismatch");

        var bookExists = await dispatcher.Dispatch<GetBookQueryById, Book?>(new GetBookQueryById { Id = Id });
        if (bookExists == null) return NotFound($"Book with ID {Id} not found");

        await dispatcher.Dispatch<UpdateBookCommand, Book>(command);
    
        return NoContent();
    }



    // DELETE: api/Books/5
    [Authorize(Policy = PermissionTypes.CanDeleteBook)]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteBooks(long id)
    {
        
        var book = await dispatcher.Dispatch<GetBookQueryById, Book>(new GetBookQueryById { Id = id });
        if (book == null)
            return NotFound($"Book with ID {id} not found.");

        // Dispatch the delete command
        var command = new DeleteBookCommand { BookId = id };
        await dispatcher.Dispatch<DeleteBookCommand, Book>(command);

        return NoContent();
    }

    private async Task<bool> BookExists(long id)
    {
        var book = await dispatcher.Dispatch<GetBookQueryById, Book>(new GetBookQueryById { Id = id });
        if (book == null) return false;
        
        return true;
    }
}