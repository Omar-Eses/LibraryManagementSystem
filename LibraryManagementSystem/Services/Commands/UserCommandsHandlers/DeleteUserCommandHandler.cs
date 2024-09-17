using LibraryManagementSystem.Data;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Commands.UserCommandsHandlers;

public class DeleteUserCommand : IRequest<User>
{
    public long UserId { get; set; }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, User>
{
    private readonly LMSContext _context;
    public DeleteUserCommandHandler(LMSContext context)
    {
        _context = context;
    }

    public async Task<User> Handle(DeleteUserCommand request)
    {
        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null) throw new Exception("User not found");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return user;
    }
}
