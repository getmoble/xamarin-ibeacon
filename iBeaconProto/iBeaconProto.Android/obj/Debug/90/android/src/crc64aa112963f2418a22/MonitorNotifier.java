package crc64aa112963f2418a22;


public class MonitorNotifier
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		org.altbeacon.beacon.MonitorNotifier
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_didDetermineStateForRegion:(ILorg/altbeacon/beacon/Region;)V:GetDidDetermineStateForRegion_ILorg_altbeacon_beacon_Region_Handler:Org.Altbeacon.Beacon.IMonitorNotifierInvoker, BeaconLibraryBindings\n" +
			"n_didEnterRegion:(Lorg/altbeacon/beacon/Region;)V:GetDidEnterRegion_Lorg_altbeacon_beacon_Region_Handler:Org.Altbeacon.Beacon.IMonitorNotifierInvoker, BeaconLibraryBindings\n" +
			"n_didExitRegion:(Lorg/altbeacon/beacon/Region;)V:GetDidExitRegion_Lorg_altbeacon_beacon_Region_Handler:Org.Altbeacon.Beacon.IMonitorNotifierInvoker, BeaconLibraryBindings\n" +
			"";
		mono.android.Runtime.register ("iBeaconProto.Droid.MonitorNotifier, iBeaconProto.Android", MonitorNotifier.class, __md_methods);
	}


	public MonitorNotifier ()
	{
		super ();
		if (getClass () == MonitorNotifier.class)
			mono.android.TypeManager.Activate ("iBeaconProto.Droid.MonitorNotifier, iBeaconProto.Android", "", this, new java.lang.Object[] {  });
	}


	public void didDetermineStateForRegion (int p0, org.altbeacon.beacon.Region p1)
	{
		n_didDetermineStateForRegion (p0, p1);
	}

	private native void n_didDetermineStateForRegion (int p0, org.altbeacon.beacon.Region p1);


	public void didEnterRegion (org.altbeacon.beacon.Region p0)
	{
		n_didEnterRegion (p0);
	}

	private native void n_didEnterRegion (org.altbeacon.beacon.Region p0);


	public void didExitRegion (org.altbeacon.beacon.Region p0)
	{
		n_didExitRegion (p0);
	}

	private native void n_didExitRegion (org.altbeacon.beacon.Region p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
