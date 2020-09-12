﻿using iBeaconProto.Features.Beacon.Status;
using iBeaconProto.Utils;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Provider.AltBeacon.Interfaces;
using Provider.AltBeacon.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace iBeaconProto.Features.Beacon.List
{
    public class BeaconListPageViewModel : ViewModelBase
    {
        bool _isScanning;
        public bool IsScanning
        {
            get => _isScanning;
            set => SetProperty(ref _isScanning, value);
        }

        BeaconViewModel _selectedData;
        public BeaconViewModel SelectedData
        {
            get => _selectedData;
            set => SetProperty(ref _selectedData, value);
        }

        ObservableCollection<BeaconViewModel> _data = new ObservableCollection<BeaconViewModel>();
        public ObservableCollection<BeaconViewModel> Data
        {
            get => _data;
            set => SetProperty(ref _data, value);
        }

        public ICommand ActionCommand { get; set; }
        public ICommand GotoStatusPageCommand { get; set; }

        IAltBeaconService _altBeaconService;

        public BeaconListPageViewModel(Page page)
        {
            _altBeaconService = DependencyService.Get<IAltBeaconService>();

            page.Appearing += Page_Appearing;
            page.Disappearing += Page_Disappearing;
            
            ActionCommand = new Command(async () =>
            {
                if (!IsScanning)
                {
                    _altBeaconService.StartRanging(Constants.Beacon.Name, Constants.Beacon.UUID);
                }
                else
                {
                    _altBeaconService.StopRanging(Constants.Beacon.Name, Constants.Beacon.UUID);
                }

                IsScanning = !IsScanning;
            });

            GotoStatusPageCommand = new Command(async () =>
            {
                if(SelectedData != null)
                {
                    _altBeaconService.StopRanging(Constants.Beacon.Name, Constants.Beacon.UUID);
                    await page.Navigation.PushAsync(new BeaconStatusPage(SelectedData));
                    SelectedData = null;
                }
            });
        }

        void Page_Disappearing(object sender, System.EventArgs e)
        {
            _altBeaconService.OnRangingBeacons -= AltBeaconService_OnRangingBeacons;
            _altBeaconService.StopRanging(Constants.Beacon.Name, Constants.Beacon.UUID);
        }

        async void Page_Appearing(object sender, System.EventArgs e)
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

            if (status != PermissionStatus.Granted)
                status = await Permissions.CheckPermissions(Permission.Location);
            
            _altBeaconService.OnRangingBeacons += AltBeaconService_OnRangingBeacons;
        }

        void AltBeaconService_OnRangingBeacons(RangingBeaconEventArgs obj)
        {
            var timestamp = DateTime.UtcNow;

            foreach (var item in obj.Beacons)
            {
                var selectedBeacon = Data.FirstOrDefault(b => b.UUID == item.Id1);
                if (selectedBeacon == null)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Data.Add(new BeaconViewModel()
                        {
                            UUID = item.Id1,
                            Major = item.Id2,
                            Minor = item.Id3,
                            BluetoothAddress = item.BluetoothAddress,
                            Distance = item.Distance,
                            LastUpdatedOn = timestamp
                        });
                    });
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        selectedBeacon.Distance = item.Distance;
                        selectedBeacon.LastUpdatedOn = timestamp;
                    });
                }
            }

            var obsoluteList = Data.Where(b => b.LastUpdatedOn < DateTime.UtcNow.AddSeconds(-3)).ToList();
            foreach (var obsolute in obsoluteList)
            {
                Data.Remove(obsolute);
            }
        }
    }
}
