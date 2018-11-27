// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Plugin.CurrentActivity;
using Softeq.XToolkit.Common.Droid.Extensions;
using Softeq.XToolkit.Common.Extensions;
using Softeq.XToolkit.Common.Interfaces;

namespace Softeq.XToolkit.Connectivity.Droid
{
	public class InternetConnectionManager : BaseInternetConnectionManager, IInternetConnectionManager
	{
		public InternetConnectionManager(ILogManager logManager, IConnectivity connectivity, ITimerFactory timerFactory) : base(logManager, connectivity, timerFactory) { }

		protected override Task<bool> IsNetworkAvailableInternalAsync()
		{
			return TaskDeferral.DoWorkAsync(async () =>
			{
				var state = CrossConnectivity.Current.IsConnected && await CheckConnectionAsync().ConfigureAwait(false);

				TryToUpdateInternetResult(state);

                return IsInternetConnectionAvailable == true;
			});
		}

		private async Task<bool> CheckConnectionAsync()
		{
			var ipAddress = await ResolveDnsName().ConfigureAwait(false);
			if (ipAddress == null)
			{
				return false;
			}
			if (CrossCurrentActivity.Current.Activity.IsInPowerSavingMode())
			{
				return true;
			}

			return await SendPingAsync(ipAddress).ConfigureAwait(false);
		}

		private Task<IPAddress> ResolveDnsName()
		{
			var task = Task.Run(() =>
			{
				try
				{
					var hostEntry = Dns.GetHostEntry(Options.HostName);
					return hostEntry.AddressList.FirstOrDefault();
				}
				catch (Exception e)
				{
					Logger.Error(e);
					return null;
				}
			});

			return task.WithTimeout(TimeSpan.FromMilliseconds(Options.DnsResolvingTimeout));
		}

		private Task<bool> SendPingAsync(IPAddress ipAddress)
		{
			return Task.Run(async () =>
			{
				try
				{
					using (var serverPing = new Ping())
					{
						var pingResult = await serverPing.SendPingAsync(ipAddress, Options.PingTimeout);
						return pingResult.Status == IPStatus.Success;
					}
				}
				catch (Exception e)
				{
					Logger.Error(e);
					return false;
				}
			});
		}
	}
}