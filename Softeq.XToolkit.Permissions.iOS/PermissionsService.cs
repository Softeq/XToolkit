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

namespace Softeq.XToolkit.Permissions.iOS
{
    public class PermissionsService : IPermissionsService
    {
        private readonly CBCentralManager _bleManager;

        public PermissionsService()
        {
            _bleManager = new CBCentralManager(new CustomCBCentralManagerDelegate(), DispatchQueue.MainQueue,
                new CBCentralInitOptions {ShowPowerAlert = false});
        }

        public async Task<PermissionStatus> RequestPermissionsAsync(Permission permission)
        {
            if (permission == Permission.Notifications)
            {
                return await RequestNotificationPermissionAsync().ConfigureAwait(false);
            }

            if (permission == Permission.Bluetooth)
            {
                return await RequestBluetoothPermissionAsync();
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

        private Task<PermissionStatus> RequestBluetoothPermissionAsync()
        {
            //creates manager and create alert
            var manager = new CBCentralManager(new CustomCBCentralManagerDelegate(), DispatchQueue.MainQueue,
                new CBCentralInitOptions {ShowPowerAlert = true});

            var taskSource = new TaskCompletionSource<PermissionStatus>();

            if (_bleManager.State == CBCentralManagerState.PoweredOn)
            {
                taskSource.SetResult(PermissionStatus.Granted);
                return taskSource.Task;
            }

            UIApplication.SharedApplication.InvokeOnMainThread(async () =>
            {
                //Waiting until system dialog will be shown
                await Task.Delay(500);

                while (UIApplication.SharedApplication.ApplicationState == UIApplicationState.Inactive)
                {
                    //wait until dialog closed, application alwayes in inactive state
                    await Task.Delay(100);
                }

                //if user click OK button
                if (UIApplication.SharedApplication.ApplicationState == UIApplicationState.Active)
                {
                    taskSource.SetResult(CheckBluetoothPermission());
                }
            });

            return taskSource.Task;
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
                default:
                    throw new NotImplementedException();
            }
        }
    }
}