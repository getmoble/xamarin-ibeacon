using Android.App;
using Android.OS;
using Android.Runtime;
using Plugin.CurrentActivity;
using System;

namespace iBeaconProto.Droid
{
#if DEBUG
    [Application(Debuggable = true)]
#else
	[Application(Debuggable = false)]
#endif
    public class MainApplication : Application
    {
        string foregroundServiceChannelId = "foregroundService";
        string channelName = "ForegroundService";

        public MainApplication(IntPtr handle, JniHandleOwnership transer)
          : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            CrossCurrentActivity.Current.Init(this);
            CreateNotificationChannel();
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }
            var channelDescription = "Foreground Sertrvice";
            var channel = new NotificationChannel(foregroundServiceChannelId, channelName, NotificationImportance.High)
            {
                Description = channelDescription
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }
}