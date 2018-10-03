using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace SunMoon.Models
{
    static class LocationManager
    {
        static Location _geopostion;
        public static Location Geoposition
        {
            get
            {
                return _geopostion;
            }
            set
            {
                _geopostion = value;
                SettingsManager.Position = value;
                SettingsManager.DateCheck = DateTime.Now;
            }
        }

        static LocationManager()
        {
            _geopostion = new Location();
        }

        async static Task<Geoposition> GetDevicePosition()
        {
            try
            {
                var access_status = await Geolocator.RequestAccessAsync();
                if (access_status != GeolocationAccessStatus.Allowed)
                    return null;
                            
                var geolocator = new Geolocator { DesiredAccuracyInMeters = 0 };
                return await geolocator.GetGeopositionAsync();
            }
            catch
            {
                return null;
            }
        }

        //return true if successeded
        public async static Task<bool> GetCoordinates()
        {
            try
            {
                _geopostion = SettingsManager.Position;
                if (SettingsManager.IsSelectedLocation) //using saved location
                    return true;
            }
            catch { }

            //when  using autodetecting
            try
            {
                var device_pos = await GetDevicePosition();
                if (device_pos == null)
                    return false;

                var device_coor = device_pos.Coordinate.Point.Position;


                if (Math.Abs(device_coor.Latitude - SettingsManager.Position.Latitude) + Math.Abs(device_coor.Longitude - SettingsManager.Position.Longitude) >= 1 || DateTime.Now - SettingsManager.DateCheck >= new TimeSpan(1, 0, 0, 0))
                {
                    var georesult = await MapLocationFinder.FindLocationsAtAsync(new Geopoint(device_coor));
                    if (georesult.Status == MapLocationFinderStatus.Success)
                    {
                        Geoposition.Name = $"{georesult.Locations[0].Address.Country} {georesult.Locations[0].Address.RegionCode} {georesult.Locations[0].Address.Town}";
                        Geoposition.TimeZone = TimeZoneInfo.Local.Id;
                        Geoposition.Latitude = device_coor.Latitude;
                        Geoposition.Longitude = device_coor.Longitude;
                        Geoposition.Id = -1;
                        SettingsManager.Position = Geoposition;
                    }
                    else
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
