using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data;

public class LMSContext : DbContext
{
    public LMSContext(DbContextOptions<LMSContext> options) 
        : base(options)
    {
    }

    public DbSet<Models.Users> Users { get; set; }
    public DbSet<Models.Books> Books { get; set; }
    public DbSet<Models.Authors> Authors { get; set; }
    public DbSet<Models.BorrowingRecord> BorrowingRecord { get; set; }
}
