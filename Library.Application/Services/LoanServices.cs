using Library.Application.Repositories;
using Library.Domain.Entities;

namespace Library.Application.Services;

public class LoanService : ILoanService
{
    private readonly IBookRepository _books;
    private readonly ILoanRepository _loans;
    private readonly IUnitOfWork _uow;

    public LoanService(IBookRepository books, ILoanRepository loans, IUnitOfWork uow)
    {
        _books = books;
        _loans = loans;
        _uow = uow;
    }

    public async Task<(bool ok, string? error)> BorrowAsync(
        int bookId, string userId, int quantity, DateTime dueAt, CancellationToken ct = default)
    {
        if (quantity <= 0) return (false, "Quantity must be > 0.");
        if (string.IsNullOrWhiteSpace(userId)) return (false, "UserId is required.");
        if (dueAt <= DateTime.UtcNow.Date) return (false, "Due date must be in the future.");

        await _uow.BeginTransactionAsync(ct);
        try
        {
            var book = await _books.GetTrackedByIdAsync(bookId, ct);
            if (book == null) return (false, "Book not found.");

            if (book.AvailableCopies < quantity)
                return (false, "Not enough copies available.");

            book.AvailableCopies -= quantity;

            var loan = new Loan
            {
                BookId = bookId,
                UserId = userId,
                Quantity = quantity,
                DueAt = dueAt.ToUniversalTime(),
                BorrowedAt = DateTime.UtcNow
            };

            await _loans.AddAsync(loan, ct);
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);

            return (true, null);
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }

    public async Task<(bool ok, string? error)> ReturnAsync(
        int loanId, string userId, bool isAdmin, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(userId)) return (false, "UserId is required.");

        await _uow.BeginTransactionAsync(ct);
        try
        {
            var loan = await _loans.GetTrackedByIdAsync(loanId, ct);
            if (loan == null) return (false, "Loan not found.");

            if (!isAdmin && loan.UserId != userId)
                return (false, "You are not allowed to return this loan.");

            if (loan.ReturnedAt != null)
            {
                await _uow.CommitTransactionAsync(ct);
                return (true, null); // już zwrócone
            }

            // book będzie załadowany przez Include w repo, ale na wszelki wypadek:
            var book = loan.Book ?? await _books.GetTrackedByIdAsync(loan.BookId, ct);
            if (book == null) return (false, "Book not found.");

            loan.ReturnedAt = DateTime.UtcNow;
            book.AvailableCopies += loan.Quantity;

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);

            return (true, null);
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }

    public Task<List<Loan>> GetMyActiveLoansAsync(string userId, CancellationToken ct = default)
        => _loans.GetMyActiveLoansAsync(userId, ct);
}
