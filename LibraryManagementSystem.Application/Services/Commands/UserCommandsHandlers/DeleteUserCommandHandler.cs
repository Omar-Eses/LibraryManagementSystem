using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Helpers;
using LibraryManagementSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Services.Commands.UserCommandsHandlers;

// design level
public class DeleteUserCommand : IRequest<User>
{
    public long Id { get; set; }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, User>
{
    private readonly IApplicationDbContext _context;

    public DeleteUserCommandHandler(IApplicationDbContext context,
        IRabbitMQPublisher<DeleteUserCommand> rabbitMqPublisher)
    {
        _context = context;
    }

    //TODO : check the logic
    public async Task<User> Handle(DeleteUserCommand request)
    {
        var user = await _context.Users.FindAsync(request.Id);
        if (user == null) throw new Exception("User not found");

        var borrowedBooks = await _context.Books
            .Where(b => b.borrowedByUserId == request.Id)
            .ToListAsync();

        foreach (var book in borrowedBooks)
        {
            book.borrowedByUserId = null;
            book.BorrowedStatus = Enums.BorrowedStatus.Available;
        }

        // rabbitMQPublisher.PublishMessageToQueueAsync(borrowedBooks); // currently doesn't work bc not of same type
        _context.Books.UpdateRange(borrowedBooks);

        var borrowingRecords = await _context.BorrowingRecords
            .Where(br => br.UserId == request.Id && br.ReturnedDate == null) // Active borrowing records
            .ToListAsync();
        foreach (var record in borrowingRecords)
        {
            record.ReturnedDate = DateTimeOffset.UtcNow;
            record.UserId = null;
        }

        // rabbitMQPublisher.PublishMessageToQueueAsync(borrowingRecords); // currently doesn't work bc not of same type
        _context.BorrowingRecords.UpdateRange(borrowingRecords);
        //context.Users.Remove(user);
        //await context.SaveChangesAsync();

        return user;
    }
}