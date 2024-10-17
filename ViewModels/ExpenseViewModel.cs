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
            Initialize();
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
            Initialize();
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

    private ObservableCollection<ExpenseModel> _expenseModel;
    public ObservableCollection<ExpenseModel> ExpenseModel
    {
        get => _expenseModel;
        set
        {
            _expenseModel = value;
            OnPropertyChanged(nameof(ExpenseModel));
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
        Initialize();
        AddExpenseWindowCommand = new(o => ShowAddExpenseWindow());
        DeleteExpenseCommand = new(async o => await DeleteExpense(o));
        PointLabel = (chartPoint, name) =>
                string.Format($"{name}={chartPoint.Y} ({chartPoint.Participation:P})");
    }

    private async Task DeleteExpense(object o)
    {
        if (o == null)
            return;
        ExpenseModel expenseModel = (ExpenseModel)o;
        await _expenseRepository.DeleteExpenseAsync(expenseModel.Id);
        ExpenseModel.Remove(expenseModel);
        if (!ExpenseModel.Any(e => e.ExpenseType.Id == expenseModel.ExpenseType.Id))
            ExpenseTypes.Remove(expenseModel.ExpenseType);
        UpdatePie();
    }

    private void Initialize()
    {
        List<Expense> expenses = _expenseRepository.GetExpenses(StartTime, EndTime);
        List<ExpenseTypeModel> expenseTypes = _expenseRepository.GetExpenseTypes().Select(e => new ExpenseTypeModel(e)).ToList();

        List<ExpenseModel> expenseModels = expenses.Select(e => new ExpenseModel()
        {
            Id = e.Id,
            Time = e.Time,
            ExpenseType = expenseTypes.First(t => t.Id == e.ExpenseTypeId),
            Amount = e.Amount,
            Description = e.Description
        }).ToList();

        ExpenseModel = new ObservableCollection<ExpenseModel>(expenseModels);
        ExpenseTypes = new ObservableCollection<ExpenseTypeModel>(expenseTypes);
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
        if (addExpenseModel.ExpenseType.Id == 0)
            ExpenseTypes.Add(new ExpenseTypeModel() { Id = expense.ExpenseTypeId, Name = addExpenseModel.ExpenseType.Name });
        Initialize();
        _addExpenseViewModel.CloseWindowCommand.Execute(obj);
    }

    public void UpdatePie()
    {
        Dictionary<string, int> expenses = ExpenseModel.GroupBy(e => e.ExpenseType.Name).ToDictionary(e => e.Key, e => e.Sum(ex => ex.Amount));
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
