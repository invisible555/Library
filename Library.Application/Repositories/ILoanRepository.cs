using Library.Domain.Entities;

namespace Library.Application.Repositories;

public interface ILoanRepository
{
    Task<Loan?> GetTrackedByIdAsync(int id, CancellationToken ct = default);

    Task AddAsync(Loan loan, CancellationToken ct = default);

    Task<List<Loan>> GetMyActiveLoansAsync(string userId, CancellationToken ct = default);
}
