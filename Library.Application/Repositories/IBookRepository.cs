using Library.Domain.Entities;

namespace Library.Application.Repositories;

public interface IBookRepository
{
    Task<List<Book>> GetAllAsync(CancellationToken ct = default);
    Task<Book?> GetByIdAsync(int id, CancellationToken ct = default);

    Task AddAsync(Book book, CancellationToken ct = default);
    void Update(Book book);
    void Remove(Book book);

    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task<Book?> GetTrackedByIdAsync(int id, CancellationToken ct = default);
   
}