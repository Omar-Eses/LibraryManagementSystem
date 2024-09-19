using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Services.Commands.BorrowingCommandsHandlers;
using System.Security.Claims;

namespace LibraryManagementSystem.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BorrowingRecordsController(IDispatcher dispatcher) : Controller
{
    [HttpPost("borrow/{bookId:long}")]
    public async Task<IActionResult> BorrowBook(long bookId)
    {
        var request = new CreateBorrowingCommand { BookId = bookId };
        await dispatcher.Dispatch<CreateBorrowingCommand, BorrowingRecord>(request);
        return NoContent();
    }

    // POST: api/BorrowingRecords/return/ 
    [HttpPost("return/{bookId:long}")]
    public async Task<IActionResult> ReturnBook(long bookId)
    {
        if (bookId == 0) return BadRequest("Book id is missing");

        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized("User not found");

        try
        {
            var request = new UpdateBorrowingCommand
            {
                BookId = bookId
            };

            await dispatcher.Dispatch<UpdateBorrowingCommand, BorrowingRecord>(request);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }


    }
}