using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DailyTrackerApp.Models
{
    public class TaskItem : INotifyPropertyChanged
    {
        public int TaskId { get; set; }

        // ✅ match column names exactly for Dapper
        public string TaskName { get; set; }  // was 'Task'
        public string TimeSlot { get; set; }  // was 'Time'

        private bool isCompleted;
        public bool IsCompleted
        {
            get => isCompleted;
            set
            {
                if (isCompleted != value)
                {
                    isCompleted = value;
                    OnPropertyChanged(nameof(IsCompleted));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class TaskLog
    {
        public int TaskId { get; set; }
        public bool IsCompleted { get; set; }
    }

}
