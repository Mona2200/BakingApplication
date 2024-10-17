using System.ComponentModel.DataAnnotations;

namespace BakingApplication.Data.Enitties;

public class Expense
{
    [Key]
    public int Id { get; set; }

    public DateTime Time { get; set; }

    public int ExpenseTypeId { get; set; }

    public int Amount { get; set; }

    public string Description { get; set; }
}
