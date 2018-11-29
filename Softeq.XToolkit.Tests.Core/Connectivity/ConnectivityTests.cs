// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using Plugin.Connectivity.Abstractions;
using Softeq.XToolkit.Common;
using Softeq.XToolkit.Common.Interfaces;
using Softeq.XToolkit.Connectivity;
using Xunit;

namespace Softeq.XToolkit.Tests.Core.Connectivity
{
	public class ConnectivityTests
	{
		private readonly ITimerFactory _timeFactory;
		private readonly ILogManager _logManager;
		private readonly IConnectivity _connectivity;
		private readonly ITimer _timerMock;
		private readonly TestBaseInternetConnectionManagerObject _internetManager;

		public ConnectivityTests()
		{
			_timeFactory = Substitute.For<ITimerFactory>();
			_logManager = Substitute.For<ILogManager>();
			_connectivity = Substitute.For<IConnectivity>();
			_timerMock = Substitute.For<ITimer>();

			_logManager.GetLogger<BaseInternetConnectionManager>().Returns(info => Substitute.For<ILogger>());
			_timeFactory.Create(Arg.Any<TaskReference>(), Arg.Any<int>()).Returns(info => _timerMock);
			_internetManager = new TestBaseInternetConnectionManagerObject(_logManager, _connectivity, _timeFactory);
		}

		[Fact]
		public void SetOptions_Throw_Exception_When_Trying_To_Set_Null_Value()
		{
			//arrange
			//act
			Exception ex = Assert.Throws<InvalidOperationException>(() => _internetManager.SetOptions(null));

			//assert
			Assert.Equal("Please set valid options.", ex.Message);
		}

		[Fact]
		public void Double_Start_Throw_Exception()
		{
			//arrange
			//act
			Exception ex = Assert.Throws<InvalidOperationException>(() =>
			{
				_internetManager.StartTracking();
				_internetManager.StartTracking();
			});

			//assert
			Assert.Equal("Tracking already started. Please stop it first.", ex.Message);
		}

		[Fact]
		public void Starting_Tracking_Invoked_Immidiately()
		{
			//arrange
			_internetManager.IsNetworkAvailable = true;

			var isInvoked = false;

			_internetManager.NetworkConnectionChanged += (sender, e) =>
			{
				isInvoked = true;
			};

			//act
			_internetManager.StartTracking();

			//assert
			Assert.True(isInvoked);
		}

		[Fact]
		public void Start_Stop_Tracking_Invoked_Start_Stop_Timer_Once()
		{
			//arrange
			//act
			_internetManager.StartTracking();
			_internetManager.StopTracking();

			//assert

			var calledMethodNames = _timerMock.ReceivedCalls()
				.Select(x => x.GetMethodInfo().Name)
				.ToArray();

			var expetcedCallNames = new[] { "Start", "Stop" };

			Assert.True(calledMethodNames.All(x => expetcedCallNames.Contains(x)));
		}

		[Fact]
		public void ConnectivityTypeChange_Should_Trigger_NetworkSource_Event()
		{
			//arrange
			var isNetworkSourceInvoked = false;
			var connectionTypes = new List<ConnectionType> { ConnectionType.Bluetooth };

			_internetManager.NetworkSourceChanged += (sender, e) =>
			{
				isNetworkSourceInvoked = true;
			};

			//act
			_internetManager.StartTracking();

			_connectivity.ConnectivityTypeChanged += Raise.Event<ConnectivityTypeChangedEventHandler>(
				null, new ConnectivityTypeChangedEventArgs
				{
					ConnectionTypes = connectionTypes
				});

			//assert
			Assert.True(isNetworkSourceInvoked);
		}

		[Fact]
		public void ConnectivityTypeChange_Should_Not_Trigger_NetworkSource_Event()
		{
			//arrange
			var isNetworkSourceInvoked = false;
			var connectionTypes = new List<ConnectionType> { ConnectionType.Bluetooth };

			_connectivity.ConnectionTypes.Returns(info => connectionTypes);

			_internetManager.NetworkSourceChanged += (sender, e) => { isNetworkSourceInvoked = true; };

			//act
			_internetManager.StartTracking();

			_connectivity.ConnectivityTypeChanged += Raise.Event<ConnectivityTypeChangedEventHandler>(null, new ConnectivityTypeChangedEventArgs
			{
				ConnectionTypes = connectionTypes
			});

			//assert
			Assert.False(isNetworkSourceInvoked);
		}

		[Fact]
		public void ConnectivityChangedEvent_Should_Always_Trigger_Check_Connection()
		{
			//arrange
			var isNetworkConnectionInvoked = false;

			_internetManager.NetworkConnectionChanged += (sender, e) =>
			{
				isNetworkConnectionInvoked = true;
			};

			//act
			_internetManager.StartTracking();

			_internetManager.IsNetworkAvailable = true;

			_connectivity.ConnectivityChanged += Raise.Event<ConnectivityChangedEventHandler>(null, new ConnectivityChangedEventArgs());

			//assert
			Assert.True(isNetworkConnectionInvoked);
		}

		[Fact]
		public void ConnectivityChangedEvent_Should_Not_Trigger_NetworkConnectionChangedEvent_If_Connection_Still_Have_Same_Status()
		{
			//arrange
			var times = 0;

			_internetManager.NetworkConnectionChanged += (sender, e) =>
			{
				times++;
			};

			//act
			_internetManager.StartTracking();

			_connectivity.ConnectivityChanged += Raise.Event<ConnectivityChangedEventHandler>(null, new ConnectivityChangedEventArgs());

			//assert
			Assert.True(times == 1);
		}
	}
}