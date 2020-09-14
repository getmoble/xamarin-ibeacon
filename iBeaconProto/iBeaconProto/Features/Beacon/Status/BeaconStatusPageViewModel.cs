using iBeaconProto.Features.Beacon.List;
using Provider.AltBeacon.Interfaces;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace iBeaconProto.Features.Beacon.Status
{
    public class BeaconStatusPageViewModel: ViewModelBase
    {
        bool _isMonitoring;
        public bool IsMonitoring
        {
            get => _isMonitoring;
            set => SetProperty(ref _isMonitoring, value);
        }

        BeaconViewModel _beacon;
        public BeaconViewModel Beacon
        {
            get => _beacon;
            set => SetProperty(ref _beacon, value);
        }

        string _status;
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public ICommand ActionCommand { get; set; }

        IBeaconService _altBeaconService;

        public BeaconStatusPageViewModel(BeaconViewModel beacon)
        {
            Beacon = beacon;

            _altBeaconService = DependencyService.Get<IBeaconService>();
            
            ActionCommand = new Command(() =>
            {
                if (!IsMonitoring)
                {
                    _altBeaconService.OnMonitorBeacons += AltBeaconService_OnMonitorBeacons;
                    _altBeaconService.StartMonitoring(beacon.UUID, beacon.Major, beacon.Minor);
                }
                else
                {
                    _altBeaconService.OnMonitorBeacons -= AltBeaconService_OnMonitorBeacons;
                    _altBeaconService.StopMonitoring(beacon.UUID, beacon.Major, beacon.Minor);
                }

                IsMonitoring = !IsMonitoring;
            });
        }

        void  AltBeaconService_OnMonitorBeacons(Provider.AltBeacon.Models.MonitorBeaconEventArgs obj)
        {
            Status = obj.Event == "Enter"? "Beacon Found": "Beacon Lost";

            Task.Run(async () =>
            {
                if (obj.Event == "Enter")
                {
                    await TextToSpeech.SpeakAsync("Beacon Found");
                }
                else if (obj.Event == "Exit")
                {
                    await TextToSpeech.SpeakAsync("Beacon Lost");
                }
            });
        }
    }
}
