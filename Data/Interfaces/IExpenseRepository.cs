using BakingApplication.Data.Enitties;
using BakingApplication.Models;

namespace BakingApplication.Data.Interfaces;

public interface IExpenseRepository
{

    List<Expense> GetExpenses(DateTime startTime, DateTime endTime);

    List<ExpenseType> GetExpenseTypes();

    Task<Expense> AddExpenseAsync(ExpenseModel expense);

    Task DeleteExpenseAsync(int id);

    Task DeleteExpenseTypeAsync(int id);
}
