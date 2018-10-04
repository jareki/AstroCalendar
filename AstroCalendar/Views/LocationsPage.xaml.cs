using AstroCalendar.ViewModels;
using AstroCalendar.Dialogs;
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
    public sealed partial class LocationsPage : Page
    {
        public LocationsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var vm = new LocationViewModel();
            DataContext = vm;
            vm.RequestClose += Vm_RequestClose;
        }

        private void Vm_RequestClose(object sender, EventArgs e)
        {
            Frame thisFrame = Window.Current.Content as Frame;
            thisFrame.Navigate(typeof(MainPage), "");
        }
        

        private async void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateBtn.IsEnabled = false;

            if (await LocationManager.GetCoordinates())
            {
                UpdateBtn.Icon = new SymbolIcon(Symbol.Accept);
            }
            else
            {
                UpdateBtn.Icon = new SymbolIcon(Symbol.Clear);
                UpdateBtn.IsEnabled = true;
            }
        }

        private void AutoListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LocationsListBox.Items.Count > 0 && AutoListBox.SelectedIndex != -1)
            {
                LocationsListBox.SelectedIndex = -1;
                (DataContext as LocationViewModel).SelectedLocation = null;
            }
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
