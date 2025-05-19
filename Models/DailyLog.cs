using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyTrackerApp.Models
{
    public class DailyLog
    {
        public int LogId { get; set; }
        public DateTime LogDate { get; set; }
        public int TaskScore { get; set; }
        public double EvaluationAvg { get; set; }
        public double CombinedScore { get; set; }
    }
}
