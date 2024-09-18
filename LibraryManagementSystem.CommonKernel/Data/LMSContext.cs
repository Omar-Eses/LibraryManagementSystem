using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data;

public class LMSContext : DbContext
{
    public LMSContext(DbContextOptions<LMSContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<BorrowingRecord> BorrowingRecord { get; set; }
    public DbSet<UserPermissions> UserPermissions { get; set; }
}
