using System.ComponentModel.DataAnnotations;

namespace Library.Domain.Entities;

public class Loan
{
    public int Id { get; set; }

    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public DateTime BorrowedAt { get; set; } = DateTime.UtcNow;
    public DateTime DueAt { get; set; }
    public DateTime? ReturnedAt { get; set; }

    public int Quantity { get; set; } = 1;
}
