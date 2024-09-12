using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BorrowingRecordsController(IBorrowingService borrowingService) : ControllerBase
{
    // POST: api/BorrowingRecords/borrow/
    [HttpPost("borrow/{bookId}")]
    public async Task<IActionResult> BorrowBook(long bookId, [FromBody] BorrowingRecord request)
    {
        //TODO : catch the exception correctly
        try
        {
            var result = await borrowingService.BorrowBookAsync(request.UserId, bookId);
            if (result) return Ok($"Book borrowed by {request.UserId}");

            return BadRequest($"Borrow book operation failed.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // POST: api/BorrowingRecords/return/
    [HttpPost("return/{bookId}")]
    public async Task<IActionResult> ReturnBook(long bookId, [FromBody] long userId)
    {
        //TODO : catch the exception 
        var result = await borrowingService.ReturnBookAsync(userId, bookId);
        if (result) return Ok($"Book returned by {userId}");

        return BadRequest($"Return book operation failed.");
    }


}
