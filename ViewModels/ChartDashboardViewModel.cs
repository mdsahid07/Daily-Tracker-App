using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using DailyTrackerApp.Models;
using DailyTrackerApp.Services;
using System.Windows.Input;
using DailyTrackerApp.Helpers;

namespace DailyTrackerApp.ViewModels
{
    public class ChartDashboardViewModel : INotifyPropertyChanged
    {
        private readonly DbService _db = new DbService();

        public SeriesCollection SeriesCollection { get; set; } = new();
        public List<string> Labels { get; set; } = new();
        public Func<double, string> Formatter { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public ICommand LoadWeeklyCommand { get; }
        public ICommand LoadMonthlyCommand { get; }
        public ICommand LoadCustomRangeCommand { get; }

        public ChartDashboardViewModel()
        {
            Formatter = value => value.ToString("F1");
            LoadWeeklyCommand = new RelayCommand(_ => LoadChartData(ChartRange.Weekly));
            LoadMonthlyCommand = new RelayCommand(_ => LoadChartData(ChartRange.Monthly));
            LoadCustomRangeCommand = new RelayCommand(_ => LoadChartData(ChartRange.Custom));
            LoadChartData(ChartRange.Weekly); // Default to weekly view
        }

        private enum ChartRange { Weekly, Monthly, Custom }

        private async void LoadChartData(ChartRange range)
        {
            var logs = await _db.LoadDailyLogsAsync();
            IEnumerable<DailyLog> filtered = logs;

            if (range == ChartRange.Weekly)
            {
                var weekStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (DateTime.Today.DayOfWeek == DayOfWeek.Sunday ? -6 : 1));
                filtered = logs.Where(log => log.LogDate >= weekStart && log.LogDate.DayOfWeek != DayOfWeek.Sunday);
            }
            else if (range == ChartRange.Monthly)
            {
                var monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                filtered = logs.Where(log => log.LogDate >= monthStart && log.LogDate.DayOfWeek != DayOfWeek.Sunday);
            }
            else if (range == ChartRange.Custom && StartDate.HasValue && EndDate.HasValue)
            {
                filtered = logs.Where(log => log.LogDate >= StartDate.Value && log.LogDate <= EndDate.Value && log.LogDate.DayOfWeek != DayOfWeek.Sunday);
            }

            var chartData = filtered.OrderBy(log => log.LogDate).ToList();
            Labels = chartData.Select(l => l.LogDate.ToString("MM/dd")).ToList();

            var redValues = new ChartValues<double>();
            var yellowValues = new ChartValues<double>();
            var greenValues = new ChartValues<double>();

            foreach (var log in chartData)
            {
                if (log.CombinedScore < 5)
                {
                    redValues.Add(log.CombinedScore);
                    yellowValues.Add(0);
                    greenValues.Add(0);
                }
                else if (log.CombinedScore < 8)
                {
                    redValues.Add(0);
                    yellowValues.Add(log.CombinedScore);
                    greenValues.Add(0);
                }
                else
                {
                    redValues.Add(0);
                    yellowValues.Add(0);
                    greenValues.Add(log.CombinedScore);
                }
            }

            SeriesCollection.Clear();
            SeriesCollection.Add(new ColumnSeries
            {
                Title = "Low (<5)",
                Values = redValues,
                Fill = Brushes.Red,
                MaxColumnWidth = 30
            });
            SeriesCollection.Add(new ColumnSeries
            {
                Title = "Medium (5-7.9)",
                Values = yellowValues,
                Fill = Brushes.Goldenrod,
                MaxColumnWidth = 30
            });
            SeriesCollection.Add(new ColumnSeries
            {
                Title = "High (8-10)",
                Values = greenValues,
                Fill = Brushes.Green,
                MaxColumnWidth = 30
            });

            OnPropertyChanged(nameof(SeriesCollection));
            OnPropertyChanged(nameof(Labels));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
