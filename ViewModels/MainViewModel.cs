using BakingApplication.Commands;
using BakingApplication.Data.Enitties;
using BakingApplication.Views;
using System.ComponentModel;
using System.Windows.Controls;

namespace BakingApplication.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private CatalogPanel _catalogPanel;
    private ExpensesPanel _expensesPanel; 

    public OrderViewModel OrderViewModel { get; private set; }

    public UserControl _selectUserControl;
    public UserControl SelectUserControl
    {
        get => _selectUserControl;
        set
        {
            _selectUserControl = value;
            OnPropertyChanged(nameof(SelectUserControl));
        }
    }

    public RelayCommand SwitchPanel { get; set; }
    public RelayCommand CloseWindowCommand { get; set; }
    public RelayCommand HideWindowCommand { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public MainViewModel(CatalogViewModel catalogViewModel, OrderViewModel orderViewModel, ExpenseViewModel costsViewModel, MainWindow mainWindow)
    {
        OrderViewModel = orderViewModel;
        _catalogPanel = new() { DataContext = catalogViewModel };
        _expensesPanel = new() { DataContext = costsViewModel };
        SwitchPanel = new(o =>
        {
            int index = Convert.ToInt32(o);
            switch (index)
            {
                case 0:
                    SelectUserControl = _catalogPanel;
                    break;
                case 1:
                    SelectUserControl = _expensesPanel;
                    break;
            }
        });
        CloseWindowCommand = new(o => mainWindow.Close());
        HideWindowCommand = new(o => mainWindow.WindowState = System.Windows.WindowState.Minimized);
        SelectUserControl = _catalogPanel;
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
