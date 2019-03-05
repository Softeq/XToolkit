﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Softeq.XToolkit.Common.Interfaces;

namespace Softeq.XToolkit.Permissions.Droid
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
            return CommonCheckWithRequestAsync(permission);
        }

        public Task<PermissionStatus> CheckAsync(Permission permission)
        {
            return _permissionsService.CheckPermissionsAsync(permission);
        }

        public void OpenSettings()
        {
            _permissionsService.OpenSettings();
        }

        private async Task<PermissionStatus> CommonCheckWithRequestAsync(Permission permission)
        {
            var permissionStatus = await _permissionsService.CheckPermissionsAsync(permission).ConfigureAwait(false);
            if (permissionStatus == PermissionStatus.Granted)
            {
                return permissionStatus;
            }

            var key = $"PermissionsManager_PermissionRequested_{permission}";
            var isRequested = _internalSettings.GetValueOrDefault(key, false);

            if (permissionStatus == PermissionStatus.Denied && isRequested)
            {
                await OpenSettingsWithConfirmationAsync(permission).ConfigureAwait(false);
                return PermissionStatus.Denied;
            }

            var confirmationResult = await _permissionsDialogService.ConfirmPermissionAsync(permission).ConfigureAwait(false);
            if (confirmationResult)
            {
                if (!isRequested)
                {
                    _internalSettings.AddOrUpdateValue(key, true);
                }
                permissionStatus = await _permissionsService.RequestPermissionsAsync(permission).ConfigureAwait(false);
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
