using AstroCalendar.Models;
using System;
using System.Collections.Generic;
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

namespace AstroCalendar.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SunDailyPage : Page
    {
        DateTime date;
        Sun sun;
        private double distX;

        public SunDailyPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            date = (DateTime)e.Parameter;
            sun = new Sun(date, LocationManager.Geoposition.Latitude, LocationManager.Geoposition.Longitude, TimeZoneInfo.FindSystemTimeZoneById(LocationManager.Geoposition.TimeZone));

            DateTxt.Text = date.ToString("dd.MM.yyyy");
            DawnTimeTxt.Text += !sun.Result.NoDawnDusk ? sun.Dawn.ToString("HH:mm") : "---";
            DuskTimeTxt.Text += !sun.Result.NoDawnDusk ? sun.Dusk.ToString("HH:mm") : "---";
            NoonTimeTxt.Text += !sun.Result.NoDawnDusk ? sun.Noon.ToString("HH:mm") : "---";
            if (sun.Dawn<sun.Dusk)
                LengthTimeTxt.Text += !sun.Result.NoDawnDusk ? (sun.Dusk - sun.Dawn).ToString(@"hh\:mm") : "---";
            else
                LengthTimeTxt.Text += !sun.Result.NoDawnDusk ? (new TimeSpan(24, 0, 0) - (sun.Dawn - sun.Dusk)).ToString(@"hh\:mm") : "---";

            CivilDawnTxt.Text = !sun.Result.NoCivil ? sun.CivilDawn.ToString("HH:mm") : "---";
            CivilDuskTxt.Text = !sun.Result.NoCivil ? sun.CivilDusk.ToString("HH:mm") : "---";
            NauticalDawnTxt.Text = !sun.Result.NoNautical ? sun.NauticalDawn.ToString("HH:mm") : "---";
            NauticalDuskTxt.Text = !sun.Result.NoNautical ? sun.NauticalDusk.ToString("HH:mm") : "---";
            AstroDawnTxt.Text = !sun.Result.NoAstronomical ? sun.AstronomicalDawn.ToString("HH:mm") : "---";
            AstroDuskTxt.Text = !sun.Result.NoAstronomical ? sun.AstronomicalDusk.ToString("HH:mm") : "---";

            base.OnNavigatedTo(e);
        }

        private void TodayBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame thisFrame = Window.Current.Content as Frame;
            thisFrame.Navigate(typeof(MainPage), string.Empty);
        }

        private void ForwardBtn_Click(object sender, RoutedEventArgs e)
        {
            var param = new MainNavParam()
            {
                Date = date.AddDays(1),
                IsSun = true
            };
            Frame thisFrame = Window.Current.Content as Frame;
            thisFrame.Navigate(typeof(MainPage), param);
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            var param = new MainNavParam()
            {
                Date = date.AddDays(-1),
                IsSun = true
            };
            Frame thisFrame = Window.Current.Content as Frame;
            thisFrame.Navigate(typeof(MainPage), param);
        }

        private void Page_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            distX = e.Cumulative.Translation.X;
        }

        private void Page_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (Math.Abs(distX) <= 3) return;
            if (distX > 0)
                BackBtn_Click(sender, null);
            else if (distX < 0)
                ForwardBtn_Click(sender, null);
        }
    }
}
