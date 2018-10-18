// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using Plugin.Connectivity.Abstractions;

namespace Softeq.XToolkit.Connectivity
{
	public class NetworkConnectionEventArgs
	{
		public bool IsNetworkAvailable { get; }
		public IEnumerable<ConnectionType> ConnectionTypes { get; }

		public NetworkConnectionEventArgs(bool isNetworkAvailable, IEnumerable<ConnectionType> connectionTypes)
		{
			IsNetworkAvailable = isNetworkAvailable;
			ConnectionTypes = connectionTypes;
		}
	}
}