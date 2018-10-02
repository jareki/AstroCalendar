using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunMoon.Models
{
    [Table("Location")]
    public class Location
    {
        string timezone;

        public int Id { get; set; }
        public string TimeZone
        {
            get
            {
                return string.IsNullOrEmpty(timezone) ? TimeZoneInfo.Utc.Id : timezone;
            }
            set
            {
                timezone = value;
            }
        }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
