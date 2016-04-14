using RecycleFinland.Engine;
using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Microsoft.Azure.Engagement;
using Microsoft.Azure.Engagement.Overlay;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RecycleFinland
{
    public delegate bool CloseView();

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : EngagementPageOverlay
    {
        //engagement data
        private string version = "1,00";
        private string language= "English";

        public MainPage()
        {
            this.InitializeComponent();

            searchControl.StartSearchWithLocation += StartSearchWithLocation;
            searchControl.GetMapCenter += GetMapCenter;
            searchControl.ShowAboutView += ShowAboutView;

            myMap.StartSearchWithLocation += StartSearchWithLocation;
            myMap.ShowDetails += ShowDetails;

            typesView.CloseView += canGobackAndExit;
            typesView.MaterialTypeSelected += MaterialTypeSelected;

            aboutView.CloseView += canGobackAndExit;

            detailsView.CloseView += canGobackAndExit;
            detailsView.CenterMapForPoisDetails += CenterMapForPoisDetails;

      //      Dictionary<object, object> dic = new Dictionary<object, object>();
      //      dic.Add("version", version);
            EngagementAgent.Instance.StartActivity("MainActivity");
        }

        protected override string GetEngagementPageName()
        {
            System.Diagnostics.Debug.WriteLine("Engagement GetEngagementPageName called");
            return "MainActivity";
        }
        protected override Dictionary<object, object> GetEngagementPageExtra()
        {
            System.Diagnostics.Debug.WriteLine("GetEngagementPageExtra");

            Dictionary<object, object> dic = new Dictionary<object, object>();
            dic.Add("version", version);
            dic.Add("language", language);
            return dic;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
   
            (Application.Current as App).canGobackAndExit += canGobackAndExit;
            JLYRestService.Instance.JLYResults += JLYResults;

            //load stored values
            myMap.Center = Settings.MapCenter;
            myMap.ZoomLevel = Settings.MapZoomLevel;
            typesView.SelectedType = Settings.SelectedType;

            myMap.InitControl();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
   
            (Application.Current as App).canGobackAndExit -= canGobackAndExit;
            JLYRestService.Instance.JLYResults -= JLYResults;

            //Store map location
            Settings.MapCenter = myMap.Center;
            Settings.MapZoomLevel = myMap.ZoomLevel;
            Settings.SelectedType = typesView.SelectedType;

            myMap.DeInitControl();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == reFreshListButton)
            {
                EngagementAgent.Instance.SendSessionEvent("Refresh_data");
                StartSearchWithLocation(myMap.Center, false);
            }
            else if(sender == openTypesListButton)
            {
                EngagementAgent.Instance.StartActivity("TypesSelectionView");

                typesView.Show();
                typesView.Visibility = Visibility.Visible;
            }
        }

        private Geopoint GetMapCenter()
        {
            return myMap.Center;
        }
        private void ShowAboutView()
        {
            EngagementAgent.Instance.StartActivity("AboutView");

            aboutView.Visibility = Visibility.Visible;
        }

        private void MaterialTypeSelected(int type)
        {
            StartSearchWithLocation(myMap.Center, false);
        }

        private void ShowDetails(List<JLYServiceItem> array, int selectedIndex)
        {
            EngagementAgent.Instance.StartActivity("DetailsView");

            detailsView.Show(array, selectedIndex);
            detailsView.Visibility = Visibility.Visible;
        }

        private void CenterMapForPoisDetails(Geopoint point)
        {
            myMap.Center = point;
        }

        public bool canGobackAndExit()
        {
            if (typesView.Visibility == Visibility.Visible)
            {
                typesView.Visibility = Visibility.Collapsed;
                EngagementAgent.Instance.StartActivity("MainActivity");
                return false;
            }

            if (detailsView.Visibility == Visibility.Visible)
            {
                detailsView.Visibility = Visibility.Collapsed;
                EngagementAgent.Instance.StartActivity("MainActivity");
                return false;
            }

            if (aboutView.Visibility == Visibility.Visible)
            {
                aboutView.Visibility = Visibility.Collapsed;
                EngagementAgent.Instance.StartActivity("MainActivity");
                return false;
            }

            //Store map location
            Settings.MapCenter = myMap.Center;
            Settings.MapZoomLevel = myMap.ZoomLevel;
            Settings.SelectedType = typesView.SelectedType;
            //nothing to handle here, so back can do default stuff
            return true;
        }

        private void StartSearchWithLocation(Geopoint point, bool addGeoMarker)
        {
          if (addGeoMarker)
            {
                myMap.setGeoCodedMarker(point);
                myMap.Center = point;
            }

            var extras = new Dictionary<object, object>();
            extras.Add("latitude", point.Position.Latitude);
            extras.Add("longitude", point.Position.Longitude);
            extras.Add("type", typesView.SelectedType);

            EngagementAgent.Instance.StartJob("FindNearby", extras);

            progressBar.Visibility = Visibility.Visible;
            JLYRestService.Instance.FindNearby(point.Position.Latitude, point.Position.Longitude, typesView.SelectedType, 15, 0);
        }

        private async void JLYResults(List<JLYServiceItem> items, Exception error)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                EngagementAgent.Instance.EndJob("FindNearby");

                progressBar.Visibility = Visibility.Collapsed;

                if (error != null)
                {
                    ShowErrorDialog(error.Message, "Error fetching data");
                }
                else if (items != null)
                {
                    System.Diagnostics.Debug.WriteLine("Done without errors, got items : " + items.Count);
                    myMap.ItemsSource = items;
                    //myMap.TryZoomToItems();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Done without anykind of results");
                }
            });
        }
        private async void ShowErrorDialog(string message, string title)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                var dialog = new MessageDialog(message, title);
                await dialog.ShowAsync();
            });
        }
    }
}
