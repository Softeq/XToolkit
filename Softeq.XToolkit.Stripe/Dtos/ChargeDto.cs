// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.XToolkit.Stripe.Dtos
{
    internal class ChargeDto
    {
        public int Amount { get; set; }

        public string Currency { get; set; }

        public string Description { get; set; }

        public string CardSourceId { get; set; }
    }
}