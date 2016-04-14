using Microsoft.Azure.Engagement;
using RecycleFinland.Engine;
using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RecycleFinland.Controls
{
    public delegate void StartSearchWithLocation(Geopoint point, bool addGeoMarker);
    public delegate void ShowDetails(List<JLYServiceItem> array, int selectedIndex);

    public sealed partial class MyMapControl : UserControl
    {
        public ShowDetails ShowDetails;
        public StartSearchWithLocation StartSearchWithLocation;
        
        //maps data
        private const String _mapServiceToken = "ADD_YOUR_TOKEN_IN_HERE";

        private RandomAccessStreamReference _mapIconMyLocation;
        private RandomAccessStreamReference _mapIconGeoLocation;

        private Geolocator _geolocator;
        private Geoposition _geoPosition;
        private MapIcon _myLocationMarker;
        private MapIcon _geoCodedLocationMarker;

        public Geopoint Center
        {
            get
            {
                return myMap.Center;
            }
            set
            {
                myMap.Center = value;
            }
        }

        public Double ZoomLevel
        {
            get
            {
                return myMap.ZoomLevel;
            }
            set
            {
                myMap.ZoomLevel = value;
            }
        }

        public System.Object ItemsSource
        {
            get
            {
                return MapItems.ItemsSource;
            }
            set
            {
                MapItems.ItemsSource = value;
            }
        }

        public MyMapControl()
        {
            this.InitializeComponent();

            _mapIconMyLocation = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/myLocationImage.png"));
            _mapIconGeoLocation = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/geoCodedLocation.png"));

        }
        public async void InitControl()
        {
            myMap.MapServiceToken = _mapServiceToken;
            myMap.Loaded += MyMap_Loaded;

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
                        geoLocationDot.Visibility = Visibility.Collapsed;
                        break;
                    case GeolocationAccessStatus.Unspecified:
                        geoLocationDot.Visibility = Visibility.Collapsed;
                        System.Diagnostics.Debug.WriteLine("Location requests Unspecified");
                        break;
                }
            }

            if (_geolocator != null)
            {
                _geolocator.PositionChanged += _geolocator_PositionChanged;
            }
        }

        public void DeInitControl()
        {
            myMap.Loaded -= MyMap_Loaded;
            if (_geolocator != null)
            {
                _geolocator.PositionChanged -= _geolocator_PositionChanged;
            }
        }

        public void setGeoCodedMarker(Geopoint point)
        {
            if (_geoCodedLocationMarker == null)
            {
                _geoCodedLocationMarker = new MapIcon();
                _geoCodedLocationMarker.Location = point;
                _geoCodedLocationMarker.NormalizedAnchorPoint = new Point(0.5, 1.0);
                _geoCodedLocationMarker.Image = _mapIconGeoLocation;
                myMap.MapElements.Add(_geoCodedLocationMarker);
            }

            _geoCodedLocationMarker.Location = point;
        }
        /*
        public async void TryZoomToItems()
        {
            List<JLYServiceItem> array = (List<JLYServiceItem>)MapItems.ItemsSource;
            if (array == null || array.Count <= 0)
            {
                return;
            }
            List<Geopoint> points = new List<Geopoint>();
            foreach (JLYServiceItem item in array)
            {
                if (item != null && item.Location != null)
                {
                    points.Add(item.Location);
                }
            }

            if (points.Count > 0)
            {
                if (points.Count > 1)
                {
                    //its just so bad, that its better withuout
                    //await myMap.TrySetSceneAsync(MapScene.CreateFromLocations(points), MapAnimationKind.Linear);
                    //the zoom appears to zoom too much out, so lets have a workaround here
                    // todo if this gets fixed, plese remove workaround
                    //myMap.ZoomLevel = (myMap.ZoomLevel - 2);
                }
                else
                {
                    myMap.Center = points[0];
                }
            }
        }*/

        private void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
            StartSearchWithLocation?.Invoke(myMap.Center,false);
        }

        private void geoLocationDot_Click(object sender, RoutedEventArgs e)
        {
            if (_geoPosition != null)
            {
                EngagementAgent.Instance.SendSessionEvent("locate_me");
                //move to the GPS location
                myMap.Center = _geoPosition.Coordinate.Point;
                StartSearchWithLocation?.Invoke(myMap.Center,false);
            }
        }

        private async void _geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (_myLocationMarker == null)
                {
                    _myLocationMarker = new MapIcon();
                    _myLocationMarker.Location = args.Position.Coordinate.Point;
                    _myLocationMarker.NormalizedAnchorPoint = new Point(0.5, 1.0);
                    _myLocationMarker.Image = _mapIconMyLocation;
                    myMap.MapElements.Add(_myLocationMarker);
                }

                _geoPosition = args.Position;
                _myLocationMarker.Location = args.Position.Coordinate.Point;
                geoLocationDot.Visibility = Visibility.Visible;
            });
        }

        private void mapItemButton_Click(object sender, RoutedEventArgs e)
        {
            var buttonSender = sender as Button;
            JLYServiceItem poi = buttonSender.DataContext as JLYServiceItem;

            List<JLYServiceItem> array = (List<JLYServiceItem>)ItemsSource;
            if (array == null || array.Count <= 0)
            {
                return;
            }

            ShowDetails?.Invoke(array, array.IndexOf(poi));
        }
    }
}
