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

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    //optionsBuilder.UseSqlServer("Server=.; Database=BlazorSignalRServer; Trusted_Connection=True; MultipleActiveResultSets=True; TrustServerCertificate=True");
        //}

        public DbSet<Book> Book { get; set; }
    }
}
