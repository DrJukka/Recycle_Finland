using RecycleFinland.iOS;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(NativeInterface))]
namespace RecycleFinland.iOS
{
    public class NativeInterface : INativeInterface
    {
        public void NavigateToPosition(double latitude, double longitude)
        {
            throw new NotImplementedException();
        }

        public void RemovePositionChangedDelegate(UserPositionChanged callback)
        {
            throw new NotImplementedException();
        }

        public void SetPositionChangedDelegate(UserPositionChanged callback)
        {
            throw new NotImplementedException();
        }
    }
}
