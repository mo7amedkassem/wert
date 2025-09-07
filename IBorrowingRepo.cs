using Library_Management.Models;

namespace Library_Management.IRepo
{
    public interface IBorrowingRepo
    {
        Task<IEnumerable<Borrowing>> GetAllAsync();
        Task<Borrowing?> GetByIdAsync(int id);
        Task AddAsync(Borrowing borrowing);
        Task UpdateAsync(Borrowing borrowing);
        Task DeleteAsync(int id);
    }
}
