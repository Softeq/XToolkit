using System.Threading.Tasks;
using Plugin.Connectivity.Abstractions;
using Softeq.XToolkit.Common;
using Softeq.XToolkit.Common.Interfaces;
using Softeq.XToolkit.Connectivity;

namespace Softeq.XToolkit.Tests.Core.Connectivity
{
	public class TestBaseInternetConnectionManagerObject : BaseInternetConnectionManager
	{
		public bool IsNetworkAvailable { get; set; }

		public TestBaseInternetConnectionManagerObject(ILogManager manager, IConnectivity connectivity, ITimerFactory timerFactory) : base(manager, connectivity, timerFactory)
		{
		}

		protected override Task<bool> IsNetworkAvailableInternalAsync()
		{
			TryToUpdateInternetResult(IsNetworkAvailable);
			return Task.FromResult(IsNetworkAvailable);
		}
	}
}
