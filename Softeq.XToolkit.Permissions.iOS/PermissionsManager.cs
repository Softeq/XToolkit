// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Softeq.XToolkit.Common.Interfaces;

namespace Softeq.XToolkit.Permissions.iOS
{
    public class PermissionsManager : IPermissionsManager
    {
        private readonly IPermissionsService _permissionsService;
        private readonly IPermissionsDialogService _permissionsDialogService;
        private readonly IInternalSettings _internalSettings;

        public PermissionsManager(
            IPermissionsService permissionsService,
            IPermissionsDialogService permissionsDialogService,
            IInternalSettings internalSettings)
        {
            _permissionsService = permissionsService;
            _permissionsDialogService = permissionsDialogService;
            _internalSettings = internalSettings;
        }

        public Task<PermissionStatus> CheckWithRequestAsync(Permission permission)
        {
            return permission == Permission.Notifications
                ? NotificationsCheckWithRequestAsync()
                : CommonCheckWithRequestAsync(permission);
        }

        public Task<PermissionStatus> CheckAsync(Permission permission)
        {
            return _permissionsService.CheckPermissionsAsync(permission);
        }

        public void OpenSettings()
        {
            _permissionsService.OpenSettings();
        }

        private readonly string _isPermissionsRequestedKey = $"{nameof(PermissionsManager)}_{nameof(IsPermissionsRequested)}";
        private bool IsPermissionsRequested
        {
            get => _internalSettings.GetValueOrDefault(_isPermissionsRequestedKey, default(bool));
            set => _internalSettings.AddOrUpdateValue(_isPermissionsRequestedKey, value);
        }

        private async Task<PermissionStatus> NotificationsCheckWithRequestAsync()
        {
            if (!IsPermissionsRequested)
            {
                var isConfirmed = await _permissionsDialogService.ConfirmPermissionAsync(Permission.Notifications).ConfigureAwait(false);
                if (!isConfirmed)
                {
                    return PermissionStatus.Denied;
                }

                IsPermissionsRequested = true;
            }

            var permissionStatus = await _permissionsService.RequestPermissionsAsync(Permission.Notifications).ConfigureAwait(false);
            if (permissionStatus != PermissionStatus.Granted)
            {
                permissionStatus = await OpenSettingsWithConfirmationAsync(Permission.Notifications).ConfigureAwait(false);
            }

            return permissionStatus;
        }

        private async Task<PermissionStatus> CommonCheckWithRequestAsync(Permission permission)
        {
            var permissionStatus = await _permissionsService.CheckPermissionsAsync(permission).ConfigureAwait(false);
            if (permissionStatus == PermissionStatus.Granted)
            {
                return permissionStatus;
            }

            if (permissionStatus == PermissionStatus.Denied)
            {
                await OpenSettingsWithConfirmationAsync(permission).ConfigureAwait(false);
            }

            if (permissionStatus == PermissionStatus.Unknown)
            {
                var confirmationResult = await _permissionsDialogService.ConfirmPermissionAsync(permission).ConfigureAwait(false);
                if (confirmationResult)
                {
                    permissionStatus = await _permissionsService.RequestPermissionsAsync(permission).ConfigureAwait(false);
                }
            }

            return permissionStatus;
        }

        private async Task<PermissionStatus> OpenSettingsWithConfirmationAsync(Permission permission)
        {
            var openSettingsConfirmed = await _permissionsDialogService.ComfirmOpenSettingsForPermissionAsync(permission).ConfigureAwait(false);
            if (openSettingsConfirmed)
            {
                OpenSettings();
            }

            return PermissionStatus.Unknown;
        }
    }
}
