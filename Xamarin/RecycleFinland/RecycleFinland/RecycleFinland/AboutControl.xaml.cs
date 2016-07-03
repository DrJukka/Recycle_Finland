using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace RecycleFinland
{
    public partial class AboutControl : Grid
    {
        public GoBackToMapView GoBackToMapView;
        public AboutControl()
        {
            InitializeComponent();
        }

        private void BackClicked(object sender, EventArgs e)
        {
            GoBackToMapView?.Invoke();
        }
    }
}
