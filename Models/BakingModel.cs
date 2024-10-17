using System.ComponentModel;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace BakingApplication.Models;

public class BakingModel : INotifyPropertyChanged
{
    public int Id { get; set; }

    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    private string? _description;
    public string? Description
    {
        get => _description;
        set
        {
            _description = value;
            OnPropertyChanged(nameof(Description));
        }
    }

    private int _cost;
    public int Cost
    {
        get => _cost;
        set
        {
            _cost = value;
            OnPropertyChanged(nameof(Cost));
        }
    }

    private int _weight;
    public int Weight
    {
        get => _weight;
        set
        {
            _weight = value;
            OnPropertyChanged(nameof(Weight));
        }
    }

    public bool HasPicture { get; set; }

    private BitmapImage? _picture;
    public BitmapImage? Picture
    {
        get => _picture;
        set
        {
            _picture = value;
            OnPropertyChanged(nameof(Picture));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
