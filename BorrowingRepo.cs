using Library_Management.IRepo;
using Library_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Library_Management.Repos
{
    public class BorrowingRepo : IBorrowingRepo
    {



        private readonly AppDbContext _dbContext;

        public BorrowingRepo(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }



        public async Task AddAsync(Borrowing borrowing)
        {
            await _dbContext.Borrowings.AddAsync(borrowing);
            await _dbContext.SaveChangesAsync();
        }






        public async Task DeleteAsync(int id)
        {
            var borrowing = await _dbContext.Borrowings.FindAsync(id);
            if (borrowing != null)
            {
                _dbContext.Borrowings.Remove(borrowing);
                await _dbContext.SaveChangesAsync();
            }
        }






        public async Task<IEnumerable<Borrowing>> GetAllAsync()
        {
            return await _dbContext.Borrowings
                .Include(b => b.Book)    // نجيب بيانات الكتاب
                .Include(b => b.member)  // نجيب بيانات العضو
                .ToListAsync();
        }






        public async Task<Borrowing?> GetByIdAsync(int id)
        {
            return await _dbContext.Borrowings
                .Include(b => b.Book)
                .Include(b => b.member)
                .FirstOrDefaultAsync(b => b.Id == id);
        }






        public async Task UpdateAsync(Borrowing borrowing)
        {
            _dbContext.Borrowings.Update(borrowing);
            await _dbContext.SaveChangesAsync();
        }
    }
}
