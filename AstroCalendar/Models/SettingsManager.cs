using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SunMoon.Models
{
    class SettingsManager
    {
        static public ApplicationDataContainer local;
        static SettingsManager()
        {
            local = ApplicationData.Current.LocalSettings;
            var localfolder = ApplicationData.Current.LocalFolder;            
        }

        static public DateTime DateCheck
        {
            get
            {
                ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)local.Values["DateCheck"];
                if (composite == null)
                    return new DateTime(2017,1,1);
                else
                {
                    var date = new DateTime((int)composite["DateCheck_year"], (int)composite["DateCheck_month"], (int)composite["DateCheck_day"]);
                    return date;
                }
            }
            set
            {
                ApplicationDataCompositeValue composite = new ApplicationDataCompositeValue();
                composite["DateCheck_year"] = value.Year;
                composite["DateCheck_month"] = value.Month;
                composite["DateCheck_day"] = value.Day;
                local.Values["DateCheck"] = composite;
            }
        }

        static public Location Position
        {
            get
            {
                ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)local.Values["Position"];
                if (composite == null)
                    return new Location { Name = "None", Latitude = 23.92, Longitude = -42.79, TimeZone = TimeZoneInfo.Utc.Id };
                else
                {
                    var location = new Location();
                    location.Name = (string)composite["Name"] ?? "None";
                    location.TimeZone = (string)composite["TimeZoneId"] ?? TimeZoneInfo.Utc.Id;
                    location.Latitude = (double)(composite["Latitude"] ?? 23.92);
                    location.Longitude = (double)(composite["Longitude"] ?? -42.79);

                    return location;
                }
            }
            set
            {
                ApplicationDataCompositeValue composite =  new ApplicationDataCompositeValue();
                composite["Name"] = value.Name;
                composite["TimeZoneId"] = value.TimeZone;
                composite["Latitude"] = value.Latitude;
                composite["Longitude"] = value.Longitude;

                local.Values["Position"] = composite;
            }
        }

        static public bool IsSelectedLocation
        {
            get
            {
                return (bool)(local.Values["IsSelectedLocation"] ?? false);
            }
            set
            {
                local.Values["IsSelectedLocation"] = value;
            }
        }
    }
}
