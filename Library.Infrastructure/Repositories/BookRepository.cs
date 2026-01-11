using Library.Application.Repositories;
using Library.Domain.Entities;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly ApplicationDbContext _db;

    public BookRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<List<Book>> GetAllAsync(CancellationToken ct =default )
        => await _db.Books.AsNoTracking()
        .OrderBy(b => b.Title)
        .ToListAsync(ct);


    public async Task<Book?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Books.AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id, ct);

    public async Task AddAsync(Book book, CancellationToken ct = default)
        => await _db.Books.AddAsync(book, ct);

    public void Update(Book book)
        => _db.Books.Update(book);

    public void Remove(Book book)
        => _db.Books.Remove(book);

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => _db.Books.AnyAsync(b => b.Id == id, ct);

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
    public Task<Book?> GetTrackedByIdAsync(int id, CancellationToken ct = default)
    => _db.Books.FirstOrDefaultAsync(b => b.Id == id, ct);

}