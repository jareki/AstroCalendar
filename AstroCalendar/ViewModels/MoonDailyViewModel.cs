using AstroCalendar.Models;
using AstroCalendar.Views;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AstroCalendar.ViewModels
{
    public class MoonDailyViewModel : BaseViewModel
    {
        Moon _moon;
        Sun _sun;

        public string Date =>  App.SelectedDate.ToString("dd.MM.yyyy");
        public string DownTime => !_moon.Result.NoDawn ? _moon.Dawn.ToString("HH:mm") : "---";
        public string DuskTime => !_moon.Result.NoDusk ? _moon.Dusk.ToString("HH:mm") : "---";
        public string PhasePercent
        {
            get
            {
                double illumination = Astro.GetMoonPhase(_moon, _sun);
                int percent = (int)(illumination * 100);
                return $"{Math.Abs(percent)} %, {Astro.GetMoonPhase(illumination).Item1}";
            }
        }

        public string MoonIcon
        {
            get
            {
                double illumination = Astro.GetMoonPhase(_moon, _sun);
                return Astro.GetMoonPhase(illumination).Item2;
            }
        }

        public string FoolMoonDate => Astro.GetFullNewMoonDate( App.SelectedDate, 
                                                                LocationManager.Geoposition.Latitude,
                                                                LocationManager.Geoposition.Longitude,
                                                                TimeZoneInfo.FindSystemTimeZoneById(LocationManager.Geoposition.TimeZone))
                                                            .Item1.ToString("dd.MM.yy");
        public string NewMoonDate => Astro.GetFullNewMoonDate( App.SelectedDate,
                                                                LocationManager.Geoposition.Latitude,
                                                                LocationManager.Geoposition.Longitude,
                                                                TimeZoneInfo.FindSystemTimeZoneById(LocationManager.Geoposition.TimeZone))
                                                            .Item2.ToString("dd.MM.yy");

        public ICommand BackwardCommand { get; set; }
        public ICommand ForwardCommand { get; set; }
        public ICommand TodayCommand { get; set; }

        public MoonDailyViewModel()
        {
            BackwardCommand = new RelayCommand(Backward);
            ForwardCommand = new RelayCommand(Forward);
            TodayCommand = new RelayCommand(Today);
            Update();
        }

        void Update()
        {
            _moon = new Moon( App.SelectedDate, LocationManager.Geoposition.Latitude, LocationManager.Geoposition.Longitude, TimeZoneInfo.FindSystemTimeZoneById(LocationManager.Geoposition.TimeZone));
            _sun = new Sun( App.SelectedDate, LocationManager.Geoposition.Latitude, LocationManager.Geoposition.Longitude, TimeZoneInfo.FindSystemTimeZoneById(LocationManager.Geoposition.TimeZone));

            OnPropertyChanged(nameof(Date));
            OnPropertyChanged(nameof(DownTime));
            OnPropertyChanged(nameof(DuskTime));
            OnPropertyChanged(nameof(FoolMoonDate));
            OnPropertyChanged(nameof(MoonIcon));
            OnPropertyChanged(nameof(NewMoonDate));
            OnPropertyChanged(nameof(PhasePercent));
        }

        void Backward()
        {
            App.SelectedDate = App.SelectedDate.AddDays(-1);
            Update();
        }

        void Forward()
        {
            App.SelectedDate = App.SelectedDate.AddDays(1);
            Update();
        }

        void Today()
        {
            App.SelectedDate = DateTime.Now;
            Update();
        }
    }
}
