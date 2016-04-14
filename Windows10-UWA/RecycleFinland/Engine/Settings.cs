using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace RecycleFinland.Engine
{
    class Settings
    {
        private static Double _mapZoomLevelDefault = 12;
        private static Double _mapLatitudeDefault  = 60.1708;
        private static Double _mapLongitudeDefault = 24.9375;

        public static Geopoint MapCenter
        {
            get
            {
                Object tmpVal1 = Windows.Storage.ApplicationData.Current.LocalSettings.Values["mapCenterLatitude"];
                Object tmpVal2 = Windows.Storage.ApplicationData.Current.LocalSettings.Values["mapCenterLongitude"];
                if (tmpVal1 == null || tmpVal2 == null)
                {
                    return new Geopoint(new BasicGeoposition()
                    {
                        //Geopoint for Helsinki 
                        Latitude = _mapLatitudeDefault,
                        Longitude = _mapLongitudeDefault
                    });
                }

                return new Geopoint(new BasicGeoposition()
                {
                    //Geopoint for Helsinki 
                    Latitude =(Double)tmpVal1,
                    Longitude = (Double)tmpVal2
                });
            }
            set
            {
                if (value is Geopoint)
                {
                    Geopoint mapPoint = (Geopoint)value;
                    if (mapPoint != null)
                    {
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values["mapCenterLatitude"] = mapPoint.Position.Latitude;
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values["mapCenterLongitude"] = mapPoint.Position.Longitude;
                    }
                }
            }
        }

        public static Double MapZoomLevel
        {
            get
            {
                Object tmpVal = Windows.Storage.ApplicationData.Current.LocalSettings.Values["mapZoomLevel"];
                if (tmpVal == null)
                {
                    return _mapZoomLevelDefault;
                }

                return (Double)tmpVal;
            }
            set
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["mapZoomLevel"] = value;
            }
        }

        public static int SelectedType
        {
            get
            {
                Object tmpVal = Windows.Storage.ApplicationData.Current.LocalSettings.Values["selectedType"];
                if (tmpVal == null)
                {
                    return 0;
                }

                return (int)tmpVal;
            }
            set
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["selectedType"] = value;
            }
        }
    }
}
