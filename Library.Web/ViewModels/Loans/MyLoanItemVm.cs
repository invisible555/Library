namespace Library.Web.ViewModels.Loans;

public class MyLoanItemVm
{
    public int LoanId { get; set; }
    public int BookId { get; set; }
    public string Title { get; set; } = "";
    public int Quantity { get; set; }
    public DateTime BorrowedAt { get; set; }
    public DateTime DueAt { get; set; }
}
