// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.XToolkit.Common.Interfaces;
using Softeq.XToolkit.RemoteData;
using Softeq.XToolkit.RemoteData.HttpClient;
using Softeq.XToolkit.Stripe.Dtos;
using Softeq.XToolkit.Stripe.Requests;

namespace Softeq.XToolkit.Stripe
{
    public class StripeRemoteService
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ILogger _logger;
        private readonly IRestHttpClient _restHttpClient;
        private readonly StripeConfig _stripeConfig;

        public StripeRemoteService(
            StripeConfig stripeConfig,
            IRestHttpClient restHttpClient,
            ILogManager logManager,
            IJsonSerializer jsonSerializer)
        {
            _stripeConfig = stripeConfig;
            _restHttpClient = restHttpClient;
            _jsonSerializer = jsonSerializer;
            _logger = logManager.GetLogger<StripeRemoteService>();
        }

        public async Task<string> GetEphemeralKeyAsync(string apiVersion)
        {
            var request = new GetEphemeralKeyRequest(_stripeConfig.Endpoint, apiVersion);
            try
            {
                var result = await _restHttpClient.SendAndGetResponseAsync(request);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return default(string);
        }

        public Task<bool> RequestPaymentAsync(int amount, string sourceId, string currency, string description)
        {
            var dto = new ChargeDto
            {
                Amount = amount,
                CardSourceId = sourceId,
                Currency = currency,
                Description = description
            };
            var request = new PostCreateChargeRequest(_jsonSerializer, dto, _stripeConfig);
            return _restHttpClient.TrySendAsync(request, _logger);
        }

        public Task<bool> AttachSourceToCustomer(string id)
        {
            var request = new PostAttachSourceToCustomerRequest(id, _stripeConfig);
            return _restHttpClient.TrySendAsync(request, _logger);
        }

        public Task<bool> DetachSourceFromCustomer(string id)
        {
            var request = new DeleteSourceFromCustomerRequest(id, _stripeConfig);
            return _restHttpClient.TrySendAsync(request, _logger);
        }
    }
}