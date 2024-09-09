using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AuthorsController : ControllerBase
{

    private readonly IAuthorsService _authorsService;
    public AuthorsController(IAuthorsService authorsService)
        =>  _authorsService = authorsService;
    
    // keep APIs simple
    // GET: api/Authors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Authors>>> GetAuthors()
        => Ok(await _authorsService.GetAllAuthorsAsync());
    

    // GET: api/Authors/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Authors>> GetAuthors(long id)
        => await _authorsService.GetAuthorByIdAsync(id) != null ? Ok() : NotFound();

    // PUT: api/Authors/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAuthor(long id, Authors author)
    {
        if (!await _authorsService.UpdateAuthorAsync(id, author))
            return BadRequest();
        
        return NoContent();
    }

    // POST: api/Authors
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<Authors>> PostAuthor(Authors author)
    {
        var newAuthor = await _authorsService.AddAuthorAsync(author);
        return CreatedAtAction("GetAuthors", new { id = newAuthor.Id }, newAuthor);
    }

    // DELETE: api/Authors/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthors(long id)
    {
        if (!await _authorsService.DeleteAuthorAsync(id))
            return NotFound();
        return NoContent();
    }

    private IActionResult AuthorsExists(long id)
    {
        if (_authorsService.AuthorExists(id))
            return Ok($"Author {id} exists");
        return NotFound();        
    }
}
