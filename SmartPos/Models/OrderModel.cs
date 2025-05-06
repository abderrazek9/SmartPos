using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using SmartPos.Data;
using SmartPos.Resources.Strings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPos.Models
{
    public partial class OrderModel : ObservableObject
    {
        public long Id { get; set; }

        public DateTime OrderDate { get; set; }

        public int TotalItemsCount { get; set; }
        public string ItemsCountText => string.Format(AppResources.OrdersPage_ItemCountFormat, TotalItemsCount);

        public decimal TotalAmountPaid { get; set; }
        public string TotalAmountPaidText => string.Format(CultureInfo.CurrentCulture, "{0:N2} {1}",TotalAmountPaid,AppResources.CurrencySymbol);

        public string PaymentMode { get; set; }  // Cash or Online

        public string PaymentModeText => PaymentMode == "Cash" ? AppResources.Cash : PaymentMode == "Online" ? AppResources.Online : PaymentMode;

        public OrdersItem[] Items { get; set; }

        [ObservableProperty]
        private bool _isSelected;

        public OrderModel()
        {
            // Register for culture change 
            WeakReferenceMessenger.Default.Register<CultureChangedMessage>(this, (r, m) =>
            {
                OnPropertyChanged(nameof(ItemsCountText));
                OnPropertyChanged(nameof(PaymentModeText));
                OnPropertyChanged(nameof(TotalAmountPaidText));
            });
        }
    }
}
