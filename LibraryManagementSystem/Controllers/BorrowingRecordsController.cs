using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BorrowingRecordsController : ControllerBase
{
    private readonly IBorrowingService _borrowingService;

    public BorrowingRecordsController(IBorrowingService borrowingService)
    {
        _borrowingService = borrowingService;
    }

    // POST: api/BorrowingRecords/borrow/
    [HttpPost("borrow/{bookId}")]
    public async Task<IActionResult> BorrowBook(long bookId, [FromBody] BorrowingRecord request)
    {
        var result = await _borrowingService.BorrowBookAsync(request.UserId, bookId);
        if (result)
            return Ok($"Book borrowed by {request.UserId}");
        return BadRequest($"Borrow book operation failed.");
    }

    // POST: api/BorrowingRecords/return/
    [HttpPost("return/{bookId}")]
    public async Task<IActionResult> ReturnBook(long bookId, [FromBody] long userId)
    {
        var result = await _borrowingService.ReturnBookAsync(userId, bookId);
        if (result)
            return Ok($"Book returned by {userId}");
        return BadRequest($"Return book operation failed.");
    }


}
