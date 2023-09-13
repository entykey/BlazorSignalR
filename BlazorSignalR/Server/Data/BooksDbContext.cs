namespace BlazorSignalR.Server.Data
{
    using Shared;
    using Microsoft.EntityFrameworkCore;


    public class BooksDbContext : DbContext
    {
        public BooksDbContext (DbContextOptions<BooksDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Book { get; set; }
    }
}
