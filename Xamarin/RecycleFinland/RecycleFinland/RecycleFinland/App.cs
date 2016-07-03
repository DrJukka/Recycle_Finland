using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace RecycleFinland
{
    public class App : Application
    {
        public static double ScreenHeight;
        public static double ScreenWidth;

        public App()
        {
            // The root page of your application
            MainPage = new MapPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            ((MapPage)MainPage).AppToSleep();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            ((MapPage)MainPage).AppResume();
        }
    }
}
