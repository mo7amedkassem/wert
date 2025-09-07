using Library_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library_Management.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _dbContext;

        public LoginController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View("Login");
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string role, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("email", "Email is required");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("password", "Password is required");
            }
            if (string.IsNullOrWhiteSpace(role))
            {
                ModelState.AddModelError("role", "Role is required");
            }
            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            // بدون هاش، نستخدم الباسورد كما هو
            var member = await _dbContext.Members
                .FirstOrDefaultAsync(m => m.Email == email && m.PasswordHash == password);

            if (member == null)
            {
                // Create member if doesn't exist (for testing purposes)
                member = new Member
                {
                    FullName = email.Split('@')[0], // Use email prefix as name
                    Email = email,
                    PasswordHash = password,
                    PhoneNumber = "0000000000",
                    IsActive = true,
                    BorrowedBooksCount = 0,
                    ExpiredMembership = false
                };
                _dbContext.Members.Add(member);
                await _dbContext.SaveChangesAsync();
            }

            if (!member.IsActive || member.ExpiredMembership)
            {
                ModelState.AddModelError(string.Empty, "Account is inactive or expired.");
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            Response.Cookies.Append("AuthMemberId", member.Id.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(8)
            });
            Response.Cookies.Append("AuthRole", role, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(8)
            });

            // Redirect according to role
            if (role == "Seller")
                return RedirectToAction("SellerBooks", "Book");
            else if (role == "Buyer")
                return RedirectToAction("BuyerBooks", "Book");

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Book");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmLogout()
        {
            if (Request.Cookies.ContainsKey("AuthMemberId"))
            {
                Response.Cookies.Delete("AuthMemberId");
            }
            return RedirectToAction("Index", "Book");
        }
    }
}
