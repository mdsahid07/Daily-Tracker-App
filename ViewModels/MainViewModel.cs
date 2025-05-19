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
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DbService _dbService = new();

        public ObservableCollection<TaskItem> Tasks { get; set; } = new();
        public ObservableCollection<EvaluationItem> Evaluations { get; set; } = new();
        public List<int> ScoreOptions { get; } = Enumerable.Range(0, 11).ToList(); // 0 to 10

        public ICommand SaveLogCommand { get; }

        private bool isSaving;
        public bool IsSaving
        {
            get => isSaving;
            set { isSaving = value; OnPropertyChanged(); }
        }

        private bool hasLoggedToday;
        public bool HasLoggedToday
        {
            get => hasLoggedToday;
            set { hasLoggedToday = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            SaveLogCommand = new RelayCommand(async _ => await SaveLogAsync(), _ => !IsSaving);
            _ = LoadAndInitializeAsync();
        }

        private async Task LoadDefaultTasksAsync()
        {
            Tasks.Clear();
            var taskList = await _dbService.LoadTasksAsync();
            foreach (var task in taskList)
            {
                Tasks.Add(new TaskItem
                {
                    TaskId = task.TaskId,
                    TaskName = task.TaskName,
                    TimeSlot = task.TimeSlot
                });
            }
        }



        private async Task LoadDefaultEvaluationsAsync()
        {
            Evaluations.Clear();
            var evalList = await _dbService.LoadEvaluationsAsync();
            foreach (var eval in evalList)
            {
                Evaluations.Add(new EvaluationItem
                {
                    EvaluationId = eval.EvaluationId,
                    Category = eval.Category,
                    Question = eval.Question
                });
            }



        }

        public async Task SaveLogAsync()
        {
            try
            {
                IsSaving = true;

                int totalTasks = Tasks.Count;
                int completedTasks = Tasks.Count(t => t.IsCompleted);
                double evalAvg = Evaluations.Count > 0 ? Evaluations.Average(e => e.Score) : 0;

                double combined = 0;
                if (totalTasks > 0)
                {
                    double taskScoreOutOfFive = (completedTasks / (double)totalTasks) * 5;
                    double evalScoreOutOfFive = (evalAvg / 10) * 5;
                    combined = Math.Round(taskScoreOutOfFive + evalScoreOutOfFive, 2);
                }

                var log = new DailyLog
                {
                    LogDate = DateTime.Today,
                    TaskScore = completedTasks,
                    EvaluationAvg = evalAvg,
                    CombinedScore = combined
                };

                await _dbService.SaveOrUpdateDailyLogAsync(log);

                foreach (var task in Tasks)
                {
                    await _dbService.SaveOrUpdateTaskLogAsync(log.LogId, task.TaskId, task.IsCompleted);
                }

                foreach (var eval in Evaluations)
                {
                    await _dbService.SaveOrUpdateEvaluationLogAsync(log.LogId, eval.EvaluationId, eval.Score);
                }

                HasLoggedToday = true;
                System.Windows.MessageBox.Show("Log saved successfully!");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                IsSaving = false;
            }
        }

        private async Task LoadAndInitializeAsync()
        {
            await LoadDefaultTasksAsync();
            await LoadDefaultEvaluationsAsync();
            await LoadTodayDataAsync();
        }

        public async Task LoadTodayDataAsync()
        {
            var today = DateTime.Today;
            var logs = await _dbService.LoadDailyLogsAsync();
            var todayLog = logs.FirstOrDefault(l => l.LogDate.Date == today);

            HasLoggedToday = todayLog != null;

            if (todayLog != null)
            {
                var taskLogs = await _dbService.LoadTaskLogsAsync(todayLog.LogId);
                foreach (var task in Tasks)
                {
                    var log = taskLogs.FirstOrDefault(t => t.TaskId == task.TaskId);
                    if (log != null)
                        task.IsCompleted = log.IsCompleted;
                }

                var evalLogs = await _dbService.LoadEvaluationLogsAsync(todayLog.LogId);
                foreach (var eval in Evaluations)
                {
                    var log = evalLogs.FirstOrDefault(e => e.EvaluationId == eval.EvaluationId);
                    if (log != null)
                        eval.Score = log.Score;
                }

                OnPropertyChanged(nameof(Tasks));
                OnPropertyChanged(nameof(Evaluations));
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
