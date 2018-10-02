using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace SunMoon.Models
{
    public class PhaseItem
    {
        public string Asset { get; set; }
        public int DayNum { get; set; }
        public Brush Color { get; set; }

        public PhaseItem(double illumination, int day, bool full_or_new)
        {
            DayNum = day;
            var result = Astro.GetMoonPhase(illumination);
            Asset = result.Item2;
            if (full_or_new)
                Color = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 46, 63, 242));
            else
                Color = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));

        }
    }
}
