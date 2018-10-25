// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;

namespace Softeq.XToolkit.Permissions.Droid
{
    public class PermissionsManager : IPermissionsManager
    {
        private readonly IPermissionsService _permissionsService;
        private readonly IPermissionsDialogService _permissionsDialogService;

        public PermissionsManager(
            IPermissionsService permissionsService,
            IPermissionsDialogService permissionsDialogService)
        {
            _permissionsService = permissionsService;
            _permissionsDialogService = permissionsDialogService;
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

            var confirmationResult = await _permissionsDialogService.ComfirmPermissionAsync(permission).ConfigureAwait(false);
            if (confirmationResult)
            {
                permissionStatus = await _permissionsService.RequestPermissionsAsync(permission).ConfigureAwait(false);
            }

            return permissionStatus;
        }
    }
}
