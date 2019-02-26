// Developed by Softeq Development Corporation
// http://www.softeq.com

using CoreBluetooth;

namespace Softeq.XToolkit.Permissions.iOS
{
    public partial class PermissionsService
    {
        private class CustomCBCentralManagerDelegate : CBCentralManagerDelegate
        {
            public override void UpdatedState(CBCentralManager central)
            {
            }
        }
    }
}