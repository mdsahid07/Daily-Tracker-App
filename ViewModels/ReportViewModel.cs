using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using DailyTrackerApp.Helpers;
using DailyTrackerApp.Models;
using DailyTrackerApp.Services;

namespace DailyTrackerApp.ViewModels
{
    public class ReportViewModel : INotifyPropertyChanged
    {
        private readonly DbService _db = new DbService();

        public ObservableCollection<DailyLog> Logs { get; set; } = new();
        private ObservableCollection<DailyLog> _allLogs = new();

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public ICommand ApplyFilterCommand { get; }
        public ICommand ThisWeekCommand { get; }
        public ICommand ThisMonthCommand { get; }
        public ICommand ClearFilterCommand { get; }

        public ReportViewModel()
        {
            ApplyFilterCommand = new RelayCommand(_ => ApplyFilter());
            ThisWeekCommand = new RelayCommand(_ => SetThisWeek());
            ThisMonthCommand = new RelayCommand(_ => SetThisMonth());
            ClearFilterCommand = new RelayCommand(_ => ClearFilter());

            LoadLogs();
        }

        private async void LoadLogs()
        {
            var result = await _db.LoadDailyLogsAsync();
            _allLogs = new ObservableCollection<DailyLog>(result);
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var filtered = _allLogs.AsEnumerable();

            if (StartDate.HasValue)
                filtered = filtered.Where(x => x.LogDate >= StartDate.Value);
            if (EndDate.HasValue)
                filtered = filtered.Where(x => x.LogDate <= EndDate.Value);

            Logs.Clear();
            foreach (var log in filtered.OrderByDescending(x => x.LogDate))
                Logs.Add(log);

            OnPropertyChanged(nameof(WeeklyAverageSummary));
            OnPropertyChanged(nameof(MonthlyAverageSummary));
        }

        private void SetThisWeek()
        {
            var today = DateTime.Today;

            // Set week start as Monday
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (today.DayOfWeek == DayOfWeek.Sunday ? -6 : 1));
            var endOfWeek = startOfWeek.AddDays(5); // Monday to Saturday only

            StartDate = startOfWeek;
            EndDate = today > endOfWeek ? endOfWeek : today;

            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(EndDate));
            ApplyFilter();
        }


        private void SetThisMonth()
        {
            var today = DateTime.Today;
            StartDate = new DateTime(today.Year, today.Month, 1);
            EndDate = today;
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(EndDate));
            ApplyFilter();
        }

        private void ClearFilter()
        {
            StartDate = null;
            EndDate = null;
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(EndDate));
            ApplyFilter();
        }

        public string WeeklyAverageSummary => CalculateAverage(LogDateFilter.Week);
        public string MonthlyAverageSummary => CalculateAverage(LogDateFilter.Month);

        private enum LogDateFilter { Week, Month }

        private string CalculateAverage(LogDateFilter filter)
        {
            var today = DateTime.Today;
            IEnumerable<DailyLog> filtered = _allLogs;

            if (filter == LogDateFilter.Week)
            {
                var weekStart = today.AddDays(-(int)today.DayOfWeek + (today.DayOfWeek == DayOfWeek.Sunday ? -6 : 1));
                var weekEnd = weekStart.AddDays(5); // Monday to Saturday
                filtered = _allLogs.Where(l =>
                    l.LogDate >= weekStart &&
                    l.LogDate <= weekEnd &&
                    l.LogDate.DayOfWeek != DayOfWeek.Sunday);
            }
            else if (filter == LogDateFilter.Month)
            {
                var monthStart = new DateTime(today.Year, today.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                filtered = _allLogs.Where(l =>
                    l.LogDate >= monthStart &&
                    l.LogDate <= monthEnd &&
                    l.LogDate.DayOfWeek != DayOfWeek.Sunday);
            }

            if (!filtered.Any()) return "No data";

            var avgTask = filtered.Average(x => x.TaskScore);
            var avgEval = filtered.Average(x => x.EvaluationAvg);
            var avgCombo = filtered.Average(x => x.CombinedScore);

            return $"Task: {avgTask:F1}, Eval: {avgEval:F1}, Combo: {avgCombo:F1}";
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
