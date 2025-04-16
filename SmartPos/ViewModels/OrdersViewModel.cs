using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartPos.Data;
using SmartPos.Models;
using SmartPos.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPos.ViewModels
{
    public partial class OrdersViewModel : ObservableObject
    {
        private readonly DataBaseService _dataBaseService;
        private readonly IBluetoothPrinterService _printerService;

        public OrdersViewModel(DataBaseService dataBaseService, IBluetoothPrinterService printerService)
        {
            _dataBaseService = dataBaseService;
            _printerService = printerService;
        }

        // مجموعة الطلبات لعرضها في الواجهة
        public ObservableCollection<OrderModel> Orders { get; set; } = new ObservableCollection<OrderModel>();

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private OrdersItem[] _orderItems = new OrdersItem[] { };

        private bool _isInitialized;

        // أمر الطباعة
        [RelayCommand]
        private async Task PrintOrderAsync(OrderModel? order)
        {
            if (order == null || order.Id == 0)
            {
                await Toast.Make("لم يتم اختيار طلب للطباعة.").Show();
                return;
            }

            try
            {
                IsLoading = true;

                // جلب عناصر الطلب من قاعدة البيانات
                var items = await _dataBaseService.GetOrdersItemsAsync(order.Id);
                OrderItems = items;
                OnPropertyChanged(nameof(OrderItems));

                // بناء نص الإيصال
                var printText = BuildOrderText(order, items);

                // محاولة الاتصال
                var isConnected = await _printerService.ConnectAsync();
                if (!isConnected)
                {
                    await Shell.Current.DisplayAlert("خطأ", "فشل الاتصال بالطابعة عبر البلوتوث.", "موافق");
                    return;
                }

                // الطباعة
                var printed = await _printerService.PrintAsync(printText);
                if (!printed)
                {
                    await Shell.Current.DisplayAlert("خطأ", "حدث خلل أثناء عملية الطباعة.", "موافق");
                }
                else
                {
                    await Toast.Make("تمت الطباعة بنجاح!").Show();
                }

                // قطع الاتصال
                await _printerService.DisconnectAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("خطأ", ex.Message, "موافق");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // دالة مساعدة لبناء النص
        private string BuildOrderText(OrderModel order, OrdersItem[] items)
        {
            var sb = new StringBuilder();
            sb.AppendLine("       === إيصال الطلب ===");
            sb.AppendLine($"رقم الطلب: {order.Id}");
            sb.AppendLine($"التاريخ: {order.OrderDate:G}");
            sb.AppendLine($"طريقة الدفع: {order.PaymentMode}");
            sb.AppendLine("---------------------------");

            foreach (var item in items)
            {
                sb.AppendLine($"{item.Name} X {item.Quantity} - {item.Price} د.ج");
            }

            sb.AppendLine("---------------------------");
            sb.AppendLine($"المجموع: {order.TotalAmountPaid} د.ج");
            sb.AppendLine("   شكرًا لتسوقكم معنا!");
            sb.AppendLine("       - Smart POS -");
            return sb.ToString();
        }

        [RelayCommand]
        private async Task SelectOrderAsync(OrderModel? order)
        {
            // منطقيتك الحالية لاختيار الطلب وعرض العناصر
            var preSelectedOrder = Orders.FirstOrDefault(o => o.IsSelected);
            if (preSelectedOrder != null)
            {
                preSelectedOrder.IsSelected = false;
                if (preSelectedOrder.Id == order?.Id)
                {
                    OrderItems = new OrdersItem[] { };
                    return;
                }
            }

            if (order == null || order.Id == 0)
            {
                OrderItems = new OrdersItem[] { };
                OnPropertyChanged(nameof(OrderItems));
                return;
            }

            IsLoading = true;
            order.IsSelected = true;
            OrderItems = await _dataBaseService.GetOrdersItemsAsync(order.Id);
            OnPropertyChanged(nameof(OrderItems));
            IsLoading = false;
        }

        public async Task<bool> PlaceOrderAsync(CartModel[] cartItems, bool isPaidOnline)
        {
            // منطقيتك الحالية لإنشاء الطلب
            var orderItems = cartItems.Select(c => new OrdersItem
            {
                Icon = c.Icon,
                ItemId = c.ItemId,
                Name = c.Name,
                Price = c.Price,
                Quantity = c.Quantity,
            }).ToArray();

            var orderModel = new OrderModel
            {
                OrderDate = DateTime.Now,
                PaymentMode = isPaidOnline ? "Online" : "Cash",
                TotalAmountPaid = cartItems.Sum(c => c.Amount),
                TotalItemsCount = cartItems.Length,
                Items = orderItems
            };

            var errorMessage = await _dataBaseService.PlaceOrderAsync(orderModel);
            if (string.IsNullOrEmpty(errorMessage))
            {
                Orders.Add(orderModel);
                await Toast.Make("تم إنشاء الطلب بنجاح").Show();
                return true;
            }

            await Shell.Current.DisplayAlert("خطأ", errorMessage, "موافق");
            return false;
        }

        public async ValueTask InitializeAsync()
        {
            if (_isInitialized) return;

            _isInitialized = true;
            IsLoading = true;

            Orders.Clear();

            var dbOrders = await _dataBaseService.GetOrdersAsync();
            var orders = dbOrders.Select(o => new OrderModel
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                PaymentMode = o.PaymentMode,
                TotalAmountPaid = o.TotalAmountPaid,
                TotalItemsCount = o.TotalItemsCount
            });

            foreach (var ord in orders)
                Orders.Add(ord);

            IsLoading = false;
        }
    }
}

