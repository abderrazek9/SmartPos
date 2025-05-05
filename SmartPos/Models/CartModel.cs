using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using SmartPos.Resources.Strings;
using System.Globalization;

namespace SmartPos.Models
{
    public partial class CartModel : ObservableObject
    {
        public int ItemId { get; set; }


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayNmKey))]
        private string nmKey;

        public string DisplayNmKey =>
                                        AppResources.ResourceManager.GetString(NmKey, AppResources.Culture)
                                                                                       ?? NmKey;

        //public string Name { get; set; }

        public string Icon { get; set; }

        public decimal Price { get; set; }

        public string PriceText => string.Format(CultureInfo.CurrentCulture,"{0:N2} {1}", Price , AppResources.CurrencySymbol);

        [ObservableProperty, NotifyPropertyChangedFor(nameof(Amount))]
        private int _quantity;

        public decimal Amount => Price * Quantity;

        public CartModel()
        {
            WeakReferenceMessenger.Default.Register<CultureChangedMessage>(this, (r, m) =>
            {
                OnPropertyChanged(nameof(DisplayNmKey));
                OnPropertyChanged(nameof(PriceText));
            });
        }
    }
}
