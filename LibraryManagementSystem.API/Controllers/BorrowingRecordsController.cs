using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Application.Services.Commands.BorrowingCommandsHandlers;
using LibraryManagementSystem.Domain.Models;

namespace LibraryManagementSystem.API.Controllers;

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