using Library.Domain.Entities;

namespace Library.Application.Services;

public interface IBookService
{
    Task<List<Book>> GetAllAsync(CancellationToken ct = default);
    Task<Book?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<int> CreateAsync(Book book, CancellationToken ct = default);
    Task<bool> UpdateAsync(Book book, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}