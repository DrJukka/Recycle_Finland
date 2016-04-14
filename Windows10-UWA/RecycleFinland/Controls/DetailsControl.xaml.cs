using Microsoft.Azure.Engagement;
using RecycleFinland.Engine;
using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;


// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RecycleFinland.Controls
{
    public delegate void CenterMapForPoisDetails(Geopoint point);
    
    public sealed partial class DetailsControl : UserControl
    {
        public CloseView CloseView;
        public CenterMapForPoisDetails CenterMapForPoisDetails;
        
        private Double               _pointerDownX;
        private List<JLYServiceItem> _detailsArray;
        private int                  _detailsIndex;
        private const int            _minXMovementForSwipe = 50;
        public DetailsControl()
        {
            this.InitializeComponent();

            // we need to get swipe right/left
            // and it appears that mouse events work better 
            // than using the real gestures API provided, also implementation is simpler this way
            mainDetailsGrid.PointerPressed += DetailsListBox_PointerPressed;
            mainDetailsGrid.PointerReleased += DetailsListBox_PointerReleased;
        }

        public void Show(List<JLYServiceItem> array,int index)
        {
            _detailsArray = array;
            _detailsIndex = index;

            if(_detailsIndex >= 0 && _detailsIndex < array.Count)
            {
                showDetails(array[_detailsIndex]);
            }
        }

        private void backFromdetails_Click(object sender, RoutedEventArgs e)
        {
            CloseView?.Invoke();
        }

        private void DetailsListBox_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            double changeX = _pointerDownX - e.GetCurrentPoint(mainDetailsGrid).Position.X;
            e.Handled = true;

            if (changeX < -_minXMovementForSwipe)
            {
                ShowNextPreviousPoi(false);
            }
            else if (changeX > _minXMovementForSwipe)
            {
                ShowNextPreviousPoi(true);
            }
        }

        private void DetailsListBox_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _pointerDownX = e.GetCurrentPoint(mainDetailsGrid).Position.X;
            e.Handled = true;
        }

        private void ShowNextPreviousPoi(bool next)
        {
            if (_detailsArray == null || _detailsArray.Count <= 0)
            {
                return;
            }

            if (next)
            {
                _detailsIndex++;
            }
            else
            {
                _detailsIndex--;
            }

            if (_detailsIndex < 0)
            {
                _detailsIndex = (_detailsArray.Count - 1);
            }

            if (_detailsArray.Count <= _detailsIndex)
            {
                _detailsIndex = 0;
            }

            showDetails(_detailsArray[_detailsIndex]);
        }

        private void showDetails(JLYServiceItem poi)
        {
            if (poi == null)
            {
                return;
            }

            var extras = new Dictionary<object, object>();
            extras.Add("DisplayName", poi.DisplayName);
            extras.Add("City", poi.City);
            EngagementAgent.Instance.SendSessionEvent("Show_POI", extras);

            detailsName.Text = poi.DisplayName;
            detailsContact.Text = poi.Contact;
            detailsAddress.Text = poi.Address;
            detailsPCodeCity.Text = poi.PostalCode + "" + poi.City;
            detailsOpenTimes.Text = poi.OpenTimes;

            List<int> types = poi.MatrialTypes;
            detailTypes.Items.Clear();
            string typeString;
            foreach (int typeKey in types)
            {
                if (JLYConstants.materialTypes.TryGetValue(typeKey, out typeString))
                {
                    detailTypes.Items.Add(typeString);
                }
            }

            CenterMapForPoisDetails?.Invoke(poi.Location);
        }

        private async void navigatetoPlace_Click(object sender, RoutedEventArgs e)
        {
            if (_detailsArray != null && _detailsIndex >= 0 && _detailsIndex < _detailsArray.Count)
            {
                var extras = new Dictionary<object, object>();
                extras.Add("DisplayName", _detailsArray[_detailsIndex].DisplayName);
                extras.Add("City", _detailsArray[_detailsIndex].City);
                EngagementAgent.Instance.SendSessionEvent("NavigateTo", extras);

                string latStr = _detailsArray[_detailsIndex].Location.Position.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                string lonStr = _detailsArray[_detailsIndex].Location.Position.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);

                string url = "ms-drive-to:?destination.latitude=" + latStr + "&destination.longitude=" + lonStr;
                await Launcher.LaunchUriAsync(new Uri(url));
            }
        }
    }
}
