// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;

namespace Softeq.XToolkit.Permissions
{
    public interface IPermissionsDialogService
    {
        Task<bool> ComfirmPermissionAsync(Permission permission);
    }
}
