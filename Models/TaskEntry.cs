using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyTrackerApp.Models
{
    public class TaskEntry
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string TimeSlot { get; set; }
        public bool IsCompleted { get; set; }
    }
}
