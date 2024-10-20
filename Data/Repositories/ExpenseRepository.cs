using BakingApplication.Data.Enitties;
using BakingApplication.Data.Interfaces;
using BakingApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace BakingApplication.Data.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly BakingApplicationContext _context;

    public ExpenseRepository(BakingApplicationContext context)
    {
        _context = context;
    }

    public async Task<Expense> AddExpenseAsync(ExpenseModel expense)
    {
        int expenseTypeId;
        if (expense.ExpenseType.Id == 0)
        {
            ExpenseType expenseType = new() { Name = expense.ExpenseType.Name };
            await _context.ExpenseTypes.AddAsync(expenseType);
            await _context.SaveChangesAsync();
            expenseTypeId = expenseType.Id;
        }
        else
            expenseTypeId = expense.ExpenseType.Id;
        Expense newExpense = new()
        {
            Amount = expense.Amount,
            ExpenseTypeId = expenseTypeId,
            Time = expense.Time,
            Description = expense.Description,
        };
        await _context.Expenses.AddAsync(newExpense);
        await _context.SaveChangesAsync();
        return newExpense;
    }

    public async Task DeleteExpenseAsync(int id)
    {
        Expense expense = await _context.Expenses.SingleAsync(e => e.Id == id);
        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();
        if (!_context.Expenses.Any(e => e.ExpenseTypeId == expense.ExpenseTypeId))
            await DeleteExpenseTypeAsync(expense.ExpenseTypeId);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteExpenseTypeAsync(int id)
    {
        ExpenseType expenseType = await _context.ExpenseTypes.SingleAsync(e => e.Id == id);
        _context.ExpenseTypes.Remove(expenseType);
        await _context.SaveChangesAsync();
    }

    public IAsyncEnumerable<Expense> GetExpensesAsAsyncEnumerable(DateTime startTime, DateTime endTime)
    {
        return _context.Expenses.Where(e => e.Time.Date >= startTime.Date && e.Time.Date <= endTime.Date).AsAsyncEnumerable();
    }

    public IAsyncEnumerable<ExpenseType> GetExpenseTypesAsAsyncEnumerable()
    {
        return _context.ExpenseTypes.AsAsyncEnumerable();
    }
}
