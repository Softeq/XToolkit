// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Foundation;
using Softeq.XToolkit.Common.Interfaces;
using Softeq.XToolkit.RemoteData.HttpClient;
using StripeSdk;
using UIKit;

namespace Softeq.XToolkit.Stripe.iOS
{
    public class StripeService : IStripeService
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly Func<UIViewController> _getUiViewControllerFunc;
        private readonly ILogManager _logManager;
        private readonly IMessageHub _messageHub;
        private readonly IRestHttpClient _restHttpClient;

        private STPPaymentContext _paymentContext;

        public StripeService(
            IRestHttpClient restHttpClient,
            ILogManager logManager,
            IMessageHub messageHub,
            IJsonSerializer jsonSerializer, Func<UIViewController> getUiViewControllerFunc)
        {
            _restHttpClient = restHttpClient;
            _logManager = logManager;
            _messageHub = messageHub;
            _jsonSerializer = jsonSerializer;
            _getUiViewControllerFunc = getUiViewControllerFunc;
        }

        public void Initialize(StripeConfig stripeConfig)
        {
            if (_paymentContext != null)
            {
                return;
            }

            if (STPPaymentConfiguration.SharedConfiguration().PublishableKey == null)
            {
                STPPaymentConfiguration.SharedConfiguration().PublishableKey = stripeConfig.ApiKey;
            }

            var stripeRemoteService =
                new StripeRemoteService(stripeConfig, _restHttpClient, _logManager, _jsonSerializer);
            var provider = new StripeEphemeralKeyProvider(stripeRemoteService, _logManager);
            var customerContext = new CustomerContext(provider, stripeRemoteService);

            _paymentContext = new STPPaymentContext(
                customerContext,
                STPPaymentConfiguration.SharedConfiguration(),
                STPTheme.DefaultTheme)
            {
                HostViewController = _getUiViewControllerFunc(),
                Delegate = new PaymentDelegate(_messageHub, stripeRemoteService)
            };
        }

        public void SelectPaymentMethod()
        {
            _paymentContext.PresentPaymentMethodsViewController();
        }

        public void RequestPayment(int amount, string currency)
        {
            _paymentContext.PaymentAmount = amount;
            _paymentContext.PaymentCurrency = currency;
            _paymentContext.RequestPayment();
        }

        private class PaymentDelegate : STPPaymentContextDelegate
        {
            private readonly IMessageHub _messageHub;
            private readonly StripeRemoteService _stripeRemoteService;

            public PaymentDelegate(IMessageHub messageHub, StripeRemoteService stripeRemoteService)
            {
                _messageHub = messageHub;
                _stripeRemoteService = stripeRemoteService;
            }

            public override void PaymentContext(STPPaymentContext paymentContext, NSError error)
            {
            }

            public override void PaymentContextDidChange(STPPaymentContext paymentContext)
            {
                var paymentMethod = paymentContext.SelectedPaymentMethod;
                var message = paymentMethod == null
                    ? new PaymentMethodChangedMessage()
                    : new PaymentMethodChangedMessage
                    {
                        Label = paymentMethod.Label,
                        Image = paymentMethod.Image?.AsPNG().AsStream()
                    };
                _messageHub.SendMessage(message);
            }

            public override async void PaymentContext(STPPaymentContext paymentContext, STPPaymentResult paymentResult,
                STPErrorBlock completion)
            {
                var result = await _stripeRemoteService.RequestPaymentAsync((int)paymentContext.PaymentAmount,
                    paymentResult.Source.StripeID, paymentContext.PaymentCurrency, "test_ios");
                completion(result ? null : new NSError());
            }

            public override void PaymentContext(STPPaymentContext paymentContext, STPPaymentStatus status,
                NSError error)
            {
            }
        }

        private class StripeEphemeralKeyProvider : NSObject, ISTPEphemeralKeyProvider
        {
            private readonly ILogger _logger;
            private readonly StripeRemoteService _stripeRemoteService;

            public StripeEphemeralKeyProvider(
                StripeRemoteService stripeRemoteService,
                ILogManager logManager)
            {
                _stripeRemoteService = stripeRemoteService;
                _logger = logManager.GetLogger<StripeEphemeralKeyProvider>();
            }

            public async void SreateCustomerKeyWithAPIVersion(string apiVersion,
                STPJSONResponseCompletionBlock completion)
            {
                var result = await _stripeRemoteService.GetEphemeralKeyAsync(apiVersion);
                if (result == null)
                {
                    completion(null, new NSError());
                    return;
                }

                var nsError = default(NSError);
                var nsDictionary = default(NSDictionary);

                try
                {
                    var nsData = NSData.FromString(result);
                    nsDictionary = (NSDictionary)NSJsonSerialization.Deserialize(
                        nsData,
                        NSJsonReadingOptions.MutableContainers,
                        out nsError);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }

                completion(nsDictionary, nsError);
            }
        }

        private class CustomerContext : STPCustomerContext
        {
            private readonly StripeRemoteService _stripeRemoteService;

            public CustomerContext(
                StripeEphemeralKeyProvider keyProvider,
                StripeRemoteService stripeRemoteService) : base(keyProvider)
            {
                _stripeRemoteService = stripeRemoteService;
            }

            public override async void AttachSourceToCustomer(ISTPSourceProtocol source, STPErrorBlock completion)
            {
                var result = await _stripeRemoteService.AttachSourceToCustomer(source.StripeID);
                completion(result ? null : new NSError());
            }

            public override async void DetachSourceFromCustomer(ISTPSourceProtocol source, STPErrorBlock completion)
            {
                var result = await _stripeRemoteService.DetachSourceFromCustomer(source.StripeID);
                if (completion != null)
                {
                    completion(result ? null : new NSError());
                }
            }
        }
    }
}