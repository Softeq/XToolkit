// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.XToolkit.Connectivity
{
    public class ConnectivityManagerOptions
    {
        private int _dnsResolvingTimeout = 3000;
        private int _pingTimeout = 3500;
        private int _timerInterval = 4000;

        public ConnectivityManagerOptions(string hostName)
        {
            ValidateDomainName(hostName);
            HostName = hostName;
        }

        public string HostName { get; }

        /// <summary>
        ///     The timeout of resolving dns ip adress. Default value is 3000 milliseconds.
        /// </summary>
        public int DnsResolvingTimeout
        {
            get => _dnsResolvingTimeout;
            set
            {
                Validate(value);
                _dnsResolvingTimeout = value;
            }
        }

        /// <summary>
        ///     The timeout for making ping request. Default value is 3500 milliseconds.
        /// </summary>
        public int PingTimeout
        {
            get => _pingTimeout;
            set
            {
                Validate(value);
                _pingTimeout = value;
            }
        }

        /// <summary>
        ///     The delay before the new call iteration. Default value is 4000 milliseconds
        /// </summary>
        public int TimerInterval
        {
            get => _timerInterval;
            set
            {
                Validate(value);
                _timerInterval = value;
            }
        }

        private static void ValidateDomainName(string name)
        {
            if (Uri.CheckHostName(name) == UriHostNameType.Unknown)
            {
                throw new ArgumentException("Hostname is invalid");
            }
        }

        private static void Validate(int newValue)
        {
            if (newValue <= 0)
            {
                throw new ArgumentException("value must be greater than 0");
            }
        }
    }
}