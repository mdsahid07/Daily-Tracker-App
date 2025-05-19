using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyTrackerApp.Models
{
    public class EvaluationEntry
    {
        public int EvaluationId { get; set; }
        public string Question { get; set; }
        public string Category { get; set; }
        public int Score { get; set; }
    }
}
