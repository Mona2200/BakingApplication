using BakingApplication.Commands;
using BakingApplication.Data.Enitties;
using BakingApplication.Data.Interfaces;
using LiveCharts;
using LiveCharts.Wpf;
using System.ComponentModel;

namespace BakingApplication.ViewModels;

public class StatisticsViewModel : INotifyPropertyChanged
{
    private readonly IOrderRepository _orderRepository;
    private readonly IExpenseRepository _expenseRepository;

    private DateTime _startTime;
    public DateTime StartTime
    {
        get => _startTime;
        set
        {
            if (value < EndTime)
            {
                _startTime = value;
                OnPropertyChanged(nameof(StartTime));
                InitializeStatisticsCollection().ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }
    }

    private DateTime _endTime;
    public DateTime EndTime
    {
        get => _endTime;
        set
        {
            if (value > StartTime)
            {
                _endTime = value;
                OnPropertyChanged(nameof(EndTime));
                InitializeStatisticsCollection().ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }
    }

    private SeriesCollection _statisticsSeriesCollection;
    public SeriesCollection StatisticsSeriesCollection
    {
        get => _statisticsSeriesCollection;
        set
        {
            _statisticsSeriesCollection = value;
            OnPropertyChanged(nameof(StatisticsSeriesCollection));
        }
    }

    private AxesCollection _xAxis;
    public AxesCollection XAxis
    {
        get => _xAxis;
        set
        {
            _xAxis = value;
            OnPropertyChanged(nameof(XAxis));
        }
    }

    private string _selectedWay;
    public string SelectedWay
    {
        get => _selectedWay;
        set
        {
            _selectedWay = value;
            OnPropertyChanged(nameof(SelectedWay));
            InitializeStatisticsCollection().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }

    public RelayCommand CloseWindowCommand { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public StatisticsViewModel(IOrderRepository orderRepository, IExpenseRepository expenseRepository)
    {
        _orderRepository = orderRepository;
        _expenseRepository = expenseRepository;
        _startTime = DateTime.Now - TimeSpan.FromDays(30);
        _endTime = DateTime.Now;
        _selectedWay = "По дням";
        StatisticsSeriesCollection = new();
        InitializeStatisticsCollection().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public async Task InitializeStatisticsCollection()
    {
        StatisticsSeriesCollection.Clear();
        List<Expense> expenses = new();
        List<Order> orders = new();
        Dictionary<DateTime, double> expenseByDate = new();
        Dictionary<DateTime, double> profitByDate = new();
        DateTime dateTime = StartTime.Date;
        while (dateTime <= EndTime.Date)
        {
            expenseByDate.Add(dateTime, 0);
            profitByDate.Add(dateTime, 0);
            switch (SelectedWay)
            {
                case "По дням":
                    dateTime += TimeSpan.FromDays(1);
                    break;
                case "По неделям":
                    dateTime += TimeSpan.FromDays(7);
                    break;
                case "По месяцам":
                    dateTime += TimeSpan.FromDays(30);
                    break;
                default:
                    return;
            }
        }
        if (expenseByDate.Keys.Last() != EndTime.Date
            && profitByDate.Keys.Last() != EndTime.Date)
        {
            expenseByDate.Add(EndTime.Date, 0);
            profitByDate.Add(EndTime.Date, 0);
        }
        await foreach (Expense expense in _expenseRepository.GetExpensesAsAsyncEnumerable(StartTime, EndTime))
            expenses.Add(expense);
        Dictionary<DateTime, int> expenseValues = expenses.OrderByDescending(e => e.Time).GroupBy(e => e.Time.Date).ToDictionary(e => e.Key, e => e.Sum(ex => ex.Amount));
        DateTime key = EndTime.Date;
        foreach (var expenseValue in expenseValues)
            if (expenseByDate.ContainsKey(expenseValue.Key))
            {
                expenseByDate[expenseValue.Key] = expenseValue.Value;
                key = expenseValue.Key;
            }
            else
                expenseByDate[key] += expenseValue.Value;



        await foreach (Order order in _orderRepository.GetOrdersByDateAsAsyncEnumerable(StartTime, EndTime))
            orders.Add(order);
        Dictionary<DateTime, int> profitValues = orders.OrderByDescending(e => e.Date).GroupBy(e => e.Date.Date).ToDictionary(e => e.Key, e => e.Sum(ex => ex.Amount));
        key = EndTime.Date;
        foreach (var profitValue in profitValues)
            if (profitByDate.ContainsKey(profitValue.Key))
                if (profitByDate.ContainsKey(profitValue.Key))
                {
                    profitByDate[profitValue.Key] = profitValue.Value;
                    key = profitValue.Key;
                }
                else
                    profitByDate[key] += profitValue.Value;

        XAxis = new AxesCollection()
        {
            new Axis()
            {
                Labels = expenseByDate.Keys.Select(e => e.ToString("dd.MM.yyyy")).ToList()
            }
        };

        StatisticsSeriesCollection.Add(new LineSeries()
        {
            Title = "Расходы",
            Values = new ChartValues<double>(expenseByDate.Values),
            LabelPoint = (point) => $"{point.Instance} ₽",
            LineSmoothness = 0
        });
        StatisticsSeriesCollection.Add(new LineSeries()
        {
            Title = "Доходы",
            Values = new ChartValues<double>(profitByDate.Values),
            LabelPoint = (point) => $"{point.Instance} ₽",
            LineSmoothness = 0
        });
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
