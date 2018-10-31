// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.XToolkit.Common.Interfaces;
using Softeq.XToolkit.RemoteData.HttpClient;
using Softeq.XToolkit.Stripe.Dtos;

namespace Softeq.XToolkit.Stripe.Requests
{
    internal class PostCreateChargeRequest : BasePostRestRequest<ChargeDto>
    {
        private readonly StripeConfig _stripeConfig;

        public PostCreateChargeRequest(IJsonSerializer jsonSerializer, ChargeDto dto, StripeConfig stripeConfig) : base(
            jsonSerializer, dto)
        {
            _stripeConfig = stripeConfig;
        }

        public override string EndpointUrl => $"{_stripeConfig.Endpoint}/charge";

        public override bool UseOriginalEndpoint => true;
    }
}