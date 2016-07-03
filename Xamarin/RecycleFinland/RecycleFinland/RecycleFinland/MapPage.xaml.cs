using RecycleFinland.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Maps;


namespace RecycleFinland
{
    public delegate void GoBackToMapView();
    public partial class MapPage : ContentPage
    {
        private int selectedType = 0;
        private Geocoder geoCoder;
        private Position userPosition;
        private int selectedPlaceIndex = 0;
       
        public MapPage()
        {
            InitializeComponent();

            geoCoder = new Geocoder();
            typesSelectionList.SelectedItem = 0;
            typesSelectionList.ItemSelected += TypesSelectionList_ItemSelected;

            JLYRestService.Instance.JLYResults += JLYResults;
            customMap.PlaceSelected += PlaceSelected;

            aboutGrid.GoBackToMapView += GoBackToMapView;

            AppResume();
        }
        public void AppResume()
        {
            selectedType = Settings.SelectedType;

            Position point = Settings.MapCenter;
            Distance dist = new Distance(Settings.MapDistance);
            customMap.MoveToRegion(MapSpan.FromCenterAndRadius(point, dist));

            StartSearchWithLocation(Settings.MapCenter);

            DependencyService.Get<INativeInterface>().SetPositionChangedDelegate(UserPositionChanged);
        }

        public void AppToSleep()
        {
            JLYRestService.Instance.Cancel();
            DependencyService.Get<INativeInterface>().RemovePositionChangedDelegate(UserPositionChanged);

            Settings.SelectedType= selectedType;
            Settings.MapCenter   = GetMapVisibleCenter();
            Settings.MapDistance = GetMapVisibleDistance().Meters;
        }
        
        public void UserPositionChanged(double latitude, double longitude)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                userPosition = new Position(latitude, longitude);
                userLocationButton.IsVisible = true;
            });
        }

        private void userLocationButton_Clicked(object sender, EventArgs e)
        {
            if(userPosition == null)
            {
                return;
            }

            customMap.MoveToRegion(MapSpan.FromCenterAndRadius(userPosition, GetMapVisibleDistance()));
            StartSearchWithLocation(userPosition);
        }

        public void PlaceSelected(int index)
        {
            if (customMap.CustomPins.Count <= index)
            {
                return;
            }

            selectedPlaceIndex = index;
            detailsGrid.IsVisible = true;
            ShowSelectedPlaceData();
        }
        private void NavigatetoPlace_Click(object sender, EventArgs e)
        {
            JLYServiceItem item = GetServiceItem(selectedPlaceIndex);
            if (item == null)
            {
                return;
            }

            DependencyService.Get<INativeInterface>().NavigateToPosition(item.Location.Latitude, item.Location.Longitude);
        }

        Boolean changedAlready = false;
        void GridOnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            Debug.WriteLine("Grid width : " + detailsGrid.Width + ", e.TotalX : " + e.TotalX);

            if (e.TotalX == 0) {
                changedAlready = false;
            }

            if (changedAlready)
            {
                return;
            }

            if (e.TotalX < -(detailsGrid.Width / 2))
            {
                selectedPlaceIndex--;
                if(selectedPlaceIndex < 0) {
                    selectedPlaceIndex = customMap.CustomPins.Count - 1;
                }
            }
            else if (e.TotalX > (detailsGrid.Width / 2))
            {
                selectedPlaceIndex++;
                if (selectedPlaceIndex >= customMap.CustomPins.Count)
                {
                    selectedPlaceIndex = 0;
                }
            }else
            {
                return;
            }

            changedAlready = true;
            ShowSelectedPlaceData();
        }

        private void ShowSelectedPlaceData()
        {
            JLYServiceItem poi = GetServiceItem(selectedPlaceIndex);
            if (poi == null)
            {
                return;
            }

            detailsName.Text = poi.DisplayName;
            detailsContact.Text = poi.Contact;
            detailsAddress.Text = poi.Address;
            detailsPCodeCity.Text = poi.PostalCode + "" + poi.City;
            detailsOpenTimes.Text = poi.OpenTimes;

            List<MaterialTypeModel> types = new List<MaterialTypeModel>();
            foreach (int typeKey in poi.MatrialTypes)
            {
                MaterialTypeModel item = MaterialTypeModel.FromCode(typeKey);
                if (item != null)
                {
                    types.Add(item);
                }
            }

            detailTypes.ItemsSource = types;

            if(customMap.VisibleRegion == null)
            {
                return;
            }

            customMap.MoveToRegion(MapSpan.FromCenterAndRadius(poi.Location, GetMapVisibleDistance()));
        }

        private void TypesSelectionList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            MaterialTypeModel selected = (MaterialTypeModel)e.SelectedItem;
            if (selected != null)
            {
                selectedType = selected.Code;
                StartSearchWithLocation(GetMapVisibleCenter());
            }
        }

        private void GoBackToMapView()
        {
            typesGrid.IsVisible = false;
            aboutGrid.IsVisible = false;
            detailsGrid.IsVisible = false;
        }
        private void Back_Clicked(object sender, EventArgs e)
        {
            GoBackToMapView();
        }

        private void ReFreshListButton_Clicked(object sender, EventArgs e)
        {
            StartSearchWithLocation(GetMapVisibleCenter());
        }

        private void OpenTypesListButton_Clicked(object sender, EventArgs e)
        {
            typesGrid.IsVisible = true;
        }

        private async void SearchBoxButton_Clicked(object sender, EventArgs e)
        {
            if (searchBox.Text.Length <= 0)
            {
                return;
            }

            IEnumerable<Position> resultLocation = await geoCoder.GetPositionsForAddressAsync(searchBox.Text);
            using (IEnumerator<Position> iter = resultLocation.GetEnumerator())
            {
                iter.MoveNext();
                customMap.MoveToRegion(MapSpan.FromCenterAndRadius(iter.Current, GetMapVisibleDistance()));
                StartSearchWithLocation(iter.Current);
            }
        }

        private void StartSearchWithLocation(Position point)
        {
            progressBar.IsRunning = true;
            JLYRestService.Instance.FindNearby(point.Latitude, point.Longitude, selectedType, 15, 0);
        }

        private void JLYResults(List<JLYServiceItem> items, Exception error)
        {
            Device.BeginInvokeOnMainThread(() => {
                
                progressBar.IsRunning = false;

                if (error != null)
                {
                    Debug.WriteLine("Error fetching data : " + error.Message);
                }
                else if (items != null)
                {
                    Debug.WriteLine("Done without errors, got items : " + items.Count);

                    customMap.ClearPins();
                    
                    foreach(JLYServiceItem item in items)
                    {
                        if(item != null)
                        {
                            customMap.AddPin(CustomPin.FromItem(item));
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("Done without anykind of results");
                }
            });
        }

        private void AboutButton_Clicked(object sender, EventArgs e)
        {
            aboutGrid.IsVisible = true;
        }

        private Position GetMapVisibleCenter()
        {
            if (customMap.VisibleRegion == null
             || customMap.VisibleRegion.Center == null)
            {
                return new Position(60.1708, 24.9375);
            }

            return customMap.VisibleRegion.Center;
        }

        private Distance GetMapVisibleDistance()
        {
            if (customMap.VisibleRegion == null
             || customMap.VisibleRegion.Radius == null)
            {
                return new Distance(15000);
            }

            return customMap.VisibleRegion.Radius;
        }

        private JLYServiceItem GetServiceItem(int index)
        {
            if (customMap.CustomPins.Count > index)
            {
                if (customMap.CustomPins[index] != null
                && customMap.CustomPins[index].Item != null)
                {
                    return customMap.CustomPins[index].Item;
                }
            }

            return null;
        }
    }
}
