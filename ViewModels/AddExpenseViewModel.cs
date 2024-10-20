using BakingApplication.Commands;
using BakingApplication.Data.Interfaces;
using BakingApplication.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BakingApplication.ViewModels;

public class AddExpenseViewModel : INotifyPropertyChanged
{
    private readonly IExpenseRepository _expenseRepository;

    private ExpenseModel _expenseModel;
    public ExpenseModel ExpenseModel
    {
        get => _expenseModel;
        set
        {
            _expenseModel = value;
            OnPropertyChanged(nameof(ExpenseModel));
        }
    }

    private string _errorMessage;
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged(nameof(ErrorMessage));
        }
    }

    private ObservableCollection<ExpenseTypeModel> _expenseTypes;
    public ObservableCollection<ExpenseTypeModel> ExpenseTypes
    {
        get => _expenseTypes;
        set
        {
            _expenseTypes = value;
            OnPropertyChanged(nameof(ExpenseTypes));
        }
    }

    public RelayCommand CloseWindowCommand { get; set; }
    public RelayCommand AddExpenseCommand { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public AddExpenseViewModel(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
