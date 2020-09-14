using Provider.AltBeacon.Models;
using System;

namespace Provider.AltBeacon.Interfaces
{
	public interface IBeaconService
	{
		event Action<RangingBeaconEventArgs> OnRangingBeacons;

		event Action<MonitorBeaconEventArgs> OnMonitorBeacons;

		void InitializeService();	
		void StartMonitoring(string uuid, string major, string minor);
		void StopMonitoring(string uuid, string major, string minor);
		void StartRanging(string uuid);
        void StopRanging(string uuid);
        void SetBackgroundMode(bool isBackground);
        void OnDestroy();
	}
}