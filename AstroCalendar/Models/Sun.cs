using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroCalendar.Models
{
    class SunResult
    {
        public bool NoDawnDusk { get; set; }
        public bool NoCivil { get; set; }
        public bool NoNautical { get; set; }
        public bool NoAstronomical { get; set; }
    }
    class Sun
    {
        public DateTime Date { get; set; }
        public DateTime Dusk { get; set; }
        public DateTime Dawn { get; set; }
        public DateTime Noon { get; set; }
        public DateTime CivilDawn { get; set; }
        public DateTime NauticalDawn { get; set; }
        public DateTime AstronomicalDawn { get; set; }
        public DateTime CivilDusk { get; set; }
        public DateTime NauticalDusk { get; set; }
        public DateTime AstronomicalDusk { get; set; }
        public SunResult Result { get; set; }

        public double EclipLon { get; set; }

        public Sun(DateTime date, double latitude, double longitude, TimeZoneInfo timezone)
        {
            //Date = new DateTime(date.Year,date.Month,date.Day) + GetTimeEquation(date);
            Date = date;
            EclipLon = -1;
            double[] rises = new double[] { 0, 0, 0, 0 };
            double[] sets = new double[] { 0, 0, 0, 0 };
            bool[] isrises = new bool[] { false, false, false, false };
            bool[] issets = new bool[] { false, false, false, false };

            double[] h_correct = new double[]
            {
                Astro.Rad(-50/60.0), Astro.Rad(-6), Astro.Rad(-12),Astro.Rad(-18)
            };

            double hour = 1.0;
            double y_minus, y_0, y_plus;
            double xe = 0, ye = 0, root1 = 0, root2 = 0;
            int nRoot = 0;

            double MJD0h = Astro.Mjd(date) - timezone.GetUtcOffset(date).TotalHours / 24.0;


            for (int i = 0; i < 4; i++)
            {
                hour = 1.0;
                y_minus = SinAlt(MJD0h, hour - 1.0, latitude, longitude) - Math.Sin(h_correct[i]);
                // loop over search intervals from [0h-2h] to [22h-24h]
                do
                {

                    y_0 = SinAlt(MJD0h, hour, latitude, longitude) - Math.Sin(h_correct[i]);
                    y_plus = SinAlt(MJD0h, hour + 1.0, latitude, longitude) - Math.Sin(h_correct[i]);

                    // find parabola through three values y_minus,y_0,y_plus
                    Astro.Quad(y_minus, y_0, y_plus, out xe, out ye, out root1, out root2, out nRoot);

                    if (nRoot == 1)
                    {
                        if (y_minus < 0.0)
                        {
                            rises[i] = hour + root1;
                            isrises[i] = true;
                        }
                        else
                        {
                            sets[i] = hour + root1;
                            issets[i] = true;
                        }
                    }

                    if (nRoot == 2)
                    {
                        if (ye < 0.0)
                        {
                            rises[i] = hour + root2;
                            sets[i] = hour + root1;
                        }
                        else
                        {
                            rises[i] = hour + root1;
                            sets[i] = hour + root2;
                        }
                        isrises[i] = true;
                        issets[i] = true;
                    }

                    y_minus = y_plus;     // prepare for next interval
                    hour += 2.0;

                }
                while (!((hour == 25.0) || (isrises[i] && issets[i])));
            }
            Dawn = Date.AddHours(rises[0]);
            Dusk = Date.AddHours(sets[0]);
            Noon = Dawn.AddHours((sets[0] - rises[0]) / 2);
            if (Dusk.Day != Dawn.Day)
                Noon = Noon.AddHours(12);

            CivilDawn = Date.AddHours(rises[1]);
            CivilDusk = Date.AddHours(sets[1]);
            NauticalDawn = Date.AddHours(rises[2]);
            NauticalDusk = Date.AddHours(sets[2]);
            AstronomicalDawn = Date.AddHours(rises[3]);
            AstronomicalDusk = Date.AddHours(sets[3]);

            Result = new SunResult
            {
                NoDawnDusk = !isrises[0] || !issets[0],
                NoCivil = !isrises[1] || !issets[1],
                NoNautical = !isrises[2] | !issets[2],
                NoAstronomical = !isrises[3] | !issets[3],
            };
        }

        //return Sine of the altitude
        //mJD0h - is Modified Julian Date on 00:00
        //hour - is adding hour to 00:00
        private double SinAlt(double mJD0h, double hour, double latitude, double longitude)
        {
            double MJD, T, RA, Declination, tau;


            MJD = mJD0h + hour / 24.0;
            T = (MJD - 51544.5) / 36525.0;

            var v = GetSunCoor(T);
            EclipLon = EclipLon == -1 ? v.X : EclipLon;
            v = Astro.ToEquatorial(v);

            RA = v.X;
            Declination = v.Y;
            tau = Astro.GMST(MJD) + Astro.Rad(longitude) - RA;

            return (Math.Sin(Astro.Rad(latitude)) * Math.Sin(Declination) + Math.Cos(Astro.Rad(latitude)) * Math.Cos(Declination) * Math.Cos(tau));
        }

        //return Ecliptic Coordinates with necessary orbital corrections
        //T - Time in Julian centuries since J2000
        Vector<double> GetSunCoor(double T)
        {
            // Mean anomaly and ecliptic longitude  
            double M = 2 * Math.PI * Astro.Frac(0.993133 + 99.997361 * T);
            double L = 2 * Math.PI * Astro.Frac(0.7859453 + M / (Math.PI * 2) +
                              (6893.0 * Math.Sin(M) + 72.0 * Math.Sin(2.0 * M) + 6191.2 * T) / 1296.0e3);

            return new Vector<double> { X = L, Y = 0 };
        }

        public static TimeSpan GetTimeEquation(DateTime date)
        {
            double b = 360 * (date.DayOfYear - 81) / 365;
            double t = 7.53 * Math.Cos(Astro.Rad(b)) + 1.5 * Math.Sin(Astro.Rad(b)) - 9.87 * Math.Sin(Astro.Rad(2 * b));
            return new TimeSpan(0, (int)t, 0);
        }
    }
}
