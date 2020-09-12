using iBeaconProto.Features.Beacon.List;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace iBeaconProto.Features.Beacon.Status
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BeaconStatusPage : ContentPage
    {
        public BeaconStatusPage(BeaconViewModel beacon)
        {
            InitializeComponent();
            BindingContext = new BeaconStatusPageViewModel(beacon);
        }
    }
}