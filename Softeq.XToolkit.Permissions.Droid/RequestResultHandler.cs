// Developed by Softeq Development Corporation
// http://www.softeq.com

using Android.Runtime;
using Plugin.Permissions;

namespace Softeq.XToolkit.Permissions.Droid
{
    public static class RequestResultHandler
    {
        public static void Handle(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
