using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecycleFinland
{
    public delegate void UserPositionChanged(double latitude, double longitude);

    public interface INativeInterface
    {
        void SetPositionChangedDelegate(UserPositionChanged callback);
        void RemovePositionChangedDelegate(UserPositionChanged callback);
        void NavigateToPosition(double latitude, double longitude);
    }
}
