package crc6410869f98e25cdc1f;


public class AltBeaconService
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("iBeaconProto.Droid.Services.AltBeaconService, iBeaconProto.Android", AltBeaconService.class, __md_methods);
	}


	public AltBeaconService ()
	{
		super ();
		if (getClass () == AltBeaconService.class)
			mono.android.TypeManager.Activate ("iBeaconProto.Droid.Services.AltBeaconService, iBeaconProto.Android", "", this, new java.lang.Object[] {  });
	}

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
