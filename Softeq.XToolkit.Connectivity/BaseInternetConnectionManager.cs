// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Connectivity.Abstractions;
using Softeq.XToolkit.Common;
using Softeq.XToolkit.Common.Extensions;
using Softeq.XToolkit.Common.Interfaces;

namespace Softeq.XToolkit.Connectivity
{
    public abstract class BaseInternetConnectionManager
    {
        protected ConnectivityManagerOptions Options { get; private set; }
        protected TaskDeferral<bool> TaskDeferral;

        protected IList<ConnectionType> ConnectionTypes;
        protected ILogger Logger { get; }
        protected ITimer Timer { get; private set; }

        private readonly IConnectivity _connectivityPlugin;
        private readonly ITimerFactory _timerFactory;

        private bool _isStarted;

        protected bool? IsInternetConnectionAvailable { get; private set; }

        public event EventHandler<NetworkConnectionEventArgs> NetworkConnectionChanged;
        public event EventHandler NetworkSourceChanged;

        protected BaseInternetConnectionManager(ILogManager manager, IConnectivity connectivity, ITimerFactory timerFactory)
        {
            _connectivityPlugin = connectivity;
            _timerFactory = timerFactory;

            TaskDeferral = new TaskDeferral<bool>();

            Logger = manager.GetLogger<BaseInternetConnectionManager>();
            SetOptions(new ConnectivityManagerOptions("www.google.com"));
        }

        public void Restart()
        {
            StopTracking();
            StartTracking();
        }

        public Task<bool> IsNetworkAvailableAsync()
        {
            return IsNetworkAvailableInternalAsync();
        }

        public void SetOptions(ConnectivityManagerOptions options)
        {
            Options = options ?? throw new InvalidOperationException("Please set valid options.");

            Timer = _timerFactory.Create(new TaskReference(IsNetworkAvailableInternalAsync), Options.TimerInterval);

            TaskDeferral = new TaskDeferral<bool>();
        }

        public virtual void StartTracking()
        {
            if (_isStarted)
            {
                throw new InvalidOperationException("Tracking already started. Please stop it first.");
            }

            _isStarted = true;

            SubscribeConnectivityTypeChanged();

            ConnectionTypes = _connectivityPlugin.ConnectionTypes.ToArray();

            IsNetworkAvailableInternalAsync().SafeTaskWrapper(Logger);

            Timer.Start();
        }

        public virtual void StopTracking()
        {
            UnsubcribeConnectivityTypeChanged();

            _isStarted = false;

            Timer.Stop();
        }

        protected abstract Task<bool> IsNetworkAvailableInternalAsync();

        protected void HandleConnectivityTypeChanged(object sender, ConnectivityTypeChangedEventArgs e)
        {
            var types = e.ConnectionTypes.ToArray();

            bool IsNetworkStatusChanged(IList<ConnectionType> connectionTypes)
            {
                return ConnectionTypes.Except(connectionTypes).Any() ||
                       connectionTypes.Except(ConnectionTypes).Any();
            }

            if (!IsNetworkStatusChanged(types))
            {
                return;
            }

            ConnectionTypes = types;

            InvokeNetworkSourceChangedEvent();
        }

        protected void TryToUpdateInternetResult(bool newValue)
        {
            if (IsInternetConnectionAvailable != newValue)
            {
                IsInternetConnectionAvailable = newValue;
                NetworkConnectionChanged?.Invoke(this, new NetworkConnectionEventArgs(IsInternetConnectionAvailable == true, ConnectionTypes));
            }
        }

        protected virtual void SubscribeConnectivityTypeChanged()
        {
            _connectivityPlugin.ConnectivityChanged += HandleNetworkChanged;
            _connectivityPlugin.ConnectivityTypeChanged += HandleConnectivityTypeChanged;
        }

        protected virtual void UnsubcribeConnectivityTypeChanged()
        {
            _connectivityPlugin.ConnectivityChanged -= HandleNetworkChanged;
            _connectivityPlugin.ConnectivityTypeChanged -= HandleConnectivityTypeChanged;
        }

        protected void InvokeNetworkSourceChangedEvent()
        {
            if (_isStarted)
            {
                NetworkSourceChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        protected void InvokeNetworkConnectionChangedEvent()
        {
            if (_isStarted)
            {
                NetworkConnectionChanged?.Invoke(this, new NetworkConnectionEventArgs(IsInternetConnectionAvailable == true, ConnectionTypes));
            }
        }

        private async void HandleNetworkChanged(object sender, EventArgs args)
        {
            await IsNetworkAvailableInternalAsync().ConfigureAwait(false);
        }
    }
}
