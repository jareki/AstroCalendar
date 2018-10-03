using SunMoon.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SunMoon.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MonthlyPage : Page
    {
        ObservableCollection<CalendItem> Calend;
        ObservableCollection<PhaseItem> Phases;
        DateTime Date;
        private double distX;

        public MonthlyPage()
        {
            this.InitializeComponent();

            Calend = new ObservableCollection<CalendItem>();
            Phases = new ObservableCollection<PhaseItem>();
            Date = DateTime.Now;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TypeCombo.Items.Clear();
            TypeCombo.Items.Add(App.res.GetString("MonthlyTypeComboStr1"));
            TypeCombo.Items.Add(App.res.GetString("MonthlyTypeComboStr2"));
            TypeCombo.Items.Add(App.res.GetString("MonthlyTypeComboStr3"));
            TypeCombo.Items.Add(App.res.GetString("MonthlyTypeComboStr4"));
            TypeCombo.Items.Add(App.res.GetString("MonthlyTypeComboStr5"));
            TypeCombo.Items.Add(App.res.GetString("MonthlyTypeComboStr6"));

            TypeCombo.SelectedIndex = 0; 
        }

        private void Update()
        {
            Sun sun;
            Moon moon;

            Calend.Clear();
            Phases.Clear();

            DateTxt.Text = Date.ToString("MMMM, yyyy");

            //find dayofweek 1st day of month
            var firstday = new DateTime(Date.Year, Date.Month, 1);
            int dayscount = DateTime.DaysInMonth(Date.Year, Date.Month);
            int offset = ((int)firstday.DayOfWeek == 0) ? 6 : (int)firstday.DayOfWeek - 1;

            if (TypeCombo.SelectedIndex==5) //Moon phases
            {
                //ofset
                for (int i = 0; i < offset; i++)
                    Phases.Add(null);

                //determine full and new moon date
                var result = Astro.GetFullNewMoonDate(firstday, LocationManager.Geoposition.Latitude, 
                    LocationManager.Geoposition.Longitude, TimeZoneInfo.FindSystemTimeZoneById(LocationManager.Geoposition.TimeZone));
                
                //adding items                
                for (int j=1;j<=dayscount;j++)
                {
                    moon = new Moon(new DateTime(Date.Year, Date.Month, j, 12, 0, 0), LocationManager.Geoposition.Latitude,
                        LocationManager.Geoposition.Longitude, TimeZoneInfo.FindSystemTimeZoneById(LocationManager.Geoposition.TimeZone));
                    sun = new Sun(new DateTime(Date.Year, Date.Month, j, 12, 0, 0, 0), LocationManager.Geoposition.Latitude,
                        LocationManager.Geoposition.Longitude, TimeZoneInfo.FindSystemTimeZoneById(LocationManager.Geoposition.TimeZone));
                    Phases.Add(new PhaseItem(Astro.GetMoonPhase(moon, sun), j, j==result.Item1.Day || j==result.Item2.Day));
                }

                //switch to PhasesGrid
                CalendarGrid.Visibility = Visibility.Collapsed;
                PhaseGrid.Visibility = Visibility.Visible;
            }
            
            else
            {
                //offset
                //ofset
                for (int i = 0; i < offset; i++)
                    Calend.Add(null);

                //adding items
                for (int j = 1; j <= dayscount; j++)
                {
                    sun = new Sun(new DateTime(Date.Year, Date.Month, j, 12, 0, 0), LocationManager.Geoposition.Latitude,
                        LocationManager.Geoposition.Longitude, TimeZoneInfo.FindSystemTimeZoneById(LocationManager.Geoposition.TimeZone));

                    switch (TypeCombo.SelectedIndex)
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
                            moon = new Moon(new DateTime(Date.Year, Date.Month, j, 12, 0, 0), LocationManager.Geoposition.Latitude,
                                    LocationManager.Geoposition.Longitude, TimeZoneInfo.FindSystemTimeZoneById(LocationManager.Geoposition.TimeZone));  
                            Calend.Add(new CalendItem(j, moon.Dawn, moon.Dusk, !moon.Result.NoDawn, !moon.Result.NoDusk));
                            break;
                    }
                }

                //switch to CalendarGrid
                CalendarGrid.Visibility = Visibility.Visible;
                PhaseGrid.Visibility = Visibility.Collapsed;
            }
            
        }

        private void TypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Date = Date.AddMonths(-1);
            Update();
        }

        private void ForwardBtn_Click(object sender, RoutedEventArgs e)
        {
            Date = Date.AddMonths(1);
            Update();
        }

        private void CalendarGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CalendarGrid.SelectedItem!=null)
            {
                var item = CalendarGrid.SelectedItem as CalendItem;
                var param = new MainNavParam()
                {
                    Date = new DateTime(Date.Year, Date.Month, item.DayNum, 12, 0, 0),
                    IsSun = TypeCombo.SelectedIndex < 4
                };
                Frame thisFrame = Window.Current.Content as Frame;
                thisFrame.Navigate(typeof(MainPage), param);
            }
        }

        private void PhaseGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PhaseGrid.SelectedItem != null)
            {
                var item = PhaseGrid.SelectedItem as PhaseItem;
                var param = new MainNavParam()
                {
                    Date = new DateTime(Date.Year, Date.Month, item.DayNum, 12, 0, 0),
                    IsSun = false
                };
                Frame thisFrame = Window.Current.Content as Frame;
                thisFrame.Navigate(typeof(MainPage), param);
            }
        }

        private void CalendarGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            distX = e.Cumulative.Translation.X;
        }

        private void CalendarGrid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (Math.Abs(distX) <= 3) return;
            if (distX > 0)
                BackBtn_Click(sender, null);
            else if (distX < 0)
                ForwardBtn_Click(sender, null);
        }
    }
}
