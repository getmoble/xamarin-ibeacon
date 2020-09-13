using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace iBeaconProto.Utils
{
    public static class AppPermissions
    {
        public static async Task<bool> CheckPermissions()
        {
            bool requested = false;

            var permissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
            if(permissionStatus == PermissionStatus.Granted)
            {
                return true;
            }
            else
            {
                if (!requested)
                {
                    var newStatus = await Permissions.RequestAsync<Permissions.LocationAlways>();

                    if (newStatus == PermissionStatus.Granted)
                    {
                        return true;
                    }

                    if (newStatus != PermissionStatus.Granted)
                    {
                        var title = $"Location Always Permission";
                        var question = $"To use the plugin the Location Always permission is required.";
                        await Application.Current?.MainPage?.DisplayAlert(title, question, "Cancel");
                        return false;
                    }
                }

                return false;
            }
        }
    }
}
