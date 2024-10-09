using LibraryManagementSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Interfaces;

public interface IApplicationDbContext
{
    public DbSet<User> Users { get; }
    public DbSet<Book> Books { get; }
    public DbSet<BorrowingRecord> BorrowingRecords { get; }
    public DbSet<UserPermission> UserPermissions { get; }
    Task<int> SaveChangesAsync();
}