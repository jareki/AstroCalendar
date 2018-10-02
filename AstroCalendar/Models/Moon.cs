using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunMoon.Models
{
    class MoonResult
    {
        public bool NoDusk { get; set; }
        public bool NoDawn { get; set; }
    }

    class Moon
    {
        public DateTime Date { get; set; }
        public DateTime Dusk { get; set; }
        public DateTime Dawn { get; set; }
        public MoonResult Result { get; set; }
        public double EclipLon { get; set; }

        public Moon(DateTime date, double latitude, double longitude, TimeZoneInfo timezone)
        {
            Date = date;
            EclipLon = -1;
            double rise=0, set=0;
            bool isrise = false, isset = false;

            double h_correct = Astro.Rad(8 / 60.0);

            double hour = 1.0;            
            double y_minus, y_0, y_plus;
            double xe=0, ye=0, root1=0, root2=0;
            int nRoot=0;

            double MJD0h = Astro.Mjd(date) - timezone.GetUtcOffset(date).TotalHours / 24.0;
            y_minus = SinAlt(MJD0h, hour - 1.0, latitude, longitude) - Math.Sin(h_correct);

            // loop over search intervals from [0h-2h] to [22h-24h]
            do
            {

                y_0 = SinAlt( MJD0h, hour, latitude, longitude) - Math.Sin(h_correct);
                y_plus = SinAlt(MJD0h, hour + 1.0, latitude, longitude) - Math.Sin(h_correct);

                // find parabola through three values y_minus,y_0,y_plus
                Astro.Quad(y_minus, y_0, y_plus, out xe, out ye, out root1, out root2, out nRoot);

                if (nRoot == 1)
                {
                    if (y_minus < 0.0)
                    {
                        rise = hour + root1;
                        isrise = true;
                    }
                    else
                    {
                        set = hour + root1;
                        isset = true;
                    }
                }

                if (nRoot == 2)
                {
                    if (ye < 0.0)
                    {
                        rise = hour + root2;
                        set = hour + root1;
                    }
                    else
                    {
                        rise = hour + root1;
                        set = hour + root2;
                    }
                    isrise = true;
                    isset = true;
                }

                y_minus = y_plus;     // prepare for next interval
                hour += 2.0;

            }
            while (!((hour == 25.0) || (isrise && isset)));

            Dawn = Date.AddHours(rise);
            Dusk = Date.AddHours(set);

            Result = new MoonResult { NoDawn = !isrise, NoDusk = !isset };
            var v = GetMoonDay(Date);
        }

        //return Sine of the altitude
        //mJD0h - is Modified Julian Date on 00:00
        //hour - is adding hour to 00:00
        private double SinAlt(double mJD0h, double hour, double latitude, double longitude)
        {
            double MJD, T, RA, Declination, tau;


            MJD = mJD0h + hour / 24.0;
            T = (MJD - 51544.5) / 36525.0;

            var v = GetMoonCoor(T);
            EclipLon = EclipLon == -1 ? v.X: EclipLon;
            v = Astro.ToEquatorial(v);

            RA = v.X;
            Declination = v.Y;
            tau = Astro.GMST(MJD) + Astro.Rad(longitude) - RA;

            return (Math.Sin(Astro.Rad(latitude)) * Math.Sin(Declination) + Math.Cos(Astro.Rad(latitude)) * Math.Cos(Declination) * Math.Cos(tau));
        }

        int GetMoonDay(DateTime date)
        {
            double eq, eq1, eq2;
            int monthH = date.Month;
            int yearH = date.Year;
            if (date.Month <= 2)
            {
                monthH += 12;
                yearH--;
            }
            eq = Math.Floor(yearH / 100.0);
            eq1 = Math.Floor(eq / 3) + Math.Floor(eq / 4) + 6 - eq;
            eq2 = (Math.Round(Astro.Frac(yearH / eq) * 209) + monthH + eq1 + date.Day) / 30;
            return (int)(Astro.Frac(eq2) * 30 + 1);
        }

        //return Ecliptic Coordinates with necessary orbital corrections
        //T - Time in Julian centuries since J2000
        Vector<double> GetMoonCoor(double T)
        {
            double Arcs = 3600.0 * 180.0 / Math.PI;

            double L_0, l, ls, F, D, dL, S, h, N;

            // Mean elements of lunar orbit
            L_0 = Astro.Frac(0.606433 + 1336.855225 * T);       // mean longitude [rev]

            l = 2 * Math.PI * Astro.Frac(0.374897 + 1325.552410 * T);  // Moon's mean anomaly 
            ls = 2 * Math.PI * Astro.Frac(0.993133 + 99.997361 * T);  // Sun's mean anomaly 
            D = 2 * Math.PI * Astro.Frac(0.827361 + 1236.853086 * T);  // Diff. long. Moon-Sun 
            F = 2 * Math.PI * Astro.Frac(0.259086 + 1342.227825 * T);  // Dist. from ascending node 


            // Perturbations in longitude and latitude
            dL = +22640 * Math.Sin(l) - 4586 * Math.Sin(l - 2 * D) + 2370 * Math.Sin(2 * D) + 769 * Math.Sin(2 * l)
                 - 668 * Math.Sin(ls) - 412 * Math.Sin(2 * F) - 212 * Math.Sin(2 * l - 2 * D) - 206 * Math.Sin(l + ls - 2 * D)
                 + 192 * Math.Sin(l + 2 * D) - 165 * Math.Sin(ls - 2 * D) - 125 * Math.Sin(D) - 110 * Math.Sin(l + ls)
                 + 148 * Math.Sin(l - ls) - 55 * Math.Sin(2 * F - 2 * D);
            S = F + (dL + 412 * Math.Sin(2 * F) + 541 * Math.Sin(ls)) / Arcs;
            h = F - 2 * D;
            N = -526 * Math.Sin(h) + 44 * Math.Sin(l + h) - 31 * Math.Sin(-l + h) - 23 * Math.Sin(ls + h)
                 + 11 * Math.Sin(-ls + h) - 25 * Math.Sin(-2 * l + F) + 21 * Math.Sin(-l + F);

            // Ecliptic longitude and latitude
            Vector<double> v = new Vector<double>();
            v.X = 2 * Math.PI * Astro.Frac(L_0 + dL / 1296.0e3); // [rad]
            v.Y = (18520.0 * Math.Sin(S) + N) / Arcs;   // [rad]
            return v;
        }                
    }
}
