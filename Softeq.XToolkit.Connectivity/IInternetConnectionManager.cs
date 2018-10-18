// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;

namespace Softeq.XToolkit.Connectivity
{
	public interface IInternetConnectionManager
    {
        event EventHandler<NetworkConnectionEventArgs> NetworkConnectionChanged;
        event EventHandler NetworkSourceChanged;

        Task<bool> IsNetworkAvailableAsync();

        void SetOptions(ConnectivityManagerOptions options);

        void StartTracking();
        void StopTracking();
    }
}