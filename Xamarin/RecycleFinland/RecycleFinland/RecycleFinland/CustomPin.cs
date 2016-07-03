using RecycleFinland.Engine;
using System;
using System.Collections.Generic;
using Xamarin.Forms.Maps;

namespace RecycleFinland
{
    public class CustomPin
    {
        public Pin Pin { get; set; }
        public JLYServiceItem Item { get; set; }

        public static CustomPin FromItem(JLYServiceItem item)
        {
            return new CustomPin
            {
                Pin = new Pin
                {
                    Type = PinType.Place,
                    Position = item.Location,
                    Label = item.DisplayName,
                    Address = item.Address
                },
                Item = item
            };
        }
    }
}
