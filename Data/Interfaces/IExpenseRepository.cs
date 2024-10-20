using BakingApplication.Data.Enitties;
using BakingApplication.Models;

namespace BakingApplication.Data.Interfaces;

public interface IExpenseRepository
{
    IAsyncEnumerable<Expense> GetExpensesAsAsyncEnumerable(DateTime startTime, DateTime endTime);

    IAsyncEnumerable<ExpenseType> GetExpenseTypesAsAsyncEnumerable();

    Task<Expense> AddExpenseAsync(ExpenseModel expense);

    Task DeleteExpenseAsync(int id);

    Task DeleteExpenseTypeAsync(int id);
}
