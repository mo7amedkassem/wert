using Library_Management.IRepo;
using Library_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Library_Management.Repos
{
    public class BookRepo : IBookRepo
    {
        private readonly AppDbContext _dbContext;

        public BookRepo(AppDbContext dbContext) 
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(Book book)
        {
            await _dbContext.Books.AddAsync(book);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var book = await _dbContext.Books.FindAsync(id);
            if (book != null)
            {
                _dbContext.Books.Remove(book);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _dbContext.Books.ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _dbContext.Books.FindAsync(id);
        }

        public Task UpdateAsync(Book book)
        {
            throw new NotImplementedException();
        }
    }
}
