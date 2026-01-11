using Library.Application.Repositories;
using Library.Domain.Entities;

namespace Library.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _repo;

    public BookService(IBookRepository repo)
    {
        _repo = repo;
    }

    public Task<List<Book>> GetAllAsync(CancellationToken ct = default)
        => _repo.GetAllAsync(ct);

    public Task<Book?> GetByIdAsync(int id, CancellationToken ct = default)
        => _repo.GetByIdAsync(id, ct);

    public async Task<int> CreateAsync(Book book, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(book.Title))
            throw new ArgumentException("Title is required.", nameof(book));
        await _repo.AddAsync(book, ct);
        await _repo.SaveChangesAsync(ct);
        return book.Id;
    }

    public async Task<bool> UpdateAsync(Book book, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(book.Title))
            throw new ArgumentException("Title is required.", nameof(book));
        var tracked = await _repo.GetTrackedByIdAsync(book.Id, ct);
        if (tracked == null) return false;

        tracked.Title = book.Title;
        tracked.Author = book.Author;
        tracked.Isbn = book.Isbn;
        tracked.Year = book.Year;

        await _repo.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var book = await _repo.GetTrackedByIdAsync(id, ct);
        if (book == null) return false;

        _repo.Remove(book);
        await _repo.SaveChangesAsync(ct);
        return true;
    }

 
}
