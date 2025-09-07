using Library_Management.IRepo;
using Library_Management.Models;
using Library_Management.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Library_Management.Filters;

namespace Library_Management.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookRepo _bookRepo;
        private readonly IBorrowingRepo _borrowingRepo;
        private readonly AppDbContext _dbContext;

        public BookController(IBookRepo bookRepo, IBorrowingRepo borrowingRepo, AppDbContext dbContext)
        {
            _bookRepo = bookRepo;
            _borrowingRepo = borrowingRepo;
            _dbContext = dbContext;
        }

        // GET: Book
        public async Task<IActionResult> Index()
        {
            var books = await _bookRepo.GetAllAsync();
            var bookViewModels = books.Select(b => new ModelViews.Book
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                ISBN = b.ISBN,
                CopiesAvailable = b.CopiesAvailable,
                ImageUrl = b.ImageUrl
            });
            return View(bookViewModels);
        }

        // GET: Book/SellerBooks
        [RoleAuthorize("Seller")]
        public async Task<IActionResult> SellerBooks()
        {
            if (!Request.Cookies.TryGetValue("AuthMemberId", out var sellerIdStr)
                || !int.TryParse(sellerIdStr, out var sellerId))
            {
                return Unauthorized();
            }

            var books = await _bookRepo.GetAllAsync();
            var sellerBooks = books
                .Where(b => b.SellerId == sellerId)
                .Select(b => new ModelViews.Book
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    ISBN = b.ISBN,
                    CopiesAvailable = b.CopiesAvailable,
                    ImageUrl = b.ImageUrl
                });
            return View("SellerBooks", sellerBooks);
        }

        // GET: Book/BuyerBooks
        [RoleAuthorize("Buyer")]
        public async Task<IActionResult> BuyerBooks()
        {
            var books = await _bookRepo.GetAllAsync();
            var bookViewModels = books.Select(b => new ModelViews.Book
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                ISBN = b.ISBN,
                CopiesAvailable = b.CopiesAvailable,
                ImageUrl = b.ImageUrl
            });
            return View("BuyerBooks", bookViewModels);
        }

        // GET: Book/BorrowedBooks
        [RoleAuthorize("Buyer")]
        public async Task<IActionResult> BorrowedBooks()
        {
            if (!Request.Cookies.TryGetValue("AuthMemberId", out var buyerIdStr)
                || !int.TryParse(buyerIdStr, out var buyerId))
            {
                return Unauthorized();
            }

            var borrowings = await _borrowingRepo.GetAllAsync();
            var buyerBorrowings = borrowings
                .Where(b => b.MemberId == buyerId && b.ReturnDate == null);
            return View("BorrowedBooks", buyerBorrowings);
        }

        // POST: Book/Borrow
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Buyer")]
        public async Task<IActionResult> Borrow(int bookId)
        {
            if (!Request.Cookies.TryGetValue("AuthMemberId", out var buyerIdStr)
                || !int.TryParse(buyerIdStr, out var buyerId))
            {
                return Unauthorized();
            }

            var borrowing = new Borrowing
            {
                BookId = bookId,
                MemberId = buyerId,
                BorrowedAt = DateTime.UtcNow
            };

            await _borrowingRepo.AddAsync(borrowing);
            return RedirectToAction(nameof(BorrowedBooks));
        }

        // GET: Book/Details/5
        [RoleAuthorize]
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookRepo.GetByIdAsync(id);
            if (book == null)
                return NotFound();

            var bookViewModel = new ModelViews.Book
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                CopiesAvailable = book.CopiesAvailable,
                ImageUrl = book.ImageUrl
            };

            return View(bookViewModel);
        }

        // GET: Book/Create
        [RoleAuthorize("Seller")]
        public IActionResult Create()
        {
            return View(new ModelViews.Book());
        }

        // POST: Book/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Seller")]
        public async Task<IActionResult> Create(ModelViews.Book bookViewModel)
        {
            if (!Request.Cookies.TryGetValue("AuthMemberId", out var sellerIdStr)
                || !int.TryParse(sellerIdStr, out var sellerId))
            {
                return Unauthorized();
            }
             sellerId = bookViewModel.SellerId; // هياخده من hidden field

            // Verify member exists in DB
            var member = await _dbContext.Members.FindAsync(sellerId);
            if (member == null)
            {
                ModelState.AddModelError(string.Empty, "Member not found. Please log in again.");
                return View(bookViewModel);
            }

            if (ModelState.IsValid)
            {
                var book = new Models.Book
                {
                    Title = bookViewModel.Title,
                    Author = bookViewModel.Author,
                    ISBN = bookViewModel.ISBN,
                    CopiesAvailable = bookViewModel.CopiesAvailable,
                    ImageUrl = bookViewModel.ImageUrl,
                    SellerId = sellerId
                };
                await _bookRepo.AddAsync(book);
                return RedirectToAction(nameof(SellerBooks));
            }
            return View(bookViewModel);
        }

        // GET: Book/Edit/5
        [RoleAuthorize("Seller")]
        public async Task<IActionResult> Edit(int id)
        {
            if (!Request.Cookies.TryGetValue("AuthMemberId", out var sellerIdStr)
                || !int.TryParse(sellerIdStr, out var sellerId))
            {
                return Unauthorized();
            }

            var book = await _bookRepo.GetByIdAsync(id);
            if (book == null || book.SellerId != sellerId)
                return Forbid();

            var bookViewModel = new ModelViews.Book
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                CopiesAvailable = book.CopiesAvailable,
                ImageUrl = book.ImageUrl
            };

            return View(bookViewModel);
        }

        public async Task UpdateAsync(Models.Book book)
        {
            var existingBook = await _dbContext.Books.FindAsync(book.Id);
            if (existingBook != null)
            {
                existingBook.Title = book.Title;
                existingBook.Author = book.Author;
                existingBook.ISBN = book.ISBN;
                existingBook.CopiesAvailable = book.CopiesAvailable;
                existingBook.ImageUrl = book.ImageUrl;

                await _dbContext.SaveChangesAsync();
            }
            else
            {
                // اختياري: لو مش لاقي الكتاب، ممكن ترمي استثناء أو تتصرف بطريقة مناسبة
                throw new KeyNotFoundException("Book not found");
            }
        }


        // GET: Book/Delete/5
        [RoleAuthorize("Seller")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!Request.Cookies.TryGetValue("AuthMemberId", out var sellerIdStr)
                || !int.TryParse(sellerIdStr, out var sellerId))
            {
                return Unauthorized();
            }

            var book = await _bookRepo.GetByIdAsync(id);
            if (book == null || book.SellerId != sellerId)
                return Forbid();

            var bookViewModel = new ModelViews.Book
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                CopiesAvailable = book.CopiesAvailable,
                ImageUrl = book.ImageUrl
            };

            return View(bookViewModel);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Seller")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!Request.Cookies.TryGetValue("AuthMemberId", out var sellerIdStr)
                || !int.TryParse(sellerIdStr, out var sellerId))
            {
                return Unauthorized();
            }

            var book = await _bookRepo.GetByIdAsync(id);
            if (book == null || book.SellerId != sellerId)
                return Forbid();

            await _bookRepo.DeleteAsync(id);
            return RedirectToAction(nameof(SellerBooks));
        }
    }
}
