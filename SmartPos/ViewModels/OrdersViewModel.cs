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
        private readonly NotificationsService _notificationsService;


        public OrdersViewModel(DataBaseService dataBaseService, IBluetoothPrinterService printerService, NotificationsService notificationsService)
        {
            _dataBaseService = dataBaseService;
            _printerService = printerService;
            _notificationsService = notificationsService;
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



            var allMenuItems = await _dataBaseService.GetAllMenuItemsAsync();
            foreach (var cart in cartItems)
            {
                var mi = allMenuItems.FirstOrDefault(x => x.Id == cart.ItemId);
                if (mi == null || mi.StockQuantity == 0)
                {
                    await Shell.Current.DisplayAlert(
                        "نفاد المخزون",
                        $"لا يمكنك طلب \"{cart.Name}\" لأن المنتج غير متوفر.",
                        "موافق"
                    );
                    return false;
                }
                if (cart.Quantity > mi.StockQuantity)
                {
                    await Shell.Current.DisplayAlert(
                        "كمية غير متاحة",
                        $"لا يمكنك طلب {cart.Quantity} من \"{cart.Name}\"، المتوفر: {mi.StockQuantity}.",
                        "موافق"
                    );
                    return false;
                }
            }







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

                // 1) تقليل المخزون
                await _dataBaseService.ReduceStockAsync(orderModel.Items);











                // 1) تحديث الكميات بعد التقليص
                allMenuItems = await _dataBaseService.GetAllMenuItemsAsync();

                var lowStockOrdered = orderModel.Items
                    .Select(oi =>
                    {
                        // استرجاع العنصر من القائمة
                        var item = allMenuItems.First(mi => mi.Id == oi.ItemId);
                        return new
                        {
                            item.Name,
                            item.Description,            // إضافة الوصف هنا
                            Remaining = item.StockQuantity
                        };
                    })
                    .Where(x => x.Remaining > 0 && x.Remaining <= 4)
                    .ToArray();

                if (lowStockOrdered.Any())
                {
                    // 2) بناء نص الرسالة ليضمّ الوصف
                    var msg = string.Join("\n",
                        lowStockOrdered.Select(li =>
                            $"{li.Name} ({li.Description}) - الكمية المتبقية: {li.Remaining}"
                        )
                    );  // استخدام String Interpolation 0

                    await Shell.Current.DisplayAlert("تنبيه المخزون المنخفض", msg, "حسنًا");

                    // 3) إضافة الإشعار بالرسالة المحدثة
                    _notificationsService.Add(new NotificationModel
                    {
                        Message = msg,
                        Timestamp = DateTime.Now,    // الحصول على الوقت الحالي 1
                        Type = "LowStock"
                    });
                }


                //// تحديث الكميات بعد التقليص
                //allMenuItems = await _dataBaseService.GetAllMenuItemsAsync();
                //var lowStockOrdered = orderModel.Items
                //    .Select(oi => new {
                //        oi.Name,
                //        Remaining = allMenuItems.First(mi => mi.Id == oi.ItemId).StockQuantity
                //    })
                //    .Where(x => x.Remaining > 0 && x.Remaining <= 4)
                //    .ToArray();

                //if (lowStockOrdered.Any())
                //{
                //    var msg = string.Join("\n",
                //        lowStockOrdered.Select(li => $"{li.Name} - الكمية المتبقية: {li.Remaining}")
                //    );
                //    await Shell.Current.DisplayAlert("تنبيه المخزون المنخفض", msg, "حسنًا");
                //    _notificationsService.Add(new NotificationModel
                //    {
                //        Message = msg,
                //        Timestamp = DateTime.Now,
                //        Type = "LowStock"
                //    });
                //}



                //// 2) التحقق من الأصناف منخفضة المخزون
                //var allItems = await _dataBaseService.GetAllMenuItemsAsync();
                //var lowStock = allItems.Where(mi => mi.StockQuantity <= 4).ToArray();
                //if (lowStock.Any())
                //{
                //    // نبني مصفوفة من "اسم المنتج - الكمية المتبقية"
                //    var lines = lowStock
                //        .Select(li => $"{li.Name} - الكمية المتبقية: {li.StockQuantity}")
                //        .ToArray();

                //    // نجمعها بفواصل سطرية
                //    var alertMessage = string.Join("\n", lines);

                //    // نعرض التنبيه
                //    await Shell.Current.DisplayAlert("تنبيه المخزون", alertMessage, "حسنًا");
                //}







                //   ______________ //
                //var lowStock = allItems.Where(mi => mi.StockQuantity <= 4).ToArray();
                //if (lowStock.Any())
                //{
                //    var names = string.Join(", ", lowStock.Select(li => li.Name));
                //    await Shell.Current.DisplayAlert(
                //        "تنبيه المخزون",
                //        $"المخزون منخفض للأصناف التالية: {names}",
                //        "حسنًا"
                //    );
                //}

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

