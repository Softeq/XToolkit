// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Com.Stripe.Android;
using Com.Stripe.Android.Model;
using Java.Lang;
using Plugin.CurrentActivity;
using Softeq.XToolkit.Common.Interfaces;
using Softeq.XToolkit.Common.Command;
using Softeq.XToolkit.RemoteData.HttpClient;
using Softeq.XToolkit.Stripe.Droid.Services;

namespace Softeq.XToolkit.Stripe.Droid
{
    public interface IStripeHolder
    {
        void OnSaveInstanceState(Bundle outState);
        void OnDestroy();
        void OnCreate(Bundle savedInstanceState, StripeConfig stripeConfig);
        void OnActivityResult(int requestCode, Result resultCode, Intent data);
    }

    public class StripeService : IStripeService, IStripeHolder
    {
        private const int RequestCodeSelectCard = 3003;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ILogManager _logManager;
        private readonly IMessageHub _messageHub;
        private readonly IRestHttpClient _restHttpClient;
        private PaymentSession _paymentSession;
        private StripeConfig _stripeConfig;

        public StripeService(
            IRestHttpClient restHttpClient,
            ILogManager logManager,
            IMessageHub messageHub,
            IJsonSerializer jsonSerializer)
        {
            _restHttpClient = restHttpClient;
            _logManager = logManager;
            _messageHub = messageHub;
            _jsonSerializer = jsonSerializer;
        }

        public void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            _paymentSession.HandlePaymentData(requestCode, (int) resultCode, data);
            if (resultCode == Result.Ok && requestCode == RequestCodeSelectCard)
            {
                RefreshCustomerInfo();
            }
        }

        public void OnCreate(Bundle savedInstanceState, StripeConfig stripeConfig)
        {
            _stripeConfig = stripeConfig;

            PaymentConfiguration.Init(_stripeConfig.ApiKey);

            var stripeRemoteService =
                new StripeRemoteService(_stripeConfig, _restHttpClient, _logManager, _jsonSerializer);

            CustomerSession.InitCustomerSession(new StripeEphemeralKeyProvider(stripeRemoteService));

            _paymentSession = new PaymentSession(CrossCurrentActivity.Current.Activity);
            var config = new PaymentSessionConfig.Builder()
                .SetShippingInfoRequired(false)
                .SetShippingMethodsRequired(false)
                .Build();
            _paymentSession.Init(new PaymentSessionListener(), config, savedInstanceState);

            StripeManager.Initialize(stripeRemoteService, _stripeConfig);
        }

        public void OnDestroy()
        {
            _paymentSession.OnDestroy();
        }

        public void OnSaveInstanceState(Bundle outState)
        {
            _paymentSession.SavePaymentSessionInstanceState(outState);
        }

        public void Initialize(StripeConfig stripeConfig)
        {
        }

        public void RequestPayment(int amount, string currency)
        {
            var stripeRemoteService =
                new StripeRemoteService(_stripeConfig, _restHttpClient, _logManager, _jsonSerializer);
            _paymentSession.CompletePayment(new PaymentCompletionProvider(stripeRemoteService, amount, currency));
        }

        public void SelectPaymentMethod()
        {
            CrossCurrentActivity.Current.Activity.StartActivityForResult(typeof(CardsActivity),
                RequestCodeSelectCard);
        }

        private void RefreshCustomerInfo()
        {
            CustomerSession.Instance.RetrieveCurrentCustomer(
                new CustomerListener(new RelayCommand<Customer>(PublishMessage)));
        }

        private void PublishMessage(Customer customer)
        {
            if (customer.DefaultSource == null)
            {
                _messageHub.SendMessage(new PaymentMethodChangedMessage());
                return;
            }

            var source = customer.Sources
                .FirstOrDefault(x => x.Id == customer.DefaultSource);

            var card = source?.AsCard();
            if (card == null)
            {
                return;
            }

            _messageHub.SendMessage(new PaymentMethodChangedMessage
            {
                Label = $"{card.Brand} - ...{card.Last4}",
                ResourceId = (int) Card.BrandResourceMap[card.Brand]
            });
        }

        #region private classes

        private class CustomerListener : Object, CustomerSession.ICustomerRetrievalListener
        {
            private readonly RelayCommand<Customer> _relayCommand;

            public CustomerListener(RelayCommand<Customer> relayCommand)
            {
                _relayCommand = relayCommand;
            }

            public void OnCustomerError(int p0, string p1)
            {
            }

            public void OnCustomerRetrieved(Customer customer)
            {
                _relayCommand.Execute(customer);
            }
        }

        private class StripeEphemeralKeyProvider : Object, IEphemeralKeyProvider
        {
            private readonly StripeRemoteService _stripeRemoteService;

            public StripeEphemeralKeyProvider(StripeRemoteService stripeRemoteService)
            {
                _stripeRemoteService = stripeRemoteService;
            }

            public async void CreateEphemeralKey(string apiVersion, IEphemeralKeyUpdateListener keyUpdateListener)
            {
                var result = await _stripeRemoteService.GetEphemeralKeyAsync(apiVersion);
                if (!string.IsNullOrEmpty(result))
                {
                    keyUpdateListener.OnKeyUpdate(result);
                }
            }
        }

        private class PaymentSessionListener : Object, PaymentSession.IPaymentSessionListener
        {
            public void OnCommunicatingStateChanged(bool p0)
            {
            }

            public void OnError(int p0, string p1)
            {
            }

            public void OnPaymentSessionDataChanged(PaymentSessionData p0)
            {
            }
        }

        private class PaymentCompletionProvider : Object, IPaymentCompletionProvider
        {
            private readonly int _amount;
            private readonly string _currency;
            private readonly StripeRemoteService _stripeRemoteService;

            public PaymentCompletionProvider(StripeRemoteService stripeRemoteService, int amount, string currency)
            {
                _stripeRemoteService = stripeRemoteService;
                _amount = amount;
                _currency = currency;
            }

            public async void CompletePayment(PaymentSessionData paymentSessionData,
                IPaymentResultListener paymentResultListener)
            {
                await _stripeRemoteService.RequestPaymentAsync(_amount, paymentSessionData.SelectedPaymentMethodId,
                    _currency, "test_droid");
                paymentResultListener.OnPaymentResult("Success");
            }
        }

        #endregion
    }
}