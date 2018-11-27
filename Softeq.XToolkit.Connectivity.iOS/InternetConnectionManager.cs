// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Net;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Softeq.XToolkit.Common.Extensions;
using Softeq.XToolkit.Common.Interfaces;

namespace Softeq.XToolkit.Connectivity.iOS
{
    public class InternetConnectionManager : BaseInternetConnectionManager, IInternetConnectionManager
    {
        private bool _previousResult = true;
        private NetworkStatus _currentConnectionStatus;

        public InternetConnectionManager(ILogManager manager, IConnectivity connectivity, ITimerFactory timerFactory) : base(manager, connectivity, timerFactory) { }

        public override void StartTracking()
        {
            base.StartTracking();

            _currentConnectionStatus = Reachability.InternetConnectionStatus();
            Reachability.ReachabilityChanged += OnReachabilityChanged;
        }

        public override void StopTracking()
        {
            base.StopTracking();

            Reachability.ReachabilityChanged -= OnReachabilityChanged;
        }

        protected override Task<bool> IsNetworkAvailableInternalAsync()
        {
            return TaskDeferral.DoWorkAsync(async () =>
            {
                var isHostReachable = await IsHostReachableAsync().ConfigureAwait(false);

                UpdateIsInternetConnectionAvailable(isHostReachable);

                return IsInternetConnectionAvailable == true;
            });
        }

        private void UpdateIsInternetConnectionAvailable(bool result)
        {
            if (result)
            {
                TryToUpdateInternetResult(true);
            }
            else if (result == _previousResult)
            {
                TryToUpdateInternetResult(false);
            }

            _previousResult = result;
        }

        private Task<bool> IsHostReachableAsync()
        {
            var task = Task.Run(() =>
            {
                try
                {
                    var domain = Dns.GetHostAddresses(Options.HostName);
                    return domain != null && domain.Length > 0;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return false;
                }
            });

            return task.WithTimeout(TimeSpan.FromMilliseconds(Options.DnsResolvingTimeout));
        }

        private async void OnReachabilityChanged(object sender, EventArgs e)
        {
            var status = Reachability.InternetConnectionStatus();

            if (status == _currentConnectionStatus)
            {
                return;
            }

            _currentConnectionStatus = status;

            await IsNetworkAvailableInternalAsync().ConfigureAwait(false);
        }
    }
}