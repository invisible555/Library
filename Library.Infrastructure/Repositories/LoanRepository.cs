using Library.Application.Repositories;
using Library.Domain.Entities;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly ApplicationDbContext _db;

    public LoanRepository(ApplicationDbContext db) => _db = db;

    public Task<Loan?> GetTrackedByIdAsync(int id, CancellationToken ct = default)
        => _db.Loans
            .Include(l => l.Book) // przydaje się do widoków/zwrotu
            .FirstOrDefaultAsync(l => l.Id == id, ct);

    public Task AddAsync(Loan loan, CancellationToken ct = default)
        => _db.Loans.AddAsync(loan, ct).AsTask();

    public Task<List<Loan>> GetMyActiveLoansAsync(string userId, CancellationToken ct = default)
        => _db.Loans.AsNoTracking()
            .Include(l => l.Book)
            .Where(l => l.UserId == userId && l.ReturnedAt == null)
            .OrderByDescending(l => l.BorrowedAt)
            .ToListAsync(ct);
}
