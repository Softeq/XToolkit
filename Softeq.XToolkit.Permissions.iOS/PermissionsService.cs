// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using CoreLocation;
using Foundation;
using Plugin.Permissions;
using UIKit;
using UserNotifications;

namespace Softeq.XToolkit.Permissions.iOS
{
    public class PermissionsService : IPermissionsService
    {
        public async Task<PermissionStatus> RequestPermissionsAsync(Permission permission)
        {
            if (permission == Permission.Notifications)
            {
                return await RequestNotificationPermissionAsync().ConfigureAwait(false);
            }

            if (permission == Permission.Location)
            {
                return await RequestLocationPermissionAsync().ConfigureAwait(false);
            }

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

        private static async Task<PermissionStatus> RequestNotificationPermissionAsync()
        {
            var notificationCenter = UNUserNotificationCenter.Current;
            var result = await notificationCenter.RequestAuthorizationAsync(
                UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound);
            return result.Item1 ? PermissionStatus.Granted : PermissionStatus.Denied;
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
                case Permission.Photos:
                    return Plugin.Permissions.Abstractions.Permission.Photos;
                case Permission.Location:
                    return Plugin.Permissions.Abstractions.Permission.Location; // Plugin is not returning result for some reason (iOS 11), added custom logic below
                case Permission.Microphone:
                    return Plugin.Permissions.Abstractions.Permission.Microphone;
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
                case Plugin.Permissions.Abstractions.Permission.Photos:
                    return Permission.Photos;
                case Plugin.Permissions.Abstractions.Permission.Storage:
                    return Permission.Storage;
                case Plugin.Permissions.Abstractions.Permission.Location:
                    return Permission.Location;
                case Plugin.Permissions.Abstractions.Permission.Microphone:
                    return Permission.Microphone;
                default:
                    throw new NotImplementedException();
            }
        }

        #region Location permission
        private readonly CLLocationManager _locationManager = new CLLocationManager();
        private TaskCompletionSource<PermissionStatus> _taskCompletionSource;

        private Task<PermissionStatus> RequestLocationPermissionAsync()
        {
            _taskCompletionSource = new TaskCompletionSource<PermissionStatus>();
            _locationManager.AuthorizationChanged += OnLocationAuthorizationChanged;
            _locationManager.RequestWhenInUseAuthorization();

            return _taskCompletionSource.Task;
        }

        private void OnLocationAuthorizationChanged(object sender, CLAuthorizationChangedEventArgs e)
        {
            if (_taskCompletionSource != null)
            {
                _taskCompletionSource.TrySetResult(ToPermissionStatus(e.Status));
                _taskCompletionSource = null;
            }
            _locationManager.AuthorizationChanged -= OnLocationAuthorizationChanged;
        }

        private PermissionStatus ToPermissionStatus(CLAuthorizationStatus status)
        {
            switch (status)
            {
                case CLAuthorizationStatus.Authorized:
                case CLAuthorizationStatus.AuthorizedWhenInUse:
                    return PermissionStatus.Granted;
                case CLAuthorizationStatus.Denied:
                case CLAuthorizationStatus.Restricted:
                    return PermissionStatus.Denied;
                case CLAuthorizationStatus.NotDetermined:
                default:
                    return PermissionStatus.Unknown;
            }
        }
        #endregion
    }
}
