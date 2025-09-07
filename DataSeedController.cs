using Library_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library_Management.Controllers
{
    public class DataSeedController : Controller
    {
        private readonly AppDbContext _context;

        public DataSeedController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> SeedTestData()
        {
            try
            {
                // Always ensure test members exist
                var seller = await _context.Members.FirstOrDefaultAsync(m => m.Email == "seller@test.com");
                if (seller == null)
                {
                    seller = new Member
                    {
                        FullName = "Test Seller",
                        Email = "seller@test.com",
                        PasswordHash = "test123",
                        PhoneNumber = "1234567890",
                        IsActive = true,
                        BorrowedBooksCount = 0,
                        ExpiredMembership = false
                    };
                    _context.Members.Add(seller);
                }

                var buyer = await _context.Members.FirstOrDefaultAsync(m => m.Email == "buyer@test.com");
                if (buyer == null)
                {
                    buyer = new Member
                    {
                        FullName = "Test Buyer",
                        Email = "buyer@test.com",
                        PasswordHash = "test123",
                        PhoneNumber = "0987654321",
                        IsActive = true,
                        BorrowedBooksCount = 0,
                        ExpiredMembership = false
                    };
                    _context.Members.Add(buyer);
                }

                await _context.SaveChangesAsync();

                return Content($"Test data seeded successfully!<br/>" +
                    $"Seller ID: {seller.Id} - seller@test.com / test123<br/>" +
                    $"Buyer ID: {buyer.Id} - buyer@test.com / test123<br/>" +
                    $"<a href='/Login/Login'>Go to Login</a>");
            }
            catch (Exception ex)
            {
                return Content($"Error seeding data: {ex.Message}<br/>" +
                    $"<a href='/Login/Login'>Go to Login</a>");
            }
        }
    }
}
