using System.ComponentModel.DataAnnotations;

namespace BakingApplication.Data.Enitties;

public class ExpenseType
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }
}
