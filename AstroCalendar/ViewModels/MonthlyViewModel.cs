using GalaSoft.MvvmLight.Command;
using AstroCalendar;
using AstroCalendar.Models;
using AstroCalendar.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AstroCalendar.ViewModels
{
    public class MonthlyViewModel : BaseViewModel
    {
        ObservableCollection<CalendItem> _calend;
        ObservableCollection<PhaseItem> _phases;
        DateTime _date;
        string _selectedtype;

        public ObservableCollection<CalendItem> Calend
        {
            get => _calend;
            set
            {
                _calend = value;
                OnPropertyChanged(nameof(_calend));
            }
        }

        public ObservableCollection<PhaseItem> Phases
        {
            get => _phases;
            set
            {
                _phases = value;
                OnPropertyChanged(nameof(_phases));
            }
        }

        public string Date
        {
            get => _date.ToString("MMMM, yyyy");
        }

        public string SelectedType
        {
            get => _selectedtype;
            set
            {
                _selectedtype = value;
                OnPropertyChanged(nameof(_selectedtype));
                Update();
            }
        }

        public ObservableCollection<string> TypeCol { get; set; }

        public ICommand UpdateCommand { get; set; }
        public ICommand BackwardCommand { get; set; }
        public ICommand ForwardCommand { get; set; }
        public ICommand ShowSunCommand { get; set; }
        public ICommand ShowMoonCommand { get; set; }

        public MonthlyViewModel()
        {
            Calend = new ObservableCollection<CalendItem>();
            Phases = new ObservableCollection<PhaseItem>();
            TypeCol = new ObservableCollection<string>
            {
                App.res.GetString("MonthlyTypeComboStr1"),
                App.res.GetString("MonthlyTypeComboStr2"),
                App.res.GetString("MonthlyTypeComboStr3"),
                App.res.GetString("MonthlyTypeComboStr4"),
                App.res.GetString("MonthlyTypeComboStr5"),
                App.res.GetString("MonthlyTypeComboStr6")
            };
            _date = DateTime.Now;
            SelectedType = TypeCol.First();

            UpdateCommand = new RelayCommand(Update);
            BackwardCommand = new RelayCommand(Backward);
            ForwardCommand = new RelayCommand(Forward);
            ShowSunCommand = new RelayCommand<CalendItem>(obj => ShowSun(obj));
            ShowMoonCommand = new RelayCommand<PhaseItem>(obj => ShowMooon(obj));

            Update();
        }

        private void Update()
        {
            Sun sun;
            Moon moon;

            Calend.Clear();
            Phases.Clear();

            //find dayofweek 1st day of month
            var firstday = new DateTime(_date.Year, _date.Month, 1);
            int dayscount = DateTime.DaysInMonth(_date.Year, _date.Month);
            int offset = ((int)firstday.DayOfWeek == 0) ? 6 : (int)firstday.DayOfWeek - 1;

            if (TypeCol.IndexOf(SelectedType) == 5) //Moon phases
            {
                //ofset
                for (int i = 0; i < offset; i++)
                    Phases.Add(null);

                //determine full and new moon date
                var result = Astro.GetFullNewMoonDate(firstday, LocationManager.Geoposition.Latitude,
                    LocationManager.Geoposition.Longitude, TimeZoneInfo.FindSystemTimeZoneById(LocationManager.Geoposition.TimeZone));

                //adding items                
                for (int j = 1; j <= dayscount; j++)
                {
                    moon = new Moon(new DateTime(_date.Year, _date.Month, j, 12, 0, 0), LocationManager.Geoposition.Latitude,
                        LocationManager.Geoposition.Longitude, TimeZoneInfo.FindSystemTimeZoneById(LocationManager.Geoposition.TimeZone));
                    sun = new Sun(new DateTime(_date.Year, _date.Month, j, 12, 0, 0, 0), LocationManager.Geoposition.Latitude,
                        LocationManager.Geoposition.Longitude, TimeZoneInfo.FindSystemTimeZoneById(LocationManager.Geoposition.TimeZone));
                    Phases.Add(new PhaseItem(Astro.GetMoonPhase(moon, sun), j, j == result.Item1.Day || j == result.Item2.Day));
                }
            }
            else
            {
                //offset
                for (int i = 0; i < offset; i++)
                    Calend.Add(null);

                //adding items
                for (int j = 1; j <= dayscount; j++)
                {
                    sun = new Sun(new DateTime(_date.Year, _date.Month, j, 12, 0, 0), LocationManager.Geoposition.Latitude,
                        LocationManager.Geoposition.Longitude, TimeZoneInfo.FindSystemTimeZoneById(LocationManager.Geoposition.TimeZone));

                    switch (TypeCol.IndexOf(SelectedType))
                    {
                        case 0: //sun dawn/dusks
                            Calend.Add(new CalendItem(j, sun.Dawn, sun.Dusk, !sun.Result.NoDawnDusk, !sun.Result.NoDawnDusk));
                            break;
                        case 1: //civil dawn/dusks
                            Calend.Add(new CalendItem(j, sun.CivilDawn, sun.CivilDusk, !sun.Result.NoCivil, !sun.Result.NoCivil));
                            break;
                        case 2: //nautical dawn/dusks
                            Calend.Add(new CalendItem(j, sun.NauticalDawn, sun.NauticalDusk, !sun.Result.NoNautical, !sun.Result.NoNautical));
                            break;
                        case 3: //astronomical dawn/dusks
                            Calend.Add(new CalendItem(j, sun.AstronomicalDawn, sun.AstronomicalDusk, !sun.Result.NoAstronomical, !sun.Result.NoAstronomical));
                            break;
                        case 4: //moon dawn/dusks
                            moon = new Moon(new DateTime(_date.Year, _date.Month, j, 12, 0, 0), LocationManager.Geoposition.Latitude,
                                    LocationManager.Geoposition.Longitude, TimeZoneInfo.FindSystemTimeZoneById(LocationManager.Geoposition.TimeZone));
                            Calend.Add(new CalendItem(j, moon.Dawn, moon.Dusk, !moon.Result.NoDawn, !moon.Result.NoDusk));
                            break;
                    }
                }
            }

        }

        void Backward()
        {
            _date = _date.AddMonths(-1);
            OnPropertyChanged(nameof(Date));
            Update();
        }

        void Forward()
        {
            _date = _date.AddMonths(1);
            OnPropertyChanged(nameof(Date));
            Update();
        }

        void ShowSun(CalendItem selected)
        {
            App.SelectedDate = new DateTime(_date.Year, _date.Month, selected.DayNum, 12, 0, 0);
            bool IsSun = true;
            Frame thisFrame = Window.Current.Content as Frame;
            thisFrame.Navigate(typeof(MainPage),IsSun);
        }

        void ShowMooon(PhaseItem selected)
        {
            App.SelectedDate = new DateTime(_date.Year, _date.Month, selected.DayNum, 12, 0, 0);
            bool IsSun = false;
            Frame thisFrame = Window.Current.Content as Frame;
            thisFrame.Navigate(typeof(MainPage), IsSun);
        }
    }
}
