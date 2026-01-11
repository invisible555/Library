using System.ComponentModel.DataAnnotations;

namespace Library.Domain.Entities;

public class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Author { get; set; }
    public string? Isbn { get; set; }
    public int? Year { get; set; }

    public int TotalCopies { get; set; } = 1;
    public int AvailableCopies { get; set; } = 1;
}