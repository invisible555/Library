using System.Security.Claims;
using Library.Application.Services;
using Library.Web.ViewModels.Loans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers;

[Authorize]
public class LoansController : Controller
{
    private readonly ILoanService _loans;

    public LoansController(ILoanService loans)
    {
        _loans = loans;
    }

    // GET: /Loans/Borrow?bookId=5
    [HttpGet]
    public IActionResult Borrow(int bookId, string? title = null, int available = 0)
    {
        // Ten GET jest “lekki”: dane przekazujemy z listy książek w querystring.
        // Jeśli wolisz 100% pewności, zrób BookService.GetById i dociągaj z bazy.
        var vm = new BorrowLoanVm
        {
            BookId = bookId,
            BookTitle = title ?? "",
            AvailableCopies = available,
            Quantity = 1,
            DueAt = DateTime.UtcNow.Date.AddDays(14)
        };

        return View(vm);
    }

    // POST: /Loans/Borrow
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Borrow(BorrowLoanVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        // Upewnij się, że dueAt jest w UTC
        var dueAtUtc = DateTime.SpecifyKind(vm.DueAt, DateTimeKind.Utc);

        var (ok, error) = await _loans.BorrowAsync(vm.BookId, userId, vm.Quantity, dueAtUtc, ct);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, error ?? "Borrow failed.");
            return View(vm);
        }

        TempData["Success"] = "Book borrowed.";
        return RedirectToAction(nameof(My));
    }

    // GET: /Loans/My
    [HttpGet]
    public async Task<IActionResult> My(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var loans = await _loans.GetMyActiveLoansAsync(userId, ct);

        var vm = loans.Select(l => new MyLoanItemVm
        {
            LoanId = l.Id,
            BookId = l.BookId,
            Title = l.Book?.Title ?? "(unknown)",
            Quantity = l.Quantity,
            BorrowedAt = l.BorrowedAt,
            DueAt = l.DueAt
        }).ToList();

        return View(vm);
    }

    // POST: /Loans/Return
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(int loanId, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var isAdmin = User.IsInRole("Admin");

        var (ok, error) = await _loans.ReturnAsync(loanId, userId, isAdmin, ct);
        if (!ok)
        {
            TempData["Error"] = error ?? "Return failed.";
        }
        else
        {
            TempData["Success"] = "Book returned.";
        }

        return RedirectToAction(nameof(My));
    }
}
