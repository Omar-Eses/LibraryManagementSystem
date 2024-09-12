using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using LibraryManagementSystem.Helpers;

namespace LibraryManagementSystem.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BooksController(IBooksService booksService) : ControllerBase
{
    // GET: api/Books
    [Authorize(Policy = PermissionTypes.CanGetBook)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
    {
        var authors = await booksService.GetAllBooksAsync();
        return Ok(authors);
    }

    // GET: api/Books/5
    [Authorize(Policy = PermissionTypes.CanGetBook)]
    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBooks(long id)
    {
        var books = await booksService.GetBookByIdAsync(id);
        if (books == null)
        {
            return NotFound();
        }
        return Ok(books);
    }

    // PUT: api/Books/5
    [Authorize(Policy = PermissionTypes.CanEditBook)]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBook(long id, Book book)
    {
        if (!await booksService.UpdateBookAsync(id, book))
        {
            return BadRequest();
        }
        return NoContent();
    }

    // POST: api/Books
    [Authorize(Policy = PermissionTypes.CanAddBook)]
    [HttpPost]
    public async Task<ActionResult<Book>> PostBook(Book book)
    {
        var newBook = await booksService.AddBookAsync(book);
        return CreatedAtAction(nameof(GetBooks), new { id = newBook.Id }, newBook);
    }

    // DELETE: api/Books/5
    [Authorize(Policy = PermissionTypes.CanDeleteBook)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooks(long id)
    {
        if (!await booksService.DeleteBookAsync(id))
        {
            return NotFound();
        }
        return NoContent();
    }

    private IActionResult BooksExists(long id)
    {
        if (booksService.BookExists(id))
            return Ok($"Book with id {id} exists");
        return NotFound();
    }
}
