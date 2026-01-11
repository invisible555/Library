using Library.Application.Services;
using Library.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers;

public class BooksController : Controller
{
    private readonly IBookService _books;

    public BooksController(IBookService books)
    {
        _books = books;
    }

    public async Task<IActionResult> Index()
        => View(await _books.GetAllAsync());

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var book = await _books.GetByIdAsync(id.Value);
        return book == null ? NotFound() : View(book);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Book book)
    {
        if (!ModelState.IsValid) return View(book);
        book.AvailableCopies = book.TotalCopies;
        await _books.CreateAsync(book);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var book = await _books.GetByIdAsync(id.Value);
        return book == null ? NotFound() : View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Book book)
    {
        if (id != book.Id) return NotFound();
        if (!ModelState.IsValid) return View(book);

        var ok = await _books.UpdateAsync(book);
        return ok ? RedirectToAction(nameof(Index)) : NotFound();
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var book = await _books.GetByIdAsync(id.Value);
        return book == null ? NotFound() : View(book);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _books.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
