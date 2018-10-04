using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroCalendar.Models
{
    static class Astro
    {
        public const double Arcs = 3600.0 * 180.0 / Math.PI;
        public const double Moonmonth_hourcount = 708.73416;
        
        //Converts Ecliptic coordinates to equatorial
        public static Vector<double> ToEquatorial(Vector<double> ecliptic)
        {
            double eps = Rad(23.43929111);

            // Equatorial coordinates

            double x = Math.Cos(ecliptic.X) * Math.Cos(ecliptic.Y);
            double y = Math.Sin(ecliptic.X) * Math.Cos(ecliptic.Y);
            double z = Math.Sin(ecliptic.Y);

            double x1 = x;
            double y1 = Math.Cos(-eps) * y + Math.Sin(-eps) * z;
            double z1 = -Math.Sin(-eps) * y + Math.Cos(-eps) * z;

            double rhoSqr = x1 * x1 + y1 * y1;
            double phi, theta;

            // Norm of vector
            double m_r = Math.Sqrt(rhoSqr + z1 * z1);

            // Azimuth of vector
            if ((x1 == 0.0) && (y1 == 0.0))
                phi = 0.0;
            else
                phi = Math.Atan2(y1, x1);
            if (phi < 0.0)
                phi += 2.0 * Math.PI;

            // Altitude of vector
            double rho = Math.Sqrt(rhoSqr);
            if ((z1 == 0.0) && (rho == 0.0))
                theta = 0.0;
            else
                theta = Math.Atan2(z1, rho);

            return new Vector<double> { X = phi, Y = theta };
        }

        // Quad: Quadratic interpolation
        public static void Quad(double y_minus, double y_0, double y_plus, out double xe, out double ye, out double root1, out double root2, out int n_root)
        {
            double a, b, c, dis, dx;

            n_root = 0;
            root1 = root2 = 1;

            // Coefficients of interpolating parabola y=a*x^2+b*x+c
            a = 0.5 * (y_plus + y_minus) - y_0;
            b = 0.5 * (y_plus - y_minus);
            c = y_0;

            // Find extreme value
            xe = -b / (2.0 * a);
            ye = (a * xe + b) * xe + c;

            dis = b * b - 4.0 * a * c; // Discriminant of y=a*x^2+b*x+c

            if (dis >= 0) // Parabola has roots 
            {
                dx = 0.5 * Math.Sqrt(dis) / Math.Abs(a);

                root1 = xe - dx;
                root2 = xe + dx;

                if (Math.Abs(root1) <= 1.0) ++n_root;
                if (Math.Abs(root2) <= 1.0) ++n_root;
                if (root1 < -1.0) root1 = root2;
            }
        }

        //return Greenwich mean sidereal time
        //MJD -  Modified Julian Date
        public static double GMST(double MJD)
        {
            const double Secs = 86400.0;        // Seconds per day            
            double MJD_0, UT, T_0, T, gmst;

            MJD_0 = Math.Floor(MJD);
            UT = Secs * (MJD - MJD_0);     // [s]
            T_0 = (MJD_0 - 51544.5) / 36525.0;
            T = (MJD - 51544.5) / 36525.0;

            gmst = 24110.54841 + 8640184.812866 * T_0 + 1.0027379093 * UT
                    + (0.093104 - 6.2e-6 * T) * T * T;      // [sec]

            return (2 * Math.PI / Secs) * (gmst % Secs);   // [Rad]
        }

        //return Modified Julian Date
        public static double Mjd(DateTime date)
        {
            long MjdMidnight;
            double FracOfDay;
            int b;
            int Year = date.Year;
            int Month = date.Month;
            int Day = date.Day;


            if (Month <= 2) { Month += 12; --Year; }

            if ((10000L * Year + 100L * Month + Day) <= 15821004L)
                b = -2 + ((Year + 4716) / 4) - 1179;     // Julian calendar 
            else
                b = (Year / 400) - (Year / 100) + (Year / 4);  // Gregorian calendar 

            MjdMidnight = 365L * Year - 679004L + b + (int)(30.6001 * (Month + 1)) + Day;

            FracOfDay = (date.Hour + date.Minute / 60.0 + date.Second / 3600.0) / 24.0;

            return MjdMidnight + FracOfDay;
        }

        //return percent of illumination (from 0 to +-1.0)
        //return positive value if moon is waxing crescent
        //and negative - if waning moon
        public static double GetMoonPhase(Moon m, Sun s)
        {
            var delta_lon = s.EclipLon - m.EclipLon;
            if (delta_lon < 0)
                delta_lon += 2 * Math.PI;

            if (delta_lon <= Math.PI)
                return delta_lon / Math.PI;
            else
                return (delta_lon - 2 * Math.PI) / Math.PI;
        }
        
        /// <summary>
        /// return MoonPhase-name and Asset
        /// </summary>
        /// <param name="illumination">MoonPhase illumination (from 0 to +-1.0)</param>
        /// <returns>MoonPhase-name and Asset in Tuple</returns>
        public static Tuple<string,string> GetMoonPhase(double illumination)
        {
            int percent = (int)(illumination * 100);
            string phasename="";
            string asset="";

            if (Math.Abs(percent) < 5)
            {
                phasename = App.res.GetString("new-moon");
                asset = "ms-appx:///Assets/new-moon.png";
            }
            else if (Math.Abs(percent) > 95)
            {
                phasename = App.res.GetString("full-moon");
                asset = "ms-appx:///Assets/full-moon.png";
            }
            else if (percent >= 5 && percent <= 25)
            {
                phasename = App.res.GetString("waningcrescent-moon");
                asset = "ms-appx:///Assets/waningcrescent-moon.png";
            }
            else if (percent > 25 && percent <= 75)
            {
                phasename = App.res.GetString("waningquarter-moon");;
                asset = "ms-appx:///Assets/waningquarter-moon.png";
            }
            else if (percent > 75 && percent <= 95)
            {
                phasename = App.res.GetString("waninggibbous-moon");
                asset = "ms-appx:///Assets/waninggibbous-moon.png";
            }
            else if (percent < -75 && percent >= -95)
            {
                phasename = App.res.GetString("waxinggibbous-moon");
                asset = "ms-appx:///Assets/waxinggibbous-moon.png";
            }
            else if (percent < -25 && percent >= -75)
            {
                phasename = App.res.GetString("waxingquarter-moon");
                asset = "ms-appx:///Assets/waxingquarter-moon.png";
            }
            else if (percent <= -5 && percent >= -25)
            {
                phasename = App.res.GetString("waxingcrescent-moon");
                asset = "ms-appx:///Assets/waxingcrescent-moon.png";
            }

            return new Tuple<string, string>(phasename, asset);
        }
        
        /// <summary>
        /// return Nearest (30 days period) FullMoon Date and NewMoon Date
        /// </summary>
        /// <returns>Full Moon Date and New Moon Date</returns>
        public static Tuple<DateTime,DateTime> GetFullNewMoonDate(DateTime date, double latitude, double longitude, TimeZoneInfo timezone)
        {

            //go to the night 00:00
            DateTime new_date = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);

            
            Sun sun = new Sun(new_date, latitude, longitude, timezone);
            Moon moon = new Moon(new_date, latitude, longitude, timezone);

            double illumination = GetMoonPhase(moon, sun);

            //FULL MOON
            DateTime fullmoon_date = new_date.AddHours((1.0 + illumination) * Moonmonth_hourcount / 2);

            //NEW MOON      
            DateTime newmoon_date;
            if (illumination < 0)
                newmoon_date = new_date.AddHours((2.0 + illumination) * Moonmonth_hourcount / 2);
            else
                newmoon_date = new_date.AddHours((illumination) * Moonmonth_hourcount / 2);
            
            return new Tuple<DateTime, DateTime>(fullmoon_date, newmoon_date);
        }

        public static double Frac(double n) => n - Math.Floor(n);
        public static double Deg(double rad) => (rad * 180) / Math.PI;
        public static double Rad(double deg) => (Math.PI  / 180) * deg;
    }
}
