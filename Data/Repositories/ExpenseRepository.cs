using BakingApplication.Data.Enitties;
using BakingApplication.Data.Interfaces;
using BakingApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace BakingApplication.Data.Repositories;

public class ExpenseRepository(BakingApplicationContext context) : IExpenseRepository
{
    public async Task<Expense> AddExpenseAsync(ExpenseModel expense)
    {
        int expenseTypeId;
        if (expense.ExpenseType.Id == 0)
        {
            ExpenseType expenseType = new() { Name = expense.ExpenseType.Name };
            await context.ExpenseTypes.AddAsync(expenseType);
            await context.SaveChangesAsync();
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
        await context.Expenses.AddAsync(newExpense);
        await context.SaveChangesAsync();
        return newExpense;
    }

    public async Task DeleteExpenseAsync(int id)
    {
        Expense expense = await context.Expenses.SingleAsync(e => e.Id == id);
        context.Expenses.Remove(expense);
        if (!context.Expenses.Any(e => e.ExpenseTypeId == expense.ExpenseTypeId))
        await DeleteExpenseTypeAsync(expense.ExpenseTypeId);
        await context.SaveChangesAsync();
    }

    public async Task DeleteExpenseTypeAsync(int id)
    {
        ExpenseType expenseType = await context.ExpenseTypes.SingleAsync(e => e.Id == id);
        context.ExpenseTypes.Remove(expenseType);
        await context.SaveChangesAsync();
    }

    public List<Expense> GetExpenses(DateTime startTime, DateTime endTime)
    {
        return context.Expenses.Where(e => e.Time >= startTime && e.Time <= endTime).ToList();
    }

    public List<ExpenseType> GetExpenseTypes()
    {
        return context.ExpenseTypes.ToList();
    }
}
