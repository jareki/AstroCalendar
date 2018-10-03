using SunMoon.Models;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SunMoon.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MoonDailyPage : Page
    {
        DateTime date;
        Moon moon;
        Sun sun;
        private double distX;

        public MoonDailyPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            date = (DateTime)e.Parameter;
            moon = new Moon(date, LocationManager.Geoposition.Latitude, LocationManager.Geoposition.Longitude, TimeZoneInfo.FindSystemTimeZoneById(LocationManager.Geoposition.TimeZone));
            sun = new Sun(date, LocationManager.Geoposition.Latitude, LocationManager.Geoposition.Longitude, TimeZoneInfo.FindSystemTimeZoneById(LocationManager.Geoposition.TimeZone));

            DateTxt.Text = date.ToString("dd.MM.yyyy");
            DawnTimeTxt.Text += !moon.Result.NoDawn ? moon.Dawn.ToString("HH:mm") : "---";
            DuskTimeTxt.Text += !moon.Result.NoDusk ? moon.Dusk.ToString("HH:mm") : "---";

            double illumination = Astro.GetMoonPhase(moon, sun);
            int percent = (int)(illumination * 100);
            PhaseTxt.Text += $"{Math.Abs(percent)} %, ";

            var result = Astro.GetMoonPhase(illumination);
            PhaseTxt.Text += result.Item1;
            MoonImg.Source = new BitmapImage(new Uri(result.Item2));

            var moondates = Astro.GetFullNewMoonDate(date, LocationManager.Geoposition.Latitude, LocationManager.Geoposition.Longitude, TimeZoneInfo.FindSystemTimeZoneById(LocationManager.Geoposition.TimeZone));
            FullMoonDateTxt.Text = moondates.Item1.ToString("dd.MM.yy");
            NewMoonDateTxt.Text = moondates.Item2.ToString("dd.MM.yy");
            
            base.OnNavigatedTo(e);
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            var param = new MainNavParam()
            {
                Date = date.AddDays(-1),
                IsSun = false
            };
            Frame thisFrame = Window.Current.Content as Frame;
            thisFrame.Navigate(typeof(MainPage), param);
        }

        private void ForwardBtn_Click(object sender, RoutedEventArgs e)
        {
            var param = new MainNavParam()
            {
                Date = date.AddDays(1),
                IsSun = false
            };
            Frame thisFrame = Window.Current.Content as Frame;
            thisFrame.Navigate(typeof(MainPage), param);
        }

        private void TodayBtn_Click(object sender, RoutedEventArgs e)
        {
            var param = new MainNavParam()
            {
                Date = DateTime.Now,
                IsSun = false
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
