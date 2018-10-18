// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Foundation;
using Plugin.Permissions;
using UIKit;

namespace Softeq.XToolkit.Permissions.iOS
{
    public class PermissionsService : IPermissionsService
    {
        public async Task<PermissionStatus> RequestPermissionsAsync(Permission permission)
        {
            var pluginPermission = ToPluginPermission(permission);
            var result = await CrossPermissions.Current.RequestPermissionsAsync(pluginPermission);
            return result.TryGetValue(pluginPermission, out var permissionStatus) 
                ? ToPermissionStatus(permissionStatus) 
                : PermissionStatus.Unknown;
        }

        public async Task<PermissionStatus> CheckPermissionsAsync(Permission permission)
        {
            var result = await CrossPermissions.Current
                .CheckPermissionStatusAsync(ToPluginPermission(permission)).ConfigureAwait(false);
            return ToPermissionStatus(result);
        }

        public void OpenSettings()
        {
            if (NSThread.IsMain)
            {
                CrossPermissions.Current.OpenAppSettings();
            }
            else
            {
                UIApplication.SharedApplication.BeginInvokeOnMainThread(() => { CrossPermissions.Current.OpenAppSettings(); });
            }
        }

        private PermissionStatus ToPermissionStatus(Plugin.Permissions.Abstractions.PermissionStatus permissionStatus)
        {
            switch (permissionStatus)
            {
                case Plugin.Permissions.Abstractions.PermissionStatus.Denied:
                    return PermissionStatus.Denied;
                case Plugin.Permissions.Abstractions.PermissionStatus.Disabled:
                    return PermissionStatus.Denied;
                case Plugin.Permissions.Abstractions.PermissionStatus.Granted:
                    return PermissionStatus.Granted;
                case Plugin.Permissions.Abstractions.PermissionStatus.Restricted:
                    return PermissionStatus.Denied;
                case Plugin.Permissions.Abstractions.PermissionStatus.Unknown:
                    return PermissionStatus.Unknown;
                default:
                    throw new ArgumentOutOfRangeException(nameof(permissionStatus), permissionStatus, null);
            }
        }

        private static Plugin.Permissions.Abstractions.Permission ToPluginPermission(Permission permission)
        {
            switch (permission)
            {
                case Permission.Camera:
                    return Plugin.Permissions.Abstractions.Permission.Camera;
                case Permission.Storage:
                    return Plugin.Permissions.Abstractions.Permission.Storage;
                case Permission.MediaLibrary:
                    return Plugin.Permissions.Abstractions.Permission.MediaLibrary;
                default:
                    throw new NotImplementedException();
            }
        }
        
        private static Permission ToPermission(Plugin.Permissions.Abstractions.Permission permission)
        {
            switch (permission)
            {
                case Plugin.Permissions.Abstractions.Permission.Camera:
                    return Permission.Camera;
                case Plugin.Permissions.Abstractions.Permission.MediaLibrary:
                    return Permission.MediaLibrary;
                case Plugin.Permissions.Abstractions.Permission.Storage:
                    return Permission.Storage;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
