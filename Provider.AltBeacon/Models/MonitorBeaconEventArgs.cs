using System;

namespace Provider.AltBeacon.Models
{
    public class MonitorBeaconEventArgs: EventArgs
    {
        public string Event { get; set; }
    }
}
