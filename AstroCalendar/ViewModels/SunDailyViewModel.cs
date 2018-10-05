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
    public class SunDailyViewModel : BaseViewModel
    {
        Sun _sun;

        public string Date =>  App.SelectedDate.ToString("dd.MM.yyyy");
        public string DawnTime => !_sun.Result.NoDawnDusk ? _sun.Dawn.ToString("HH:mm") : "---";
        public string DuskTime => !_sun.Result.NoDawnDusk ? _sun.Dusk.ToString("HH:mm") : "---";
        public string NoonTime => !_sun.Result.NoDawnDusk ? _sun.Noon.ToString("HH:mm") : "---";
        public string LengthTime
        {
            get
            {
                if (_sun.Dawn < _sun.Dusk)
                    return !_sun.Result.NoDawnDusk ? (_sun.Dusk - _sun.Dawn).ToString(@"hh\:mm") : "---";
                else
                    return !_sun.Result.NoDawnDusk ? (new TimeSpan(24, 0, 0) - (_sun.Dawn - _sun.Dusk)).ToString(@"hh\:mm") : "---";
            }
        }

        public string CivilDawnTime => !_sun.Result.NoCivil ? _sun.CivilDawn.ToString("HH:mm") : "---";
        public string CivilDuskTime => !_sun.Result.NoCivil? _sun.CivilDusk.ToString("HH:mm") : "---";
        public string NauticalDawnTime => !_sun.Result.NoNautical? _sun.NauticalDawn.ToString("HH:mm") : "---";
        public string NauticalDuskTime => !_sun.Result.NoNautical? _sun.NauticalDusk.ToString("HH:mm") : "---";
        public string AstroDawnTime => !_sun.Result.NoAstronomical? _sun.AstronomicalDawn.ToString("HH:mm") : "---";
        public string AstroDuskTime => !_sun.Result.NoAstronomical? _sun.AstronomicalDusk.ToString("HH:mm") : "---";

        public ICommand BackwardCommand { get; set; }
        public ICommand ForwardCommand { get; set; }
        public ICommand TodayCommand { get; set; }

        public SunDailyViewModel()
        {
            BackwardCommand = new RelayCommand(Backward);
            ForwardCommand = new RelayCommand(Forward);
            TodayCommand = new RelayCommand(Today);
            Update();
        }

        void Update()
        {
            _sun = new Sun( App.SelectedDate, LocationManager.Geoposition.Latitude, LocationManager.Geoposition.Longitude, TimeZoneInfo.FindSystemTimeZoneById(LocationManager.Geoposition.TimeZone));

            OnPropertyChanged(nameof(Date));
            OnPropertyChanged(nameof(DawnTime));
            OnPropertyChanged(nameof(DuskTime));
            OnPropertyChanged(nameof(AstroDawnTime));
            OnPropertyChanged(nameof(AstroDuskTime));
            OnPropertyChanged(nameof(CivilDawnTime));
            OnPropertyChanged(nameof(NauticalDawnTime));
            OnPropertyChanged(nameof(NauticalDuskTime));
            OnPropertyChanged(nameof(NoonTime));
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
