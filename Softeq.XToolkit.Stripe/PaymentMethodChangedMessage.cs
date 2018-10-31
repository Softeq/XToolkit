// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.IO;

namespace Softeq.XToolkit.Stripe
{
    public class PaymentMethodChangedMessage
    {
        public string Label { get; set; }
        public Stream Image { get; set; }
        public int ResourceId { get; set; }

        public bool IsEmpty => string.IsNullOrEmpty(Label) && Image == null && ResourceId == 0;
    }
}