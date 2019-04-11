// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using CoreBluetooth;
using CoreFoundation;
using Foundation;
using Plugin.Permissions;
using UIKit;
using UserNotifications;
using System.ComponentModel;

namespace Softeq.XToolkit.Permissions.iOS
{
    public class PermissionsService : IPermissionsService
    {
#if DEBUG || RELEASE_WITH_BLE
        private readonly CBCentralManager _bleManager;
        private CBCentralManager _bleManagerWithAlert;
#endif
        public PermissionsService()
        {
#if DEBUG || RELEASE_WITH_BLE
            _bleManager = new CBCentralManager(new CustomCBCentralManagerDelegate(), DispatchQueue.MainQueue,
                new CBCentralInitOptions {ShowPowerAlert = false});
#endif
        }

        public async Task<PermissionStatus> RequestPermissionsAsync(Permission permission)
        {
            if (permission == Permission.Notifications)
            {
                return await RequestNotificationPermissionAsync().ConfigureAwait(false);
            }

            if (permission == Permission.Bluetooth)
            {
                return RequestBluetoothPermission();
            }

            var pluginPermission = ToPluginPermission(permission);
            var result = await CrossPermissions.Current.RequestPermissionsAsync(pluginPermission);
            return result.TryGetValue(pluginPermission, out var permissionStatus)
                ? ToPermissionStatus(permissionStatus)
                : PermissionStatus.Unknown;
        }

        public async Task<PermissionStatus> CheckPermissionsAsync(Permission permission)
        {
            if (permission == Permission.Bluetooth)
            {
                return CheckBluetoothPermission();
            }

            if (permission == Permission.Notifications)
            {
                return CheckNotificationsPermission();
            }

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
                UIApplication.SharedApplication.BeginInvokeOnMainThread(() =>
                {
                    CrossPermissions.Current.OpenAppSettings();
                });
            }
        }

        private PermissionStatus CheckBluetoothPermission()
        {
            var result = PermissionStatus.Unknown;

            if (_bleManager.State == CBCentralManagerState.PoweredOn)
            {
                result = PermissionStatus.Granted;
            }

            if (_bleManager.State == CBCentralManagerState.PoweredOff)
            {
                result = PermissionStatus.Denied;
            }

            return result;
        }

        private PermissionStatus RequestBluetoothPermission()
        {
#if DEBUG || RELEASE_WITH_BLE
            _bleManagerWithAlert = new CBCentralManager(new CustomCBCentralManagerDelegate(),
                DispatchQueue.CurrentQueue,
                new CBCentralInitOptions {ShowPowerAlert = true});
#endif
            return PermissionStatus.Unknown;
        }

        private PermissionStatus CheckNotificationsPermission()
        {
            var types = UIUserNotificationType.None;

            UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                types = UIApplication.SharedApplication.CurrentUserNotificationSettings.Types;
            });

            if (types.HasFlag(UIUserNotificationType.Alert))
            {
                return PermissionStatus.Granted;
            }

            return PermissionStatus.Unknown;
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
                case Permission.LocationInUse:
                    return Plugin.Permissions.Abstractions.Permission.LocationWhenInUse;
                case Permission.Bluetooth:
                case Permission.Notifications:
                    throw new InvalidEnumArgumentException(
                        $"Plugin.Permissions does not work with {permission} permissions. " +
                        "Please handle it separately");
                default:
                    throw new NotImplementedException();
            }
        }
    }
}