using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunMoon.Models
{
    public class GeoPos
    {
        public Geoname[] geonames { get; set; }
    }

    public class Geoname
    {
        public string countryName { get; set; }
        public string adminName1 { get; set; }

        public double latitude { get; set; }
        public double longitude { get; set; }
    }


}
