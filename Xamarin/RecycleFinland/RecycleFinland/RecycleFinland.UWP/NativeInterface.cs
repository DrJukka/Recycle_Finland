using RecycleFinland.UWP;
using System;
using Windows.Devices.Geolocation;
using Windows.System;

[assembly: Xamarin.Forms.Dependency(typeof(NativeInterface))]

namespace RecycleFinland.UWP
{
    public class NativeInterface : INativeInterface
    {
        private UserPositionChanged UserPositionChanged;
        private Geolocator _geolocator;

        public async void NavigateToPosition(double latitude, double longitude)
        {
            string latStr = latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string lonStr = longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);

            string url = "ms-drive-to:?destination.latitude=" + latStr + "&destination.longitude=" + lonStr;
            await Launcher.LaunchUriAsync(new Uri(url));
        }

        public void RemovePositionChangedDelegate(UserPositionChanged callback)
        {
            if (_geolocator != null)
            {
                _geolocator.PositionChanged -= _geolocator_PositionChanged;
            }

            UserPositionChanged = null;
        }

        public async void SetPositionChangedDelegate(UserPositionChanged callback)
        {
            UserPositionChanged = callback;
            if (_geolocator == null)
            {
                var accessStatus = await Geolocator.RequestAccessAsync();

                switch (accessStatus)
                {
                    case GeolocationAccessStatus.Allowed:
                        System.Diagnostics.Debug.WriteLine("Location requests Allowed");
                        _geolocator = new Geolocator { ReportInterval = 2000 };
                        break;
                    case GeolocationAccessStatus.Denied:
                        System.Diagnostics.Debug.WriteLine("Location requests Denied");
                        break;
                    case GeolocationAccessStatus.Unspecified:
                        System.Diagnostics.Debug.WriteLine("Location requests Unspecified");
                        break;
                }
                if (_geolocator != null)
                {
                    _geolocator.PositionChanged += _geolocator_PositionChanged;
                }
            }
        }

        private void _geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            UserPositionChanged?.Invoke(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);
        }
    }
}
