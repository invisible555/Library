using Library.Domain.Entities;

namespace Library.Application.Services;

public interface ILoanService
{
    Task<(bool ok, string? error)> BorrowAsync(int bookId, string userId, int quantity, DateTime dueAt, CancellationToken ct = default);
    Task<(bool ok, string? error)> ReturnAsync(int loanId, string userId, bool isAdmin, CancellationToken ct = default);

    Task<List<Loan>> GetMyActiveLoansAsync(string userId, CancellationToken ct = default);
}
