using System.ComponentModel.DataAnnotations;

namespace Library_Management.Models
{
    public class Borrowing
    {
        // Composite Key (BookId + MemberId) هنضبطها بالـ Fluent API

        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BorrowedAt { get; set; } = DateTime.UtcNow;

        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; }






        // Navigation Properties
        public int BookId { get; set; }
        public Book Book { get; set; }

        public int MemberId { get; set; }
        public Member member { get; set; }
    }
}
