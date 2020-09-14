using System;
using System.Collections.Generic;

namespace Provider.AltBeacon.Models
{
    public class RangingBeaconEventArgs: EventArgs
    {
        public List<Beacon> Beacons { get; set; }
    }
}
