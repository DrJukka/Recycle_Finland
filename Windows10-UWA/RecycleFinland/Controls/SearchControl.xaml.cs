using Microsoft.Azure.Engagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RecycleFinland.Controls
{
    public delegate Geopoint GetMapCenter();
    public delegate void ShowAboutView();
    public sealed partial class SearchControl : UserControl
    {
        public GetMapCenter GetMapCenter;
        public ShowAboutView ShowAboutView;
        public StartSearchWithLocation StartSearchWithLocation;
        public SearchControl()
        {
            this.InitializeComponent();
        }

        private void searchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                searchBoxButton_Click(null, null);
            }
        }

        private async void searchBoxButton_Click(object sender, RoutedEventArgs e)
        {

            if (searchBox.Text.Length > 0 && (GetMapCenter != null))
            {
                var extras = new Dictionary<object, object>();
                extras.Add("SearchText", searchBox.Text);
                EngagementAgent.Instance.StartJob("SeachAddress", extras);

                MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(searchBox.Text, GetMapCenter());

                System.Diagnostics.Debug.WriteLine("found " + result.Locations.Count + " results.");

                EngagementAgent.Instance.EndJob("SeachAddress");
                if (result.Locations.Count > 0)
                {
                    Geopoint resultLocation = result.Locations[0].Point;
                    StartSearchWithLocation(resultLocation, true);
                }
            }
        }
        private void aboutButton_Click(object sender, RoutedEventArgs e)
        {
            ShowAboutView?.Invoke();
        }
    }
}
