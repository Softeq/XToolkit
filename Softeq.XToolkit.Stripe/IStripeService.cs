// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.XToolkit.Stripe
{
    public interface IStripeService
    {
        void Initialize(StripeConfig stripeConfig);
        void SelectPaymentMethod();
        void RequestPayment(int amount, string currency);
    }
}