using BlazorSignalR.Server.Data;
using BlazorSignalR.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorSignalR.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BooksDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public BooksController(BooksDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:BlazorSignalRConnectionString"]; // specify to match your appsettings.json
        }


        // GET: api/Books
        /// <summary>
        /// Returns a list of books
        /// 
        /// </summary>
        /// <returns>A list of books</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBook()
        {
            //return await _context.Book.ToListAsync();

            List<Book> result = new List<Book>();
            Book b = null;
            
            string query = @"SELECT b.Id, b.Isbn, b.Name, b.Author, b.Price " +
                            "FROM Book b";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        // loop through all records:
                        foreach (DbDataRecord s in reader)
                        {
                            b = new Book();
                            b.Id = reader.IsDBNull(0) ? null : reader.GetString(0);
                            b.Isbn = reader.IsDBNull(1) ? null : reader.GetString(1);
                            b.Name = reader.IsDBNull(2) ? null : reader.GetString(2);
                            b.Author = reader.IsDBNull(3) ? null : reader.GetString(3);
                            b.Price = reader.GetDouble(4);
                            // @Identity note: concurrencyStamp only have its value (not null) after the role is modified at least one time.
                            result.Add(b);
                        }
                        
                    }
                }

                await connection.CloseAsync();

                return result.ToList();
            }
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(string id)
        {
            var book = await _context.Book.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(string id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Books
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult> PostBook(Book book)
        {
            book.Id = Guid.NewGuid().ToString();
            _context.Book.Add(book);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BookExists(book.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Book>> DeleteBook(string id)
        {
            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Book.Remove(book);
            await _context.SaveChangesAsync();

            return book;
        }

        private bool BookExists(string id)
        {
            return _context.Book.Any(e => e.Id == id);
        }
    }
}
