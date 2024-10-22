using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using BakingApplication.ViewModels;

namespace BakingApplication.Views
{
    /// <summary>
    /// Логика взаимодействия для StatisticsWindow.xaml
    /// </summary>
    public partial class StatisticsWindow : Window
    {
        public StatisticsWindow()
        {
            InitializeComponent();
        }

        private void MoveChartCursorAndToolTip_OnMouseMove(object sender, MouseEventArgs e)
        {
            var chart = sender as CartesianChart;

            if (!TryFindVisualChildElement(chart, out Canvas outerCanvas) ||
                !TryFindVisualChildElement(outerCanvas, out Canvas graphPlottingArea))
            {
                return;
            }
            Point chartMousePosition = e.GetPosition(chart);

            // Add the cursor for the x-axis.
            // Since Chart internally reverses the screen coordinates
            // to match chart's coordinate system
            // and this coordinate system orientation applies also to Chart.VisualElements,
            // the UIElements like Popup and Line are added directly to the plotting canvas.
            if (chart.TryFindResource("CursorX") is Line cursorX
              && !graphPlottingArea.Children.Contains(cursorX))
            {
                graphPlottingArea.Children.Add(cursorX);
            }

            if (!(chart.TryFindResource("CursorXToolTip") is FrameworkElement cursorXToolTip))
            {
                return;
            }

            // Add the cursor for the x-axis.
            // Since Chart internally reverses the screen coordinates
            // to match chart's coordinate system
            // and this coordinate system orientation applies also to Chart.VisualElements,
            // the UIElements like Popup and Line are added directly to the plotting canvas.
            if (!graphPlottingArea.Children.Contains(cursorXToolTip))
            {
                graphPlottingArea.Children.Add(cursorXToolTip);
            }

            // Position the ToolTip
            Point canvasMousePosition = e.GetPosition(graphPlottingArea);
            Canvas.SetLeft(cursorXToolTip, canvasMousePosition.X - cursorXToolTip.ActualWidth);
            Canvas.SetTop(cursorXToolTip, canvasMousePosition.Y);
        }

        private bool TryFindVisualChildElement<TChild>(DependencyObject parent, out TChild resultElement)
    where TChild : DependencyObject
        {
            resultElement = null;
            for (var childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(parent); childIndex++)
            {
                DependencyObject childElement = VisualTreeHelper.GetChild(parent, childIndex);

                if (childElement is Popup popup)
                {
                    childElement = popup.Child;
                }

                if (childElement is TChild)
                {
                    resultElement = childElement as TChild;
                    return true;
                }

                if (TryFindVisualChildElement(childElement, out resultElement))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
