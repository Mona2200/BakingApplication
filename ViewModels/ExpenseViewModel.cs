using BakingApplication.Commands;
using BakingApplication.Data.Enitties;
using BakingApplication.Data.Interfaces;
using BakingApplication.Models;
using BakingApplication.Views;
using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Media;

namespace BakingApplication.ViewModels;

public class ExpenseViewModel : INotifyPropertyChanged
{
    private readonly MainWindow _mainWindow;
    private readonly IExpenseRepository _expenseRepository;
    private readonly AddExpenseViewModel _addExpenseViewModel;
    private AddExpenseWindow _addExpenseWindow;

    private SeriesCollection _expenseSeriesCollection;
    public SeriesCollection ExpenseSeriesCollection
    {
        get => _expenseSeriesCollection;
        set
        {
            _expenseSeriesCollection = value;
            OnPropertyChanged(nameof(ExpenseSeriesCollection));
        }
    }

    private DateTime _startTime;
    public DateTime StartTime
    {
        get => _startTime;
        set
        {
            _startTime = value;
            OnPropertyChanged(nameof(StartTime));
            InitializeExpenseCollection().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }

    private DateTime _endTime;
    public DateTime EndTime
    {
        get => _endTime;
        set
        {
            _endTime = value;
            OnPropertyChanged(nameof(EndTime));
            InitializeExpenseCollection().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }

    private ObservableCollection<ExpenseTypeModel> _expenseTypes = new();
    public ObservableCollection<ExpenseTypeModel> ExpenseTypes
    {
        get => _expenseTypes;
        set
        {
            _expenseTypes = value;
            OnPropertyChanged(nameof(ExpenseTypes));
        }
    }

    private ExpenseModel _selectedExpenseModel;
    public ExpenseModel SelectedExpenseModel
    {
        get => _selectedExpenseModel;
        set
        {
            _selectedExpenseModel = value;
            OnPropertyChanged(nameof(SelectedExpenseModel));
        }
    }

    private ObservableCollection<ExpenseModel> _expenseModel = new();
    public ObservableCollection<ExpenseModel> ExpenseModels
    {
        get => _expenseModel;
        set
        {
            _expenseModel = value;
            OnPropertyChanged(nameof(ExpenseModels));
        }
    }

    public Func<ChartPoint, string, string> PointLabel { get; set; }
    public RelayCommand AddExpenseWindowCommand { get; set; }
    public RelayCommand DeleteExpenseCommand { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ExpenseViewModel(MainWindow mainWindow, IExpenseRepository expenseRepository, AddExpenseViewModel addExpenseViewModel)
    {
        _mainWindow = mainWindow;
        _expenseRepository = expenseRepository;
        _addExpenseViewModel = addExpenseViewModel;
        ExpenseSeriesCollection = new();
        _startTime = DateTime.Now - TimeSpan.FromDays(30);
        _endTime = DateTime.Now;
        AddExpenseWindowCommand = new(o => ShowAddExpenseWindow());
        DeleteExpenseCommand = new(async o => await DeleteExpense(o));
        PointLabel = (chartPoint, name) =>
                string.Format($"{name}={chartPoint.Y} ({chartPoint.Participation:P})");
        InitializeExpenseCollection().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    private async Task DeleteExpense(object o)
    {
        if (o == null)
            return;
        ExpenseModel expenseModel = (ExpenseModel)o;
        await _expenseRepository.DeleteExpenseAsync(expenseModel.Id);
        InitializeExpenseCollection().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    private async Task InitializeExpenseCollection()
    {
        ExpenseModels.Clear();
        ExpenseTypes.Clear();

        await foreach (ExpenseType expenseType in _expenseRepository.GetExpenseTypesAsAsyncEnumerable())
            ExpenseTypes.Add(new ExpenseTypeModel(expenseType));
        await foreach (Expense expense in _expenseRepository.GetExpensesAsAsyncEnumerable(StartTime, EndTime))
            ExpenseModels.Add(new ExpenseModel()
            {
                Id = expense.Id,
                Time = expense.Time,
                ExpenseType = ExpenseTypes.First(t => t.Id == expense.ExpenseTypeId),
                Amount = expense.Amount,
                Description = expense.Description
            });
        UpdatePie();
    }

    private void ShowAddExpenseWindow()
    {
        _addExpenseViewModel.CloseWindowCommand = new(o => _addExpenseWindow.DialogResult = false);
        _addExpenseViewModel.AddExpenseCommand = new(async o => await AddExpenseToList(o));
        _addExpenseViewModel.ExpenseModel = new();
        _addExpenseViewModel.ExpenseTypes = ExpenseTypes;
        _addExpenseWindow = new();
        _addExpenseWindow.Owner = _mainWindow;
        _addExpenseWindow.DataContext = _addExpenseViewModel;
        _addExpenseWindow.ShowDialog();
    }

    private async Task AddExpenseToList(object obj)
    {
        ExpenseModel addExpenseModel = (ExpenseModel)obj;
        if (addExpenseModel.Amount == 0)
        {
            _addExpenseViewModel.ErrorMessage = "Введите сумму";
            return;
        }
        if (string.IsNullOrWhiteSpace(addExpenseModel.ExpenseType.Name))
        {
            _addExpenseViewModel.ErrorMessage = "Укажите категорию";
            return;
        }

        ExpenseTypeModel? expenseType = ExpenseTypes.FirstOrDefault(t => t.Name == addExpenseModel.ExpenseType.Name);
        if (expenseType != null)
            addExpenseModel.ExpenseType = expenseType;

        Expense expense = await _expenseRepository.AddExpenseAsync(addExpenseModel);
        InitializeExpenseCollection().ConfigureAwait(false).GetAwaiter().GetResult();
        _addExpenseViewModel.CloseWindowCommand.Execute(obj);
    }

    public void UpdatePie()
    {
        Dictionary<string, int> expenses = ExpenseModels.GroupBy(e => e.ExpenseType.Name).ToDictionary(e => e.Key, e => e.Sum(ex => ex.Amount));
        ExpenseSeriesCollection.Clear();
        ExpenseSeriesCollection.AddRange(expenses.Select(e => new PieSeries()
        {
            Values = new ChartValues<double> { e.Value },
            Title = e.Key,
            DataLabels = true,
            LabelPoint = point => PointLabel(point, e.Key),
            LabelPosition = PieLabelPosition.OutsideSlice,
            Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0))
        }));
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
