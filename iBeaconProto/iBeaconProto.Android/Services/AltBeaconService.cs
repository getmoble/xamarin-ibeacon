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

[assembly: Xamarin.Forms.Dependency(typeof(AltBeaconService))]
namespace iBeaconProto.Droid.Services
{
    public class AltBeaconService : Java.Lang.Object, IAltBeaconService
    {
        object _lock = new object();

        string foregroundServiceChannelId = "foregroundService";

        public event Action<RangingBeaconEventArgs> OnRangingBeacons;

        public event Action<MonitorBeaconEventArgs> OnMonitorBeacons;

        readonly MonitorNotifier _monitorNotifier;
        
        readonly RangeNotifier _rangeNotifier;

        List<SharedBeacon> _sharedBeacons = new List<SharedBeacon>();
        
        BeaconManager _beaconManager;

        public AltBeaconService()
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
            _monitorNotifier.DetermineStateForRegionComplete += DeterminedStateForRegionComplete;
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
            builder.SetContentTitle("Scanning for Beacons");
            Intent intent = new Intent(Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity, typeof(AltBeaconService));
            PendingIntent pendingIntent = PendingIntent.GetActivity(Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity, 1, intent, PendingIntentFlags.UpdateCurrent);
            builder.SetContentIntent(pendingIntent);
            return builder.Build();
        }

        public void StartRanging(string name, string uuid)
        {
            BeaconManagerImpl.AddRangeNotifier(_rangeNotifier);

            try
            {
                var tagRegion = new Region(name, Identifier.Parse(uuid), null, null);
                BeaconManagerImpl.StartRangingBeaconsInRegion(tagRegion);
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine("StartRangingException: " + ex.Message);
            }

        }

        public void StopRanging(string name, string uuid)
        {
            if (_beaconManager != null)
            {
                try
                {
                    var tagRegion = new Region(name, Identifier.Parse(uuid), null, null);
                    BeaconManagerImpl.StopRangingBeaconsInRegion(tagRegion);
                    BeaconManagerImpl.RemoveAllRangeNotifiers();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("StopRangingException: " + ex.Message);
                }
            }
        }

        public void StartMonitoring(string name, string uuid)
        {
            var tagRegion = new Region(name, Identifier.Parse(uuid), null, null);
            BeaconManagerImpl.AddMonitorNotifier(_monitorNotifier);
            BeaconManagerImpl.StartMonitoringBeaconsInRegion(tagRegion);
        }

        public void StopMonitoring(string name, string uuid)
        {
            if (_beaconManager != null)
            {
                try
                {
                    var tagRegion = new Region(name, Identifier.Parse(uuid), null, null);
                    BeaconManagerImpl.StopMonitoringBeaconsInRegion(tagRegion);
                    BeaconManagerImpl.RemoveAllMonitorNotifiers();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("StopMonitoringException: " + ex.Message);
                }
            }
        }

        void DeterminedStateForRegionComplete(object sender, MonitorEventArgs e)
        {
            Console.WriteLine("DeterminedStateForRegionComplete");
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

            System.Diagnostics.Debug.WriteLine("ExitedRegion");

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

            System.Diagnostics.Debug.WriteLine("EnteredRegion");

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
            _sharedBeacons = new List<SharedBeacon>();

            lock (_lock)
            {

                // Get all beacons and create the SharedBeacon
                foreach (Beacon beacon in e.Beacons)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("NAME {0} - IP {1} - {2}dB", beacon.BluetoothName, beacon.BluetoothAddress, beacon.Rssi));
                    _sharedBeacons.Add(new SharedBeacon(beacon.BluetoothName, beacon.BluetoothAddress, beacon.Id1.ToString(), beacon.Id2.ToString(), beacon.Id3.ToString(), beacon.Distance, beacon.Rssi));
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