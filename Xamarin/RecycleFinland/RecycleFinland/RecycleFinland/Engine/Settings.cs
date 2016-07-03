using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using Xamarin.Forms.Maps;

namespace RecycleFinland.Engine
{
    class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        private static Double _mapZoomLevelDefault = 15000;
        private static Double _mapLatitudeDefault = 60.1708;
        private static Double _mapLongitudeDefault = 24.9375;

        public static Position MapCenter
        {
            get
            {
                Double lat = AppSettings.GetValueOrDefault<Double>("mapCenterLatitude", _mapLatitudeDefault);
                Double lon = AppSettings.GetValueOrDefault<Double>("mapCenterLongitude", _mapLongitudeDefault);
                return new Position(lat, lon);
            }
            set
            {
                Position mapPoint = (Position)value;
                if (mapPoint != null)
                {
                    AppSettings.AddOrUpdateValue<Double>("mapCenterLatitude", mapPoint.Latitude);
                    AppSettings.AddOrUpdateValue<Double>("mapCenterLongitude", mapPoint.Longitude);
                }
            }
        }

        public static Double MapDistance
        {
            get
            {
                return AppSettings.GetValueOrDefault<Double>("mapZoomLevel", _mapZoomLevelDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<Double>("mapZoomLevel", value);
            }
        }

        public static int SelectedType
        {
            get
            {
                return AppSettings.GetValueOrDefault<int>("selectedType", 0);
            }
            set
            {
                AppSettings.AddOrUpdateValue<int>("selectedType", value);
            }
        }
    }
}

