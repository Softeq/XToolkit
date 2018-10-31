// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Threading.Tasks;
using Com.Stripe.Android;
using Com.Stripe.Android.Model;
using Java.Lang;
using Plugin.CurrentActivity;

namespace Softeq.XToolkit.Stripe.Droid.Services
{
    internal class StripeManager
    {
        private readonly StripeConfig _config;
        private readonly StripeRemoteService _stripeRemoteService;

        private StripeManager(StripeRemoteService stripeRemoteService, StripeConfig config)
        {
            _stripeRemoteService = stripeRemoteService;
            _config = config;
        }

        public static StripeManager Instance { get; private set; }

        public static void Initialize(
            StripeRemoteService stripeRemoteService,
            StripeConfig config)
        {
            Instance = new StripeManager(stripeRemoteService, config);
        }

        public Task<IList<CustomerSource>> GetUserCardsAsync()
        {
            var tcs = new TaskCompletionSource<IList<CustomerSource>>();
            CustomerSession.Instance.UpdateCurrentCustomer(new CustomerRetrievalListener(tcs));
            return tcs.Task;
        }

        public Task<bool> DetachSourceFromCustomerAsync(string id)
        {
            return _stripeRemoteService.DetachSourceFromCustomer(id);
        }

        public Task<bool> AttachSourceToCustomerAsync(string id)
        {
            return _stripeRemoteService.AttachSourceToCustomer(id);
        }

        public Task<string> CreateTokenAsync(Card card)
        {
            var tcs = new TaskCompletionSource<string>();
            var stripe = new Com.Stripe.Android.Stripe(CrossCurrentActivity.Current.Activity, _config.ApiKey);
            stripe.CreateToken(card, new TokenCallback(tcs));
            return tcs.Task;
        }

        private class TokenCallback : Object, ITokenCallback
        {
            private readonly TaskCompletionSource<string> _tcs;

            public TokenCallback(TaskCompletionSource<string> tcs)
            {
                _tcs = tcs;
            }

            public void OnError(Exception p0)
            {
                _tcs.TrySetResult(null);
            }

            public void OnSuccess(Token token)
            {
                _tcs.TrySetResult(token.Id);
            }
        }

        private class CustomerRetrievalListener : Object, CustomerSession.ICustomerRetrievalListener
        {
            private readonly TaskCompletionSource<IList<CustomerSource>> _tcs;

            public CustomerRetrievalListener(TaskCompletionSource<IList<CustomerSource>> tcs)
            {
                _tcs = tcs;
            }

            public void OnCustomerError(int p0, string p1)
            {
                _tcs.TrySetResult(null);
            }

            public void OnCustomerRetrieved(Customer customer)
            {
                _tcs.TrySetResult(customer.Sources);
            }
        }
    }
}