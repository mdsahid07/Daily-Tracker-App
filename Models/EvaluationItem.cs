using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DailyTrackerApp.Models
{
    public class EvaluationItem : INotifyPropertyChanged
    {
        public int EvaluationId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;

        private int score;
        public int Score
        {
            get => score;
            set
            {
                if (score != value)
                {
                    score = value;
                    OnPropertyChanged(nameof(Score));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
 
    public class EvaluationLog
    {
        public int EvaluationId { get; set; }
        public int Score { get; set; }
    }

}
