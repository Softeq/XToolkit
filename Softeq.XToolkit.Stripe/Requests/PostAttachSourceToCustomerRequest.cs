// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Net.Http;
using System.Text;
using Softeq.XToolkit.RemoteData;
using Softeq.XToolkit.RemoteData.HttpClient;

namespace Softeq.XToolkit.Stripe.Requests
{
    internal class PostAttachSourceToCustomerRequest : BaseRestRequest
    {
        private readonly string _dto;
        private readonly StripeConfig _stripeConfig;

        public PostAttachSourceToCustomerRequest(string dto, StripeConfig stripeConfig)
        {
            _stripeConfig = stripeConfig;
            _dto = dto;
        }

        public override string EndpointUrl => $"{_stripeConfig.Endpoint}/card/{_dto}";

        public override bool UseOriginalEndpoint => true;

        public override HttpMethod Method => HttpMethod.Post;

        public override HttpContent GetContent()
        {
            return new StringContent(string.Empty, Encoding.UTF8, HttpConsts.ApplicationJsonHeaderValue);
        }
    }
}