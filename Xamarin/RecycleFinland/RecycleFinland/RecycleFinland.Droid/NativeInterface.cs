using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RecycleFinland.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(NativeInterface))]

namespace RecycleFinland.Droid
{
    public class NativeInterface : INativeInterface
    {
        public void NavigateToPosition(double latitude, double longitude)
        {
            
        }

        public void RemovePositionChangedDelegate(UserPositionChanged callback)
        {
            
        }

        public void SetPositionChangedDelegate(UserPositionChanged callback)
        {
            
        }
    }
}