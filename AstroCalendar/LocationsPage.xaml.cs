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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SunMoon
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocationsPage : Page
    {
        Location autolocation;
        public LocationsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            autolocation = LocationManager.Geoposition;
            AutoListBox.Items.Clear();
            AutoListBox.Items.Add(autolocation);
            using (var db = new LocationContext())
            {
                LocationsListBox.ItemsSource = db.Locations.ToList();
            }
            AutoListBox.SelectedIndex = 0;
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LocationsListBox.SelectedItem != null)
            {
                Location location = LocationsListBox.SelectedItem as Location;
                using (var db = new LocationContext())
                {
                    db.Locations.Remove(location);
                    db.SaveChanges();
                    LocationsListBox.ItemsSource = db.Locations.ToList();
                }
            }
        }

        private async void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new LocationDialog();
            var result = await dlg.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                using (var db = new LocationContext())
                {
                    db.Locations.Add(dlg.Location);
                    db.SaveChanges();
                    LocationsListBox.ItemsSource = db.Locations.ToList();
                }
            }
        }

        private async void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LocationsListBox.SelectedItem != null)
            {
                var location = LocationsListBox.SelectedItem as Location;
                var dlg = new LocationDialog(location);
                var result = await dlg.ShowAsync();                
                if (result == ContentDialogResult.Primary)
                {
                    location.Latitude = dlg.Location.Latitude;
                    location.Longitude = dlg.Location.Longitude;
                    location.Name = dlg.Location.Name;
                    location.TimeZone = dlg.Location.TimeZone;
                    
                    using (var db = new LocationContext())
                    {
                        db.Locations.Update(location);
                        db.SaveChanges();
                        LocationsListBox.ItemsSource = db.Locations.ToList();
                    }
                }
            }

        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AutoListBox.SelectedItem as Location != null)
            {
                LocationManager.Geoposition = autolocation;
                SettingsManager.IsSelectedLocation = false;
            }
            else if (LocationsListBox.SelectedItem as Location != null)
            {
                LocationManager.Geoposition = LocationsListBox.SelectedItem as Location;
                SettingsManager.IsSelectedLocation = true;
            }

            Frame thisFrame = Window.Current.Content as Frame;
            thisFrame.Navigate(typeof(MainPage),"");
        }
        

        private async void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateBtn.IsEnabled = false;

            if (await LocationManager.GetCoordinates())
            {
                UpdateBtn.Icon = new SymbolIcon(Symbol.Accept);
                autolocation = LocationManager.Geoposition;
                AutoListBox.Items.Clear();
                AutoListBox.Items.Add(autolocation);
            }
            else
            {
                UpdateBtn.Icon = new SymbolIcon(Symbol.Clear);
                UpdateBtn.IsEnabled = true;
            }
        }

        private void AutoListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LocationsListBox.Items.Count > 0 && AutoListBox.SelectedIndex!=-1)
                LocationsListBox.SelectedIndex = -1;
        }

        private void LocationsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LocationsListBox.Items.Count > 0 && LocationsListBox.SelectedIndex != -1)
            {
                AutoListBox.SelectedIndex = -1;
                EditBtn.IsEnabled = true;
                DelBtn.IsEnabled = true;
            }
            else
            {
                EditBtn.IsEnabled = false;
                DelBtn.IsEnabled = false;
            }
        }
    }
}
