using LibraryManagementSystem.CommonKernel.Interfaces;
using LibraryManagementSystem.CommonKernel.Services;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services.Commands.UserCommandsHandlers;

public class DeleteUserCommand : IRequest<User>
{
    public long UserId { get; set; }
}

public class DeleteUserCommandHandler(LMSContext context, IRabbitMQPublisher<DeleteUserCommand> rabbitMQPublisher) : IRequestHandler<DeleteUserCommand, User>
{
    public async Task<User> Handle(DeleteUserCommand request)
    {
        var user = await context.Users.FindAsync(request.UserId);
        if (user == null) throw new Exception("User not found");

        var borrowedBooks = await context.Books
            .Where(b => b.borrowedByUserId == request.UserId)
            .ToListAsync();

        foreach (var book in borrowedBooks)
        {
            book.borrowedByUserId = null;
            book.BorrowedStatus = Enums.BorrowedStatus.Available;
        }
        // rabbitMQPublisher.PublishMessageToQueueAsync(borrowedBooks); // currently doesn't work bc not of same type
        context.Books.UpdateRange(borrowedBooks);

        var borrowingRecords = await context.BorrowingRecord
            .Where(br => br.UserId == request.UserId && br.ReturnedDate == null) // Active borrowing records
            .ToListAsync();
        foreach (var record in borrowingRecords)
        {
            record.ReturnedDate = DateTimeOffset.UtcNow;
            record.UserId = null;
        }
        // rabbitMQPublisher.PublishMessageToQueueAsync(borrowingRecords); // currently doesn't work bc not of same type
        context.BorrowingRecord.UpdateRange(borrowingRecords);

        await rabbitMQPublisher.PublishMessageToQueueAsync(request);
        //context.Users.Remove(user);
        //await context.SaveChangesAsync();

        return user;
    }
}