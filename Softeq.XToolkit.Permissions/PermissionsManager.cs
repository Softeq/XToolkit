// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;

namespace Softeq.XToolkit.Permissions
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
        
        public async Task<PermissionStatus> CheckWithRequestAsync(Permission permission)
        {
            var permissionStatus = PermissionStatus.Unknown;

            permissionStatus = await _permissionsService.CheckPermissionsAsync(permission).ConfigureAwait(false);

            if (permissionStatus != PermissionStatus.Granted)
            {
                var confirmationResult = await _permissionsDialogService.ComfirmPermissionAsync(permission).ConfigureAwait(false);
                if (confirmationResult)
                {
                    permissionStatus = await _permissionsService.RequestPermissionsAsync(permission);
                }
            }

            return permissionStatus;
        }

        public Task<PermissionStatus> CheckAsync(Permission permission)
        {
            return _permissionsService.CheckPermissionsAsync(permission);
        }

        public void OpenSettings()
        {
            _permissionsService.OpenSettings();
        }
    }
}
