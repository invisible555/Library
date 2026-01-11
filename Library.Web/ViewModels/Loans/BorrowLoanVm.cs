using System.ComponentModel.DataAnnotations;

namespace Library.Web.ViewModels.Loans;

public class BorrowLoanVm
{
    [Required]
    public int BookId { get; set; }

    public string BookTitle { get; set; } = "";

    [Range(1, 100)]
    public int Quantity { get; set; } = 1;

    [DataType(DataType.Date)]
    public DateTime DueAt { get; set; } = DateTime.UtcNow.Date.AddDays(14);

    public int AvailableCopies { get; set; }
}
