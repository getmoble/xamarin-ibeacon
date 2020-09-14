using Android.App;
using Android.Content;
using Android.Support.V4.App;
using iBeaconProto.Droid.Services;
using Org.Altbeacon.Beacon;
using Provider.AltBeacon.Interfaces;
using Provider.AltBeacon.Models;
using System;
using System.Collections.Generic;
using System.Linq;

[assembly: Xamarin.Forms.Dependency(typeof(BeaconService))]
namespace iBeaconProto.Droid.Services
{
    public class BeaconService : Java.Lang.Object, IBeaconService
    {
        object _lock = new object();

        string foregroundServiceChannelId = "foregroundService";

        string _rangingRegion = "ranging-region";
        string _monitorRegion = "monitor-region";


        public event Action<RangingBeaconEventArgs> OnRangingBeacons;

        public event Action<MonitorBeaconEventArgs> OnMonitorBeacons;

        readonly MonitorNotifier _monitorNotifier;

        readonly RangeNotifier _rangeNotifier;

        List<Provider.AltBeacon.Models.Beacon> _sharedBeacons = new List<Provider.AltBeacon.Models.Beacon>();

        BeaconManager _beaconManager;

        public BeaconService()
        {
            _monitorNotifier = new MonitorNotifier();
            _rangeNotifier = new RangeNotifier();
        }

        public BeaconManager BeaconManagerImpl
        {
            get
            {
                if (_beaconManager == null)
                    _beaconManager = InitializeBeaconManager();
                return _beaconManager;
            }
        }

        public void InitializeService()
        {
            if (_beaconManager == null)
                _beaconManager = InitializeBeaconManager();
        }

        BeaconManager InitializeBeaconManager()
        {
            // Enable the BeaconManager 
            BeaconManager bm = BeaconManager.GetInstanceForApplication(Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity);

            var iBeaconParser = new BeaconParser();
            //	Estimote > 2013
            iBeaconParser.SetBeaconLayout("m:2-3=0215,i:4-19,i:20-21,i:22-23,p:24-24");
            bm.BeaconParsers.Add(iBeaconParser);

            _monitorNotifier.EnterRegionComplete += EnteredRegion;
            _monitorNotifier.ExitRegionComplete += ExitedRegion;
            _rangeNotifier.DidRangeBeaconsInRegionComplete += RangingBeaconsInRegion;

            bm.EnableForegroundServiceScanning(GetForegroundServiceNotification(), 456);

            bm.BackgroundScanPeriod = 1100;
            bm.BackgroundBetweenScanPeriod = 5000;

            bm.ForegroundScanPeriod = 1100;
            bm.ForegroundBetweenScanPeriod = 5000;

            bm.BackgroundMode = false;

            bm.Bind((IBeaconConsumer)Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity);

            return bm;
        }

        public Notification GetForegroundServiceNotification()
        {
            NotificationCompat.Builder builder = new NotificationCompat.Builder(Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity, foregroundServiceChannelId);
            builder
                .SetContentTitle("iBeacon Proto")
                .SetContentText("Monitoring Beacons");
            Intent intent = new Intent(Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity, typeof(BeaconService));
            PendingIntent pendingIntent = PendingIntent.GetActivity(Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity, 1, intent, PendingIntentFlags.UpdateCurrent);
            builder.SetContentIntent(pendingIntent);
            return builder.Build();
        }

        public void StartRanging(string uuid)
        {
            BeaconManagerImpl.AddRangeNotifier(_rangeNotifier);
            var tagRegion = new Region(_rangingRegion, Identifier.Parse(uuid), null, null);
            BeaconManagerImpl.StartRangingBeaconsInRegion(tagRegion);
        }

        public void StopRanging(string uuid)
        {
            if (_beaconManager != null)
            {
                var tagRegion = new Region(_rangingRegion, Identifier.Parse(uuid), null, null);
                BeaconManagerImpl.StopRangingBeaconsInRegion(tagRegion);
                BeaconManagerImpl.RemoveAllRangeNotifiers();
            }
        }

        public void StartMonitoring(string uuid, string major, string minor)
        {
            var tagRegion = new Region(_monitorRegion, Identifier.Parse(uuid), Identifier.Parse(major), Identifier.Parse(minor));
            BeaconManagerImpl.AddMonitorNotifier(_monitorNotifier);
            BeaconManagerImpl.StartMonitoringBeaconsInRegion(tagRegion);
        }

        public void StopMonitoring(string uuid, string major, string minor)
        {
            if (_beaconManager != null)
            {
                var tagRegion = new Region(_monitorRegion, Identifier.Parse(uuid), Identifier.Parse(major), Identifier.Parse(minor));
                BeaconManagerImpl.StopMonitoringBeaconsInRegion(tagRegion);
                BeaconManagerImpl.RemoveAllMonitorNotifiers();
            }
        }

        void ExitedRegion(object sender, MonitorEventArgs e)
        {
            string region = "???";
            if (e.Region != null)
            {
                if (e.Region.Id1 == null)
                    region = "null region";
                else
                    region = e.Region.Id1.ToString().ToUpper();
            }

            if (OnMonitorBeacons != null)
            {
                OnMonitorBeacons.Invoke(new MonitorBeaconEventArgs()
                {
                    Event = "Exit"
                });
            }
        }

        void EnteredRegion(object sender, MonitorEventArgs e)
        {
            string region = "???";
            if (e.Region != null)
            {
                if (e.Region.Id1 == null)
                    region = "null region";
                else
                    region = e.Region.Id1.ToString().ToUpper();
            }

            if (OnMonitorBeacons != null)
            {
                OnMonitorBeacons.Invoke(new MonitorBeaconEventArgs()
                {
                    Event = "Enter"
                });
            }
        }

        void RangingBeaconsInRegion(object sender, RangeEventArgs e)
        {
            _sharedBeacons = new List<Provider.AltBeacon.Models.Beacon>();

            lock (_lock)
            {
                // Get all beacons and create the SharedBeacon
                foreach (Org.Altbeacon.Beacon.Beacon beacon in e.Beacons)
                {
                    _sharedBeacons.Add(new Provider.AltBeacon.Models.Beacon(beacon.BluetoothName, beacon.BluetoothAddress, beacon.Id1.ToString(), beacon.Id2.ToString(), beacon.Id3.ToString(), beacon.Distance, beacon.Rssi));
                };

                if (_sharedBeacons.Count > 0 && OnRangingBeacons != null)
                {
                    OnRangingBeacons.Invoke(new RangingBeaconEventArgs()
                    {
                        Beacons = _sharedBeacons.ToList()
                    });
                }
            }
        }

        public void SetBackgroundMode(bool isBackground)
        {
            if (_beaconManager != null)
                BeaconManagerImpl.BackgroundMode = isBackground;
        }

        public void OnDestroy()
        {
            if (_beaconManager != null && BeaconManagerImpl.IsBound((IBeaconConsumer)Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity))
                BeaconManagerImpl.Unbind((IBeaconConsumer)Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity);
        }
    }
}