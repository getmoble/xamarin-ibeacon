using iBeaconProto.Features.Beacon.List;
using Provider.AltBeacon.Interfaces;
using Xamarin.Forms;

namespace iBeaconProto
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            DependencyService.Get<IAltBeaconService>().InitializeService();
            MainPage = new NavigationPage(new BeaconListPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            DependencyService.Get<IAltBeaconService>().SetBackgroundMode(false);
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            DependencyService.Get<IAltBeaconService>().SetBackgroundMode(true);
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            DependencyService.Get<IAltBeaconService>().SetBackgroundMode(false);
        }
    }
}
