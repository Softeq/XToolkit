// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.XToolkit.RemoteData.HttpClient;

namespace Softeq.XToolkit.Stripe.Requests
{
    internal class GetEphemeralKeyRequest : BaseRestRequest
    {
        private readonly string _apiVersion;
        private readonly string _paymentEndpoint;

        public GetEphemeralKeyRequest(string url, string apiVersion)
        {
            _paymentEndpoint = url;
            _apiVersion = apiVersion;
        }

        public override string EndpointUrl => $"{_paymentEndpoint}/customer/key?version={_apiVersion}";

        public override bool UseOriginalEndpoint => true;
    }
}