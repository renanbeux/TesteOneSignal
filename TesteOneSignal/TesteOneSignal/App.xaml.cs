using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Com.OneSignal;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TesteOneSignal
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();

            if (Device.RuntimePlatform != Device.UWP)
            {
                OneSignal.Current.StartInit("efd8208c-6f2e-4dff-ba45-d8865fbd2610").EndInit();
                OneSignal.Current.IdsAvailable(IdsAvaiable);
            }
        }
              
        protected override void OnStart()
        {
            if (Device.RuntimePlatform != Device.UWP)
            {
                OneSignal.Current.StartInit("efd8208c-6f2e-4dff-ba45-d8865fbd2610").EndInit();
                OneSignal.Current.IdsAvailable(IdsAvaiable);
            }
        }

        private async void IdsAvaiable(string userID, string pushToken)
        {
            if (App.Current.Properties["userID"].ToString() != userID)
            {
                App.Current.Properties["userID"] = userID;
                App.Current.Properties["pushToken"] = pushToken;
                await App.Current.SavePropertiesAsync();
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
