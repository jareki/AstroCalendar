using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroCalendar.Models
{
    public class CalendItem
    {
        public string Rise { get; set; }
        public string Set { get; set; }
        public int DayNum { get; set; }

        public CalendItem(int day, DateTime rise, DateTime set, bool isset, bool isrise)
        {
            Rise = isrise ? rise.ToString("H:mm") : "--:--";
            Set = isset ? set.ToString("H:mm") : "--:--";
            DayNum = day;
        }
    }
}
