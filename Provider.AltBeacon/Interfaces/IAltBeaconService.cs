using Provider.AltBeacon.Models;
using System;

namespace Provider.AltBeacon.Interfaces
{
	public interface IAltBeaconService
	{
		event Action<RangingBeaconEventArgs> OnRangingBeacons;

		event Action<MonitorBeaconEventArgs> OnMonitorBeacons;

		void InitializeService();	
		void StartMonitoring(string name, string uuid);
		void StopMonitoring(string name, string uuid);
		void StartRanging(string name, string uuid);
        void StopRanging(string name, string uuid);
        void SetBackgroundMode(bool isBackground);
        void OnDestroy();
	}
}