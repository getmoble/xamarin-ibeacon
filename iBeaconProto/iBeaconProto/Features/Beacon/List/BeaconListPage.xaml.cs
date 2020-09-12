using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace iBeaconProto.Features.Beacon.List
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BeaconListPage : ContentPage
    {
        public BeaconListPage()
        {
            InitializeComponent();
            BindingContext = new BeaconListPageViewModel(this);
        }
    }
}