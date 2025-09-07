using System.ComponentModel.DataAnnotations;

namespace Library_Management.Models
{
    public class Member
    {
        public int Id { get; set; }   // Primary Key

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;  // Unique

        [Required]
        public string PasswordHash { get; set; } = null!;


        [Phone]
        public string? PhoneNumber { get; set; }

        // الخصائص الجديدة المطلوبة للـ View
        public bool IsActive { get; set; } = true;           // هل العضو نشط؟
        public int BorrowedBooksCount { get; set; } = 0;     // عدد الكتب المستعارة
        public bool ExpiredMembership { get; set; } = false; // هل العضوية منتهية؟

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "Buyer"; // القيمة الافتراضية Buyer
        // Navigation Property

        public ICollection<Borrowing> Borrowings { get; set; } = new List<Borrowing>();


    }
}
