using System;

namespace iBeaconProto.Features.Beacon.List
{
    public class BeaconViewModel: ViewModelBase
    {
        string _bluetoothAddress;
        public string BluetoothAddress
        {
            get => _bluetoothAddress;
            set => SetProperty(ref _bluetoothAddress, value);
        }

        string _uuid;
        public string UUID
        {
            get => _uuid;
            set => SetProperty(ref _uuid, value);
        }

        string _major;
        public string Major
        {
            get => _major;
            set => SetProperty(ref _major, value);
        }

        string _minor;
        public string Minor
        {
            get => _minor;
            set => SetProperty(ref _minor, value);
        }

        double _distance;
        public double Distance
        {
            get => _distance;
            set => SetProperty(ref _distance, value);
        }

        public DateTime LastUpdatedOn { get; set; }
    }
}
