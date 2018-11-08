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

            OneSignal.Current.StartInit("efd8208c-6f2e-4dff-ba45-d8865fbd2610").EndInit();
        }

        protected override void OnStart()
        {
            OneSignal.Current.StartInit("efd8208c-6f2e-4dff-ba45-d8865fbd2610").EndInit();
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
