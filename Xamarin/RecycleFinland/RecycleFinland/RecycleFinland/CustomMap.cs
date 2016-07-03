using RecycleFinland.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms.Maps;

namespace RecycleFinland
{
    public delegate void PlaceSelected(int index);
    public class CustomMap : Map
    {
        public PlaceSelected PlaceSelected;
        public List<CustomPin> CustomPins { get; set; }

        public void ClearPins()
        {
            Pins.Clear();
            CustomPins = new List<CustomPin>();
        }

        public void AddPin(CustomPin pin)
        {
            CustomPins.Add(pin);
            Pins.Add(pin.Pin);

            pin.Pin.Clicked += (sender, e) =>
            {
                try
                {
                    Debug.WriteLine("Pin Clicked : " + ((Pin)sender).Label);

                    for (int i = 0; i < CustomPins.Count; i++)
                    {
                        if(CustomPins[i] != null && CustomPins[i].Pin.Equals(sender))
                        {
                            PlaceSelected?.Invoke(i);
                            return;
                        }
                    }   
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Pin Clicked Exception: " + ex.Message);
                }
            };
        }
    }
}
