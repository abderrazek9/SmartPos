using CommunityToolkit.Mvvm.ComponentModel;
using SmartPos.Resources.Strings;

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

        [ObservableProperty, NotifyPropertyChangedFor(nameof(Amount))]
        private int _quantity;

        public decimal Amount => Price * Quantity;
    }
}
