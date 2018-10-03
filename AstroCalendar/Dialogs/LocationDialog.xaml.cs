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
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SunMoon.Dialogs
{
    public sealed partial class LocationDialog : ContentDialog
    {
        public Location Location { get; private set; }

        public LocationDialog(Location location=null)
        {
            this.InitializeComponent();

            Location = location;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            double lati, longi;
            args.Cancel = true;

            if (string.IsNullOrEmpty(NameTb.Text))
            {
                AlertTxt.Text = App.res.GetString("LocationNameWrong");
                FlyoutBase.ShowAttachedFlyout(NameTb);
            }
            else if (!double.TryParse(LatTb.Text, out lati))
            {
                AlertTxt.Text = App.res.GetString("LocationLatWrong");
                FlyoutBase.ShowAttachedFlyout(LatTb);
            }
            else if (!double.TryParse(LonTb.Text, out longi))
            {
                AlertTxt.Text = App.res.GetString("LocationLonWrong");
                FlyoutBase.ShowAttachedFlyout(LonTb);
            }
            else
            {
                Location = new Location();
                args.Cancel = false;
                Location.Latitude = lati;
                Location.Longitude = longi;
                Location.Name = NameTb.Text;
                var str = TimeZoneCombo.SelectedItem as string;
                Location.TimeZone = str.Substring(0, str.LastIndexOf(' '));
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }       

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            TimeZoneCombo.ItemsSource = TimeZoneInfo.GetSystemTimeZones().Select(p => $"{p.Id} (GMT{p.GetUtcOffset(DateTime.UtcNow).Hours})").ToList();
            if (Location != null)
            {
                NameTb.Text = Location.Name;
                LatTb.Text = Location.Latitude.ToString();
                LonTb.Text = Location.Longitude.ToString();
                TimeZoneCombo.SelectedItem = $"{Location.TimeZone} (GMT{TimeZoneInfo.FindSystemTimeZoneById(Location.TimeZone).GetUtcOffset(DateTime.UtcNow).Hours})";
            }
            else
                TimeZoneCombo.SelectedIndex = 0;
        }        
    }
}
