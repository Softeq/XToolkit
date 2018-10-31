// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Stripe.Android;
using Com.Stripe.Android.Model;
using Softeq.XToolkit.Common;
using Softeq.XToolkit.Common.Command;
using Softeq.XToolkit.Common.Extensions;
using Softeq.XToolkit.Stripe.Droid.Services;
using Object = Java.Lang.Object;

namespace Softeq.XToolkit.Stripe.Droid
{
    [Activity]
    internal class CardsActivity : AppCompatActivity
    {
        private const int RequestCodeAddCard = 700;
        private Button _addButton;
        private CardsAdapter _cardsAdapter;
        private ImageButton _doneButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.cards_page);

            var recyclerView = FindViewById<RecyclerView>(Resource.Id.cards_page_list_view);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));
            _cardsAdapter = new CardsAdapter(new RelayCommand<CustomerSource>(RemoveSourceAsync));
            recyclerView.SetAdapter(_cardsAdapter);

            _addButton = FindViewById<Button>(Resource.Id.cards_page_add_button);
            _addButton.Click += OnAddCardClicked;

            _doneButton = FindViewById<ImageButton>(Resource.Id.cards_page_ok_button);
            _doneButton.Click += OnDoneButtonClick;

            LoadDataAsync().SafeTaskWrapper();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == RequestCodeAddCard && resultCode == Result.Ok)
            {
                var cardToken = data.GetStringExtra(AddCardActivity.NewCardExtra);
                AddSourceAsync(cardToken).SafeTaskWrapper();
            }
        }

        private void OnDoneButtonClick(object sender, EventArgs e)
        {
            if (_cardsAdapter.SelectedCustomerSource != null)
            {
                CustomerSession.Instance.SetCustomerDefaultSource(
                    this,
                    _cardsAdapter.SelectedCustomerSource.Id,
                    _cardsAdapter.SelectedCustomerSource.SourceType,
                    new CustomerRetrievalListener(new RelayCommand<bool>(OnSourceSelected)));
            }
        }

        private void OnSourceSelected(bool isSelected)
        {
            SetResult(isSelected ? Result.Ok : Result.Canceled);
            Finish();
        }

        private void OnAddCardClicked(object sender, EventArgs e)
        {
            StartActivityForResult(typeof(AddCardActivity), RequestCodeAddCard);
        }

        private async Task LoadDataAsync()
        {
            var cards = await StripeManager.Instance.GetUserCardsAsync().ConfigureAwait(false);
            RunOnUiThread(() => { _cardsAdapter.SetItems(cards); });
        }

        private async void RemoveSourceAsync(CustomerSource customerSource)
        {
            var result = await StripeManager.Instance.DetachSourceFromCustomerAsync(customerSource.Id)
                .ConfigureAwait(false);
            if (result)
            {
                await LoadDataAsync().ConfigureAwait(false);
            }
        }

        private async Task AddSourceAsync(string token)
        {
            var result = await StripeManager.Instance.AttachSourceToCustomerAsync(token).ConfigureAwait(false);
            if (result)
            {
                await LoadDataAsync().ConfigureAwait(false);
            }
        }

        private class CustomerRetrievalListener : Object, CustomerSession.ICustomerRetrievalListener
        {
            private readonly RelayCommand<bool> _relayCommand;

            public CustomerRetrievalListener(RelayCommand<bool> relayCommand)
            {
                _relayCommand = relayCommand;
            }

            public void OnCustomerError(int p0, string p1)
            {
                _relayCommand.Execute(false);
            }

            public void OnCustomerRetrieved(Customer customer)
            {
                _relayCommand.Execute(true);
            }
        }

        private class CardsAdapter : RecyclerView.Adapter
        {
            private readonly List<CustomerSource> _items = new List<CustomerSource>();
            private readonly RelayCommand<CustomerSource> _removeCommand;
            private int _selectedIndex;

            public CardsAdapter(RelayCommand<CustomerSource> removeCommand)
            {
                _removeCommand = removeCommand;
            }

            public override int ItemCount => _items.Count;

            public CustomerSource SelectedCustomerSource => _selectedIndex == -1 ? null : _items[_selectedIndex];

            public void SetItems(IEnumerable<CustomerSource> items)
            {
                _items.Clear();
                _items.AddRange(items);
                _selectedIndex = -1;
                NotifyDataSetChanged();
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var cardViewHolder = (CardViewHolder) holder;
                cardViewHolder.BindHolder(_items[position], position == _selectedIndex);
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.card_item, parent, false);
                return new CardViewHolder(itemView, new RelayCommand<CustomerSource>(OnSourceSelected), _removeCommand);
            }

            private void OnSourceSelected(CustomerSource customerSource)
            {
                _selectedIndex = _items.IndexOf(customerSource);
                NotifyDataSetChanged();
            }
        }

        private class CardViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly RelayCommand<CustomerSource> _checkCommand;
            private readonly ImageView _imageView;
            private readonly ImageButton _removeButton;
            private readonly RelayCommand<CustomerSource> _removeCommand;
            private readonly ImageView _selectedImage;
            private readonly TextView _textView;

            private WeakReferenceEx<CustomerSource> _source;

            public CardViewHolder(
                View itemView,
                RelayCommand<CustomerSource> checkCommand,
                RelayCommand<CustomerSource> removeCommand) : base(itemView)
            {
                _textView = itemView.FindViewById<TextView>(Resource.Id.card_item_title);
                _selectedImage = itemView.FindViewById<ImageView>(Resource.Id.card_item_selected_image);
                _imageView = itemView.FindViewById<ImageView>(Resource.Id.card_item_image);
                _removeButton = itemView.FindViewById<ImageButton>(Resource.Id.card_item_remove_button);

                _selectedImage.SetImageResource(Resource.Drawable.ic_checkmark);
                _checkCommand = checkCommand;
                _removeCommand = removeCommand;
                itemView.SetOnClickListener(this);
                _removeButton.Click += OnRemoveButtonClick;
            }

            public void OnClick(View v)
            {
                _checkCommand.Execute(_source.Target);
            }

            public void BindHolder(CustomerSource item, bool isSelected)
            {
                _textView.Text = item.Id;
                _source = WeakReferenceEx.Create(item);

                var card = item.AsCard();

                _textView.Text = $"{card.Brand} - ...{card.Last4}";

                var imageId = (int) Card.BrandResourceMap[card.Brand];

                _imageView.SetImageResource(imageId);

                _selectedImage.Visibility = isSelected ? ViewStates.Visible : ViewStates.Invisible;
            }

            private void OnRemoveButtonClick(object sender, EventArgs e)
            {
                _removeCommand.Execute(_source.Target);
            }
        }
    }
}