using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SmartPos.Data;
using SmartPos.Models;
using SmartPos.Services;
using SmartPos.Resources.Strings;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CommunityToolkit.Maui.Views;
using SmartPos.Pages.Popups;
using System.Threading.Tasks;

namespace SmartPos.ViewModels
{
    public partial class OrdersViewModel : ObservableObject, IRecipient<CultureChangedMessage>
    {
        private readonly DataBaseService _dataBaseService;
        private readonly IBluetoothPrinterService _printerService;
        private readonly NotificationsService _notificationsService;


        public OrdersViewModel(DataBaseService dataBaseService, IBluetoothPrinterService printerService, NotificationsService notificationsService)
        {
            _dataBaseService = dataBaseService;
            _printerService = printerService;
            _notificationsService = notificationsService;

            WeakReferenceMessenger.Default.Register(this);
        }

        // مجموعة الطلبات لعرضها في الواجهة
        public ObservableCollection<OrderModel> Orders { get; set; } = new ObservableCollection<OrderModel>();


        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayOrderItems))]
        private OrdersItem[] _orderItems = new OrdersItem[] { };

        public CartModel[] DisplayOrderItems => _orderItems
                        .Select(oi => new CartModel
                        {
                            ItemId = oi.ItemId,
                            NmKey = oi.Name,
                            Icon = oi.Icon,
                            Price = oi.Price,
                            Quantity = oi.Quantity
                        })
            .ToArray();

        private bool _isInitialized;

        // أمر الطباعة
        [RelayCommand]
        private async Task PrintOrderAsync(OrderModel? order)
        {
            if (order is null)
                return;

            // 1) جلب عناصر الطلب
            var items = await _dataBaseService.GetOrdersItemsAsync(order.Id);

            // 2) تحويل العناصر إلى CartModel للاستفادة من DisplayNmKey و PriceText
            var printableItems = items.Select(oi => new CartModel
            {
                ItemId = oi.ItemId,
                NmKey = oi.Name,
                Icon = oi.Icon,
                Price = oi.Price,
                Quantity = oi.Quantity
            }).ToArray();

            // 3) توليد نص الإيصال
            var printText = BuildOrderText(order, printableItems);

            // 4) إنشاء Popup للمعاينة
            var preview = new PrintPreviewPopup(printText, async () =>
            {
                var isConnected = await _printerService.ConnectAsync();
                if (!isConnected)
                {
                    await Shell.Current.DisplayAlert($"{AppResources.Prompt_PrintError_Connection_Title}", $"{AppResources.Prompt_PrintError_Connection_Message}", $"{AppResources.Prompt_Ok}");
                    return;
                }

                var success = await _printerService.PrintAsync(printText);
                await _printerService.DisconnectAsync();

                if (success)
                    await Shell.Current.DisplayAlert($"{AppResources.Toast_PrintTex}", $"{AppResources.Toast_PrintSuccess}", $"{AppResources.Prompt_Ok}");
                else
                    await Shell.Current.DisplayAlert($"{AppResources.Prompt_PrintError_Connection_Title}", $"{AppResources.Prompt_PrintError_PrintFailed_Message}", $"{AppResources.Prompt_Ok}");
            });

            // 5) عرض نافذة المعاينة
            await Application.Current.MainPage.ShowPopupAsync(preview);
        }
        //{
        //    if (order == null || order.Id == 0)
        //    {
        //        await Toast.Make("لم يتم اختيار طلب للطباعة.").Show();
        //        return;
        //    }

        //    try
        //    {
        //        IsLoading = true;

        //        // جلب عناصر الطلب من قاعدة البيانات
        //        var items = await _dataBaseService.GetOrdersItemsAsync(order.Id);
        //        OrderItems = items;
        //        OnPropertyChanged(nameof(OrderItems));

        //        // بناء نص الإيصال
        //        var printText = BuildOrderText(order, items);

        //        // محاولة الاتصال
        //        var isConnected = await _printerService.ConnectAsync();
        //        if (!isConnected)
        //        {
        //            await Shell.Current.DisplayAlert("خطأ", "فشل الاتصال بالطابعة عبر البلوتوث.", "موافق");
        //            return;
        //        }

        //        // الطباعة
        //        var printed = await _printerService.PrintAsync(printText);
        //        if (!printed)
        //        {
        //            await Shell.Current.DisplayAlert("خطأ", "حدث خلل أثناء عملية الطباعة.", "موافق");
        //        }
        //        else
        //        {
        //            await Toast.Make("تمت الطباعة بنجاح!").Show();
        //        }

        //        // قطع الاتصال
        //        await _printerService.DisconnectAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        await Shell.Current.DisplayAlert("خطأ", ex.Message, "موافق");
        //    }
        //    finally
        //    {
        //        IsLoading = false;
        //    }
        //}

        // دالة مساعدة لبناء النص
        private string BuildOrderText(OrderModel order, CartModel[] items)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"              === {AppResources.Receipt_Title} ===");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine($"{AppResources.Receipt_Date}:             {order.OrderDate:  yyyy-MM-dd   HH:mm:ss}");
            sb.AppendLine("");
            sb.AppendLine($"{AppResources.Receipt_OrderNumber}:              {order.Id}");
            sb.AppendLine("");
            sb.AppendLine($"{AppResources.OrdersPage_Header_PayMode}:              {order.PaymentModeText}");
            sb.AppendLine("------------------------------------------------------------------");

            foreach (var item in items)
            {
                sb.AppendLine($"{item.DisplayNmKey}      X {item.Quantity}         =  {item.PriceText} ");
            }

            sb.AppendLine("------------------------------------------------------------------");
            sb.AppendLine("");
            sb.AppendLine($"{AppResources.Receipt_Total}:                                     {order.TotalAmountPaidText} ");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine($"      {AppResources.Receipt_Thanks}");
            sb.AppendLine("");
            sb.AppendLine("                           - Smart POS -");
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
                    var title = AppResources.Prompt_StockEmpty_Title;
                    var msg = string.Format(AppResources.Prompt_StockEmpty_Message, cart.DisplayNmKey);
                    await Shell.Current.DisplayAlert(AppResources.Prompt_StockEmpty_Title, msg, AppResources.Prompt_PlaceOrderError_Accept/*"نفاد المخزون", $"لا يمكنك طلب \"{cart.DisplayNmKey}\" لأن المنتج غير متوفر.", "موافق"*/);
                    _notificationsService.Add(new NotificationModel
                    {
                        Message = $"{title}\n{msg}",
                        Timestamp = DateTime.Now,
                        Type = "OutOfStock"
                    });
                    return false;
                }
                if (cart.Quantity > mi.StockQuantity)
                {
                    await Shell.Current.DisplayAlert(AppResources.Prompt_StockQuantityUnavailable_Title, string.Format(AppResources.Prompt_StockQuantityUnavailable_Message, cart.Quantity, cart.DisplayNmKey, mi.StockQuantity), AppResources.Prompt_PlaceOrderError_Accept/*"كمية غير متاحة", $"لا يمكنك طلب {cart.Quantity} من \"{cart.DisplayNmKey}\"، المتوفر: {mi.StockQuantity}.","موافق"*/);
                    return false;
                }
            }







            // منطقيتك الحالية لإنشاء الطلب
            var orderItems = cartItems.Select(c => new OrdersItem
            {
                Icon = c.Icon,
                ItemId = c.ItemId,
                Name = c.NmKey,
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
                await Toast.Make(AppResources.Toast_OrderCreated).Show();

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
                            item.DisplayNameK,
                            item.DisplayDescription,            // إضافة الوصف هنا
                            Remaining = item.StockQuantity
                        };
                    })
                    .Where(x => x.Remaining > 0 && x.Remaining <= 4)
                    .ToArray();

                if (lowStockOrdered.Any())
                {
                    // 2) بناء نص الرسالة ليضمّ الوصف
                    var msg = string.Join("\n",
                        lowStockOrdered.Select(li => String.Format(AppResources.Prompt_LowStockAlert_Message, li.DisplayNameK, li.DisplayDescription, li.Remaining)
                        //$"{li.DisplayNameK} ({li.DisplayDescription}) - الكمية المتبقية: {li.Remaining}"
                        )
                    );  // استخدام String Interpolation 0

                    await Shell.Current.DisplayAlert(AppResources.Prompt_LowStockAlert_Title/*"تنبيه المخزون المنخفض"*/, msg, AppResources.Prompt_LowStockAlert_Accept);

                    // 3) إضافة الإشعار بالرسالة المحدثة
                    _notificationsService.Add(new NotificationModel
                    {
                        Message = msg,
                        Timestamp = DateTime.Now,    // الحصول على الوقت الحالي 1
                        Type = "LowStock"
                    });
                }


                var outOfStockItems = orderModel.Items
                    .Select(oi =>
                    {
                        var item = allMenuItems.First(mi=> mi.Id == oi.ItemId);
                        return new
                        {
                            item.DisplayNameK,
                            item.DisplayDescription,
                            Remaining = item.StockQuantity
                        };
                    })
                        .Where(x => x.Remaining == 0)
                        .ToArray();

                if (outOfStockItems.Any())
                {
                    var msg = string.Join("\n",
                        outOfStockItems.Select(li => String.Format(AppResources.Prompt_StockEmpty_Message, li.DisplayNameK, li.DisplayDescription))
                    );
                    await Shell.Current.DisplayAlert(AppResources.Prompt_StockEmpty_Title, msg, AppResources.Prompt_PlaceOrderError_Accept);
                    _notificationsService.Add(new NotificationModel
                    {
                        Message = msg,
                        Timestamp = DateTime.Now,
                        Type = "OutOfStock"
                    });
                }




                return true;
            }

            await Shell.Current.DisplayAlert(AppResources.Prompt_PlaceOrderError_Title, errorMessage, AppResources.Prompt_PlaceOrderError_Accept);
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



        public string ordtext => AppResources.OrdersPage_Title;
        public string orIdtext => AppResources.OrdersPage_Header_OrderId;
        public string orDateText => AppResources.OrdersPage_Header_OrderDate;
        public string orAmountText => AppResources.OrdersPage_Header_Amount;
        public string orPmodText => AppResources.OrdersPage_Header_PayMode;
        public string orItemsText => AppResources.OrdersPage_Header_NoOfItems;
        public string orPrintText => AppResources.OrdersPage_Button_Printing;
        public string orSelectText => AppResources.OrdersPage_SelectedItems_Title;
        public string NoOrdSText => AppResources.OrdersPage_Empty_NoOrderSelected;
        public string SeOrdText => AppResources.OrdersPage_Empty_SelectOrder;
        public string CurrencyText => AppResources.CurrencySymbol;


        public async void Receive(CultureChangedMessage message)
        {
            await InitializeAsync();

            OnPropertyChanged(nameof(Orders));
            OnPropertyChanged(nameof(DisplayOrderItems));
            OnPropertyChanged(nameof(ordtext));
            OnPropertyChanged(nameof(orIdtext));
            OnPropertyChanged(nameof(orDateText));
            OnPropertyChanged(nameof(orAmountText));
            OnPropertyChanged(nameof(orPmodText));
            OnPropertyChanged(nameof(orItemsText));
            OnPropertyChanged(nameof(orPrintText));
            OnPropertyChanged(nameof(orSelectText));
            OnPropertyChanged(nameof(NoOrdSText));
            OnPropertyChanged(nameof(SeOrdText));
            OnPropertyChanged(nameof(CurrencyText));
        }
    }
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