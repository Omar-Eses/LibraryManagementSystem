using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IBooksService _booksService;

    public BooksController(IBooksService booksService)
    {
        _booksService = booksService;
    }

    // GET: api/Books
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Books>>> GetBooks()
    {
        var authors = await _booksService.GetAllBooksAsync();
        return Ok(authors);
    }

    // GET: api/Books/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Books>> GetBooks(long id)
    {
        var books = await _booksService.GetBookByIdAsync(id);
        if (books == null) {
            return NotFound();
        }
        return Ok(books);
    }

    // PUT: api/Books/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBook(long id, Books book)
    {
        if (!await _booksService.UpdateBookAsync(id, book))
        {
            return BadRequest();
        }
        return NoContent();
    }

    // POST: api/Books
    
    [HttpPost]
    public async Task<ActionResult<Books>> PostBook(Books book)
    {
        var newBook = await _booksService.AddBookAsync(book);
        return CreatedAtAction("GetBooks", new { id = newBook.Id }, newBook);
    }

    // DELETE: api/Books/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooks(long id)
    {
        if (!await _booksService.DeleteBookAsync(id))
        {
            return NotFound();
        }
        return NoContent();
    }

    private IActionResult BooksExists(long id)
    {
        if (_booksService.BookExists(id)) 
            return Ok($"Book with id {id} exists");
        return NotFound();
    }
}
