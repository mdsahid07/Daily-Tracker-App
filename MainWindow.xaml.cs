using DailyTrackerApp.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DailyTrackerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var vm = new MainViewModel();
            DataContext = vm;
            _ = vm.LoadTodayDataAsync(); // Load today's data

        }
        private void ViewReports_Click(object sender, RoutedEventArgs e)
        {
            var reportWindow = new Views.ReportView();
            reportWindow.Show();
        }
        private void ViewChart_Click(object sender, RoutedEventArgs e)
        {
            var chartWindow = new Views.ChartDashboard();
            chartWindow.Show();
        }

    }
}