using BakingApplication.Commands;
using BakingApplication.Data.Enitties;
using BakingApplication.Data.Interfaces;
using BakingApplication.Models;
using BakingApplication.Utilities;
using BakingApplication.Views;
using GemBox.Spreadsheet;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Globalization;
using System.Windows.Forms;

namespace BakingApplication.ViewModels;

public class OrderViewModel : INotifyPropertyChanged
{
    private readonly IOrderRepository _orderRepository;
    private readonly MainWindow _mainWindow;
    private OrderListWindow _orderListWindow;
    private readonly OrderListViewModel _orderListViewModel;

    private OrderModel _orderModel;
    public OrderModel OrderModel
    {
        get => _orderModel;
        set
        {
            _orderModel = value;
            OnPropertyChanged(nameof(OrderModel));
        }
    }

    public RelayCommand IncreaseBakingCountCommand { get; set; }
    public RelayCommand DecreaseBakingCountCommand { get; set; }
    public RelayCommand ClearOrderCommand { get; set; }
    public RelayCommand PrintOrderCommand { get; set; }
    public RelayCommand OrderListCommand { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public OrderViewModel(
        IOrderRepository orderRepository,
        MainWindow mainWindow,
        OrderModel orderModel,
        OrderListViewModel orderListViewModel)
    {
        OrderModel = orderModel;
        _orderRepository = orderRepository;
        _mainWindow = mainWindow;
        _orderListViewModel = orderListViewModel;
        IncreaseBakingCountCommand = new(o => IncreaseBakingCount(o));
        DecreaseBakingCountCommand = new(o => DecreaseBakingCount(o));
        ClearOrderCommand = new(o => ClearOrder());
        PrintOrderCommand = new(async o => await PrintOrder());
        OrderListCommand = new(o => ShowOrderList());
    }

    private void ShowOrderList()
    {
        _orderListViewModel.CloseWindowCommand = new(o => _orderListWindow.DialogResult = false);
        _orderListViewModel.RepeatOrderCommand = new(o =>
        {
            OrderModel = (OrderModel)o;
            _orderListWindow.DialogResult = false;
        });
        _orderListViewModel.OrderModels = _orderListViewModel.InitilizeOrderCollection();
        _orderListWindow = new();
        _orderListWindow.Owner = _mainWindow;
        _orderListWindow.DataContext = _orderListViewModel;
        _orderListWindow.ShowDialog();
    }

    private async Task PrintOrder()
    {
        if (!OrderModel.BakingCount.Any())
            return;

        SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
        DateTime time = DateTime.Now;
        CultureInfo culture = CultureInfo.CreateSpecificCulture("ru-RU");

        ExcelFile workbook = new ExcelFile();
        ExcelWorksheet sheet = workbook.Worksheets.Add("1");

        // Задание ширины колонок

        sheet.Columns[0].SetWidth(30, LengthUnit.CharacterWidth);
        sheet.Columns[1].SetWidth(10, LengthUnit.CharacterWidth);
        sheet.Columns[2].SetWidth(10, LengthUnit.CharacterWidth);
        sheet.Columns[3].SetWidth(15, LengthUnit.CharacterWidth);

        // Строка Фирма

        sheet.Rows[0].SetHeight(15, LengthUnit.Point);
        sheet.Cells.GetSubrangeAbsolute(0, 0, 0, 3).Merged = true;
        sheet.Cells[0, 0].Value = "Фирма: \"Ням-Ням\"";

        //Строка Дата

        sheet.Rows[1].SetHeight(15, LengthUnit.Point);
        sheet.Cells.GetSubrangeAbsolute(1, 0, 1, 3).Merged = true;
        sheet.Cells[1, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
        sheet.Cells[1, 0].Value = $"Дата: {time.ToString("«dd» MMMM yyyy", culture)}г.";

        sheet.Rows[2].SetHeight(5, LengthUnit.Point);

        // Заголовок таблицы

        sheet.Rows[3].SetHeight(20, LengthUnit.Point);
        sheet.Cells.GetSubrangeAbsolute(3, 0, 3, 3).Merged = true;
        sheet.Cells[3, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
        sheet.Cells[3, 0].Style.VerticalAlignment = VerticalAlignmentStyle.Center;
        sheet.Cells[3, 0].Style.Font.Weight = 600;
        sheet.Cells[3, 0].Value = "ТОВАРНЫЙ ЧЕК";

        // Шапка таблицы

        sheet.Rows[4].SetHeight(15, LengthUnit.Point);
        sheet.Rows[4].Style.Borders.SetBorders(MultipleBorders.Top, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
        sheet.Rows[4].Style.Borders.SetBorders(MultipleBorders.Bottom, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);

        sheet.Cells[4, 0].Value = "Наименование товара";
        sheet.Cells[4, 0].Style.Borders.SetBorders(MultipleBorders.Right, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
        sheet.Cells[4, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

        sheet.Cells[4, 1].Value = "Кол-во";
        sheet.Cells[4, 1].Style.Borders.SetBorders(MultipleBorders.Right, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
        sheet.Cells[4, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

        sheet.Cells[4, 2].Value = "Цена";
        sheet.Cells[4, 2].Style.Borders.SetBorders(MultipleBorders.Right, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
        sheet.Cells[4, 2].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

        sheet.Cells[4, 3].Value = "Сумма";
        sheet.Cells[4, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

        // Заполнение таблицы

        int rowIndex = 5;

        foreach (OrderBakingModel baking in OrderModel.BakingCount)
        {
            sheet.Rows[rowIndex].SetHeight(15, LengthUnit.Point);
            sheet.Rows[rowIndex].Style.Borders.SetBorders(MultipleBorders.Bottom, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);

            sheet.Cells[rowIndex, 0].Value = baking.BakingModel.Name;
            sheet.Cells[rowIndex, 0].Style.Borders.SetBorders(MultipleBorders.Right, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);

            sheet.Cells[rowIndex, 1].Value = baking.Count;
            sheet.Cells[rowIndex, 1].Style.Borders.SetBorders(MultipleBorders.Right, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
            sheet.Cells[rowIndex, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

            sheet.Cells[rowIndex, 2].Value = baking.BakingModel.Cost;
            sheet.Cells[rowIndex, 2].Style.Borders.SetBorders(MultipleBorders.Right, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Thin);
            sheet.Cells[rowIndex, 2].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

            sheet.Cells[rowIndex, 3].Value = baking.BakingModel.Cost * baking.Count;
            sheet.Cells[rowIndex, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

            rowIndex++;
        }

        // Строка Всего

        sheet.Rows[rowIndex].SetHeight(20, LengthUnit.Point);
        sheet.Cells.GetSubrangeAbsolute(rowIndex, 0, rowIndex, 3).Merged = true;
        sheet.Cells[rowIndex++, 0].Value = $"Всего: {NumberToWordsConverter.Convert(OrderModel.Amount)} рублей";

        // Строка Подпись продавца

        sheet.Rows[rowIndex].SetHeight(20, LengthUnit.Point);
        sheet.Cells.GetSubrangeAbsolute(rowIndex, 0, rowIndex, 3).Merged = true;
        sheet.Cells.GetSubrangeAbsolute(rowIndex, 0, rowIndex, 3).Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
        sheet.Cells[rowIndex++, 0].Value = "Подпись продавца: ________________________";

        sheet.Cells.GetSubrangeAbsolute(0, 0, rowIndex, 3).Style.Font.Name = "Times New Roman";
        sheet.NamedRanges.SetPrintArea(sheet.Cells.GetSubrangeAbsolute(0, 0, rowIndex, 3));

        ExcelPrintOptions sheetPrintOptions = workbook.Worksheets[0].PrintOptions;
        sheetPrintOptions.HorizontalCentered = true;

        PrintDialog printDialog = new PrintDialog() { AllowSomePages = true };
        if (printDialog.ShowDialog() == DialogResult.OK)
        {
            PrinterSettings printerSettings = printDialog.PrinterSettings;
            PrintOptions printOptions = new PrintOptions() { SelectionType = SelectionType.EntireFile };

            printOptions.CopyCount = printerSettings.Copies;
            printOptions.FromPage = printerSettings.FromPage == 0 ? 0 : printerSettings.FromPage - 1;
            printOptions.ToPage = printerSettings.ToPage == 0 ? int.MaxValue : printerSettings.ToPage - 1;

            workbook.Print(printerSettings.PrinterName, printOptions);
        }

        await _orderRepository.AddOrderAsync(new Order()
        {
            Date = time,
            Amount = OrderModel.Amount
        },
            OrderModel.BakingCount.ToDictionary(b => b.BakingId, b => b.Count));
    }

    private void ClearOrder()
    {
        OrderModel.BakingCount.Clear();
        OrderModel.Amount = 0;
    }

    private void DecreaseBakingCount(object o)
    {
        OrderBakingModel orderBakingModel = (OrderBakingModel)o;
        OrderBakingModel orderBakingModelFromDict = OrderModel.BakingCount.First(b => b.BakingId == orderBakingModel.BakingId);
        if (orderBakingModelFromDict.Count == 1)
            OrderModel.BakingCount.Remove(orderBakingModel);
        else
            orderBakingModelFromDict.Count -= 1;
    }

    private void IncreaseBakingCount(object o)
    {
        OrderBakingModel orderBakingModel = (OrderBakingModel)o;
        OrderModel.BakingCount.First(b => b.BakingId == orderBakingModel.BakingId).Count += 1;
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
