// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using Moq;
using Plugin.Connectivity.Abstractions;
using Softeq.XToolkit.Common;
using Softeq.XToolkit.Common.Interfaces;
using Softeq.XToolkit.Connectivity;
using Xunit;

namespace Softeq.XToolkit.Tests.Core.Connectivity
{
	public class ConnectivityTests
	{
		private readonly Mock<ITimerFactory> _timeFactory;
		private readonly Mock<ILogManager> _logManager;
		private readonly Mock<IConnectivity> _connectivity;
		private readonly Mock<ITimer> _timerMock;
		private readonly TestBaseInternetConnectionManagerObject _internetManager;

		public ConnectivityTests()
		{
			_timeFactory = new Mock<ITimerFactory>();
			_logManager = new Mock<ILogManager>();
			_connectivity = new Mock<IConnectivity>();
			_timerMock = new Mock<ITimer>();

			_logManager.Setup(_ => _.GetLogger<BaseInternetConnectionManager>()).Returns(() => new Mock<ILogger>().Object);
			_timerMock.Setup(_ => _.Start()).Callback(() => { });
			_timeFactory.Setup(_ => _.Create(It.IsAny<TaskReference>(), It.IsAny<int>())).Returns(() => _timerMock.Object);

			_internetManager = new TestBaseInternetConnectionManagerObject(_logManager.Object, _connectivity.Object, _timeFactory.Object);
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
			_timerMock.Verify(mock => mock.Start(), Times.Once);
			_timerMock.Verify(mock => mock.Stop(), Times.Once);
		}

		[Fact]
		public void ConnectivityTypeChange_Should_Trigger_NetworkSource_Event()
		{
			//arrange
			var isNetworkSourceInvoked = false;
			var connectionTypes = new List<ConnectionType> { ConnectionType.Bluetooth };

			_internetManager.NetworkSourceChanged += (sender, e) => { isNetworkSourceInvoked = true; };

			//act
			_internetManager.StartTracking();

			_connectivity.Raise(_ => _.ConnectivityTypeChanged += null, new ConnectivityTypeChangedEventArgs
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

			_connectivity.Setup(_ => _.ConnectionTypes).Returns(() => connectionTypes);

			_internetManager.NetworkSourceChanged += (sender, e) => { isNetworkSourceInvoked = true; };

			//act
			_internetManager.StartTracking();

			_connectivity.Raise(_ => _.ConnectivityTypeChanged += null, new ConnectivityTypeChangedEventArgs
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

			_internetManager.NetworkConnectionChanged += (sender, e) => { isNetworkConnectionInvoked = true; };

			//act
			_internetManager.StartTracking();

			_internetManager.IsNetworkAvailable = true;

			_connectivity.Raise(_ => _.ConnectivityChanged += null, new ConnectivityChangedEventArgs());

			//assert
			Assert.True(isNetworkConnectionInvoked);
		}

		[Fact]
		public void ConnectivityChangedEvent_Should_Not_Trigger_NetworkConnectionChangedEvent_If_Connection_Still_Have_Same_Status()
		{
			//arrange
			var isNetworkConnectionInvoked = false;

			_internetManager.NetworkConnectionChanged += (sender, e) => { isNetworkConnectionInvoked = true; };

			//act
			_internetManager.StartTracking();

			_connectivity.Raise(_ => _.ConnectivityChanged += null, new ConnectivityChangedEventArgs());

			//assert
			Assert.False(isNetworkConnectionInvoked);
		}
	}
}