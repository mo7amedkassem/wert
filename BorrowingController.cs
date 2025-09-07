using Library_Management.IRepo;
using Library_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Library_Management.Filters;

namespace Library_Management.Controllers
{
    public class BorrowingController : Controller
    {
        private readonly IBorrowingRepo _borrowingRepo;
        private readonly IBookRepo _bookRepo;

        public BorrowingController(
            IBorrowingRepo borrowingRepo,
            IBookRepo bookRepo)
        {
            _borrowingRepo = borrowingRepo;
            _bookRepo = bookRepo;
        }

        // GET: Borrowing
        [RoleAuthorize] // must be logged in to view own borrowings
        public async Task<IActionResult> Index()
        {
            var borrowings = await _borrowingRepo.GetAllAsync();
            return View(borrowings);
        }

        // GET: Borrowing/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var borrowing = await _borrowingRepo.GetByIdAsync(id);
            if (borrowing == null)
                return NotFound();

            return View(borrowing);
        }

        // GET: Borrowing/Create
        [RoleAuthorize("Librarian","BookOwner","Seller")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Books = await _bookRepo.GetAllAsync();
            return View();
        }

        // POST: Borrowing/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Librarian","BookOwner","Seller")]
        public async Task<IActionResult> Create(Borrowing borrowing)
        {
            if (ModelState.IsValid)
            {
                await _borrowingRepo.AddAsync(borrowing);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Books = await _bookRepo.GetAllAsync();
            return View(borrowing);
        }

        // GET: Borrowing/Edit/5
        [RoleAuthorize("Librarian","BookOwner","Seller")]
        public async Task<IActionResult> Edit(int id)
        {
            var borrowing = await _borrowingRepo.GetByIdAsync(id);
            if (borrowing == null)
                return NotFound();

            ViewBag.Books = await _bookRepo.GetAllAsync();
            return View(borrowing);
        }

        // POST: Borrowing/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Librarian","BookOwner","Seller")]
        public async Task<IActionResult> Edit(int id, Borrowing borrowing)
        {
            if (id != borrowing.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                await _borrowingRepo.UpdateAsync(borrowing);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Books = await _bookRepo.GetAllAsync();
            return View(borrowing);
        }

        // GET: Borrowing/Delete/5
        [RoleAuthorize("Librarian","BookOwner","Seller")]
        public async Task<IActionResult> Delete(int id)
        {
            var borrowing = await _borrowingRepo.GetByIdAsync(id);
            if (borrowing == null)
                return NotFound();

            return View(borrowing);
        }

        // POST: Borrowing/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Librarian","BookOwner","Seller")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _borrowingRepo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Reader action: add a book to personal library (borrow)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Reader","Borrower","Member","Reader / Borrower")]
        public async Task<IActionResult> AddToMyLibrary(int bookId)
        {
            if (!HttpContext.Request.Cookies.TryGetValue("AuthMemberId", out var memberIdStr) || !int.TryParse(memberIdStr, out var memberId))
                return Forbid();

            var borrowing = new Borrowing
            {
                BookId = bookId,
                MemberId = memberId,
                BorrowedAt = DateTime.UtcNow
            };

            await _borrowingRepo.AddAsync(borrowing);
            return RedirectToAction("Index", "Book");
        }
    }
}
