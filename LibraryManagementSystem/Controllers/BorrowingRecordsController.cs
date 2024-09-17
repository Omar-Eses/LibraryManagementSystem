using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Services.Commands.BorrowingCommandsHandlers;

namespace LibraryManagementSystem.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BorrowingRecordsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public BorrowingRecordsController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    // Inside the BorrowingRecordsController class  
    [HttpPost("borrow/{bookId}")]
    public async Task<IActionResult> BorrowBook(long bookId, [FromBody] long userId)
    {
        var request = new CreateBorrowingCommand { BookId = bookId, UserId = userId };
        await _dispatcher.Dispatch<CreateBorrowingCommand, BorrowingRecord>(request);
        return NoContent();
    }

    // POST: api/BorrowingRecords/return/ 
    [HttpPost("return/{bookId}")]
    public async Task<IActionResult> ReturnBook(long bookId, [FromBody] long userId)
    {
        if (bookId == 0 || userId == 0) return BadRequest("Book ID or User ID is missing.");

        var request = new UpdateBorrowingCommand { BookId = bookId, UserId = userId };
        await _dispatcher.Dispatch<UpdateBorrowingCommand, BorrowingRecord>(request);
        return NoContent();
    }
}
