// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Com.Stripe.Android.View;
using Softeq.XToolkit.Stripe.Droid.Services;

namespace Softeq.XToolkit.Stripe.Droid
{
    [Activity]
    internal class AddCardActivity : AppCompatActivity
    {
        public const string NewCardExtra = "NewCardExtraKey";

        private CardMultilineWidget _cardMultilineWidget;
        private ImageButton _okButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.add_card_page);

            _cardMultilineWidget = new CardMultilineWidget(this);

            var layout = FindViewById<RelativeLayout>(Resource.Id.add_card_container);
            var layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent);
            _cardMultilineWidget.LayoutParameters = layoutParams;
            layout.AddView(_cardMultilineWidget);

            _okButton = FindViewById<ImageButton>(Resource.Id.add_card_ok_button);
            _okButton.Click += OnOkButtonClick;
        }

        private async void OnOkButtonClick(object sender, EventArgs e)
        {
            var card = _cardMultilineWidget.Card;
            var result = await StripeManager.Instance.CreateTokenAsync(card).ConfigureAwait(false);
            RunOnUiThread(() =>
            {
                if (!string.IsNullOrEmpty(result))
                {
                    Intent.PutExtra(NewCardExtra, result);
                    SetResult(Result.Ok, Intent);
                }
                else
                {
                    SetResult(Result.Canceled);
                }

                Finish();
            });
        }
    }
}