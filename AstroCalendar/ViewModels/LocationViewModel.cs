using GalaSoft.MvvmLight.Command;
using SunMoon.Dialogs;
using SunMoon.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace AstroCalendar.ViewModels
{
    public class LocationViewModel : BaseViewModel
    {
        LocationContext _db;
        Location  _selectedlocaiton;
        ObservableCollection<Location> _locations, _autolocations;

        public LocationContext DB
        {
            get => _db;
            set
            {
                _db = value;
                OnPropertyChanged(nameof(_db));
            }
        }

        public ObservableCollection<Location> AutoLocations
        {
            get => _autolocations;
            set
            {
                _autolocations = value;
                OnPropertyChanged(nameof(_autolocations));
            }
        }

        public Location SelectedLocation
        {
            get => _selectedlocaiton;
            set
            {
                _selectedlocaiton = value;
                OnPropertyChanged(nameof(_selectedlocaiton));
            }
        }

        public ObservableCollection<Location> Locations
        {
            get => _locations;
            set
            {
                _locations = value;
                OnPropertyChanged(nameof(_locations));
            }
        }

        public event EventHandler RequestClose;

        public ICommand ExitCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand EditCommand { get; set; }

        public LocationViewModel()
        {
            DB = new LocationContext();

            AddCommand = new RelayCommand(Add);
            EditCommand = new RelayCommand(Edit);
            DeleteCommand = new RelayCommand(Delete);
            UpdateCommand = new RelayCommand(Update);
            ExitCommand = new RelayCommand(Exit);

            Locations = new ObservableCollection<Location>(DB.Locations);
            SelectedLocation = Locations.FirstOrDefault();
            AutoLocations = new ObservableCollection<Location>();
        }

        void Refresh()
        {
            Locations.Clear();
            var collection = DB.Locations;
            foreach (var item in collection)
                Locations.Add(item);
            SelectedLocation = Locations.FirstOrDefault();
        }

        void OnRequestClose()
        {
            RequestClose?.Invoke(this, new EventArgs());
        }

        async void Add()
        {
            var dlg = new LocationDialog();
            var result = await dlg.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                    DB.Locations.Add(dlg.Location);
                    DB.SaveChanges();
                    Refresh();
            }
        }

        async void Edit()
        {
            var dlg = new LocationDialog(SelectedLocation);
            var result = await dlg.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                SelectedLocation.Latitude = dlg.Location.Latitude;
                SelectedLocation.Longitude = dlg.Location.Longitude;
                SelectedLocation.Name = dlg.Location.Name;
                SelectedLocation.TimeZone = dlg.Location.TimeZone;

                DB.SaveChanges();
                Refresh();
            }
        }

        async void Update()
        {
            SettingsManager.IsSelectedLocation = false;
            if (await LocationManager.GetCoordinates())
            {
                AutoLocations.Clear();
                AutoLocations.Add(LocationManager.Geoposition);
            }
            SettingsManager.IsSelectedLocation = true;
        }

        void Delete()
        {
            DB.Locations.Remove(SelectedLocation);
            DB.SaveChanges();
            Refresh();
        }

        void Exit()
        {            
            if (SelectedLocation != null)
                LocationManager.Geoposition = SelectedLocation;
            else if (AutoLocations!=null && AutoLocations.Count>0)
                LocationManager.Geoposition = AutoLocations.FirstOrDefault();

            SettingsManager.IsSelectedLocation = true;
            OnRequestClose();
        }
    }
}
