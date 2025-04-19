using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SmartPos.Data;
using SmartPos.Models;
using SmartPos.Pages;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using MenuItem = SmartPos.Data.MenuItem;

namespace SmartPos.ViewModels
{
    public partial class HomeViewModel : ObservableObject, IRecipient<MenuItemChangedMessage>
    {
        private readonly DataBaseService _dataBaseService;
        private readonly OrdersViewModel _ordersViewModel;
        private readonly SettingsViewModel _settingsViewModel;
        [ObservableProperty]
        private MenuCategoryModel[] _categories = [];

        [ObservableProperty]
        private MenuItem[] _menuItems = [];

        [ObservableProperty]
        private MenuCategoryModel? _SelectedCategory = null;

        public ObservableCollection<CartModel> CartItems { get; set; } = new();


        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty , NotifyPropertyChangedFor(nameof(PromoAmount))]
        [NotifyPropertyChangedFor(nameof(Total))]
        private decimal _subtotal;

        [ObservableProperty , NotifyPropertyChangedFor(nameof(PromoAmount))]
        [NotifyPropertyChangedFor(nameof(Total))]
        private int _promopercentage;


        public decimal PromoAmount => (Subtotal * Promopercentage) / 100;

        public decimal Total => Subtotal - PromoAmount;

        [ObservableProperty]
        private string _name = "Guest";

        public HomeViewModel(DataBaseService dataBaseService , OrdersViewModel ordersViewModel, SettingsViewModel settingsViewModel)
        {
            _dataBaseService = dataBaseService;
            _ordersViewModel = ordersViewModel;
            _settingsViewModel = settingsViewModel;
            CartItems.CollectionChanged += CartItems_CollectionChanged;

            WeakReferenceMessenger.Default.Register<MenuItemChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<NameChangedMessage>(this, (recipient, message) => Name = message.Value);

            // Get PromoPercentage from preferences

            Promopercentage = _settingsViewModel.GetPromoPercentage();
        }

        private void CartItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            //it will be executed whenvr
            //we are adding any items in the cart
            //removing items from the cart
            //or clearing from the cart
            RecalculateAmounts();
        }

        private bool _isIntialized;
        public async ValueTask InitializeAsync()
        {
            if (_isIntialized)
                return; //already initialzed

            _isIntialized = true;

            IsLoading = true;

            Categories = (await _dataBaseService.GetMenuCategoriesAsync())
                .Select(MenuCategoryModel.FromEntity)
                .ToArray();


            Categories[0].IsSelected = true;
            SelectedCategory = Categories[0];

            MenuItems = await _dataBaseService.GetMenuItemsByCategoryAsync(SelectedCategory.Id);  

            IsLoading = false;
            
        }

        [RelayCommand]
        private async Task SelectCategoryAsync (int categoryId)
        {
            if (SelectedCategory.Id == categoryId)
            
                return; // the current category is already selected
            
            IsLoading = true;

            var existingSelectedCategory = Categories.First(c => c.IsSelected);
            existingSelectedCategory.IsSelected = false;

            var newlySelectedCategory = Categories.First(c => c.Id == categoryId);
            newlySelectedCategory.IsSelected = true;

            SelectedCategory = newlySelectedCategory;


            MenuItems = await _dataBaseService.GetMenuItemsByCategoryAsync(SelectedCategory.Id);

            IsLoading = false;

        }



        //[RelayCommand]
        //private async Task AddToCartAsync(MenuItem menuItem)
        //{
        //    // 1) منع الطلب إذا نفد المنتج
        //    if (menuItem.StockQuantity == 0)
        //    {
        //        await Shell.Current.DisplayAlert(
        //            "عذرًا",
        //            $"لا يمكنك الطلب، المنتج \"{menuItem.Name}\" نفد من المخزون.",
        //            "موافق");
        //        return;
        //    }

        //    // 2) نقص المخزون وتحديثه في قاعدة البيانات
        //    menuItem.StockQuantity--;
        //    await _dataBaseService.UpdateMenuItemStockAsync(menuItem.Id, menuItem.StockQuantity);

        //    // 3) عرض إشعار عند وصول المخزون للعتبة المنخفضة أو النفاد
        //    const int LOW_STOCK_THRESHOLD = 5;
        //    if (menuItem.StockQuantity > 0 && menuItem.StockQuantity <= LOW_STOCK_THRESHOLD)
        //    {
        //        await Toast.Make(
        //            $"انتباه: كمية \"{menuItem.Name}\" أصبحت {menuItem.StockQuantity} فقط."
        //        ).Show();

        //        Notifications.Add(
        //            new StockNotification(menuItem.Name, menuItem.StockQuantity, isOutOfStock: false)
        //        );
        //    }
        //    else if (menuItem.StockQuantity == 0)
        //    {
        //        Notifications.Add(
        //            new StockNotification(menuItem.Name, 0, isOutOfStock: true)
        //        );
        //    }

        //    // 4) إضافة العنصر إلى السلة أو زيادة الكمية
        //    var cartItem = CartItems.FirstOrDefault(c => c.ItemId == menuItem.Id);
        //    if (cartItem == null)
        //    {
        //        cartItem = new CartModel
        //        {
        //            ItemId = menuItem.Id,
        //            Icon = menuItem.Icon,
        //            Name = menuItem.Name,
        //            Price = menuItem.Price,
        //            Quantity = 1
        //        };
        //        CartItems.Add(cartItem);
        //    }
        //    else
        //    {
        //        cartItem.Quantity++;
        //        RecalculateAmounts();
        //    }

        //    // 5) إعادة حساب الإجمالي
        //    RecalculateAmounts();
        //}

        //public ObservableCollection<StockNotification> Notifications { get; }
        //     = new ObservableCollection<StockNotification>();

        //// الأمر لعرض صفحة الإشعارات:
        //[RelayCommand]
        //private async Task ShowNotificationsAsync()
        //{
        //    await Shell.Current.GoToAsync(nameof(NotificationPage));
        //}


        [RelayCommand]
        private void AddToCart(MenuItem menuItem)
        {
            var cartItem = CartItems.FirstOrDefault(c => c.ItemId == menuItem.Id);
            if(cartItem == null)
            {
                // Item does not exist in the cart
                // Add item in the cart 

                cartItem = new CartModel
                {
                    ItemId = menuItem.Id,
                    Icon = menuItem.Icon,
                    Name = menuItem.Name,
                    Price = menuItem.Price,
                    Quantity = 1
                };
                CartItems.Add(cartItem);
            }
            else
            {
                // This item exist in the cart
                // Increase the quantity for this item in the cart
                cartItem.Quantity++;
                RecalculateAmounts();
            }

        }

        [RelayCommand]
        private void IncreaseQuantity(CartModel cartItem)
        {
            cartItem.Quantity++;
            RecalculateAmounts();
        }

        [RelayCommand]
        private void DecreaseQuantiyi(CartModel cartItem)
        {
            cartItem.Quantity--;
            if (cartItem.Quantity == 0)
                CartItems.Remove(cartItem);
            else
            RecalculateAmounts();
        }
        [RelayCommand]
        private void RemoveItemFromCart(CartModel cartItem) 
        {
            CartItems.Remove(cartItem);
            RecalculateAmounts();
        }

        [RelayCommand]
        private async Task ClearCartAsync()
        {
             if ( await Shell.Current.DisplayAlert("Clear Cart?","Do you really want to clear the cart", "Yes" , "No"))
            {
                CartItems.Clear();

            }
        }

        private void RecalculateAmounts()
        {
            Subtotal = CartItems.Sum(c => c.Amount);
        }

        [RelayCommand]
        private async Task PromoPercentageClickaAsync()
        {
            var result = await Shell.Current.DisplayPromptAsync("Prom Percentage ", "Enter The Applicable Discount Percentage", placeholder: "10", initialValue: Promopercentage.ToString());
            if (!string.IsNullOrWhiteSpace(result))
            {
                if(!int.TryParse(result , out int enteredpromopercentage))
                {
                    await Shell.Current.DisplayAlert("Invaled Value", "Entered Discount Percentage Is Invalid", "OK");
                    return;
                }

                if(enteredpromopercentage > 100)
                {
                    await Shell.Current.DisplayAlert("Invalid Value", "Discount Percentage Cannot be more then 100", "ok");
                    return;
                }

                Promopercentage = enteredpromopercentage;

                // Save it in the preferences

                _settingsViewModel.SetPromoPercentage(enteredpromopercentage);
            }
        }

        [RelayCommand]
        private async Task PlaceOrderAsync(bool isPaidOnline)
        {
            IsLoading = true;

            var ordereItems = CartItems.ToArray();

            if (await _ordersViewModel.PlaceOrderAsync(ordereItems, isPaidOnline))
            {
                CartItems.Clear();

                if (SelectedCategory != null)
                {
                    MenuItems = await _dataBaseService.GetMenuItemsByCategoryAsync(SelectedCategory.Id);
                }
            }

            //if(await _ordersViewModel.PlaceOrderAsync([.. CartItems], isPaidOnline))
            //{
            //    // order creation was succesfuly
            //    // clear the cart items

                //    CartItems.Clear();

           // }
            IsLoading = false;
        }

        public void Receive(MenuItemChangedMessage message)
        {
            var model = message.Value;
            var menuItem = MenuItems.FirstOrDefault(m => m.Id == model.Id);
            if (menuItem != null)
            {
                // This menu item is on the screen the right now

                // Check if this item still has a mapping to selected category
                if (!model.SelectedCategories.Any(c => c.Id == SelectedCategory.Id))
                {
                    // This item no longer belong to the selected category 
                    // Remove this item from the current UI Menu Items list
                    MenuItems = [.. MenuItems.Where(m => m.Id != model.Id)];
                    return;
                }
                // Update the details
                menuItem.Price = model.Price;
                menuItem.Description = model.Description;
                menuItem.Icon = model.Icon;
                menuItem.Name = model.Name;

                MenuItems = [.. MenuItems];
            }
            else if (model.SelectedCategories.Any(c => c.Id == SelectedCategory.Id))
            {
                // This item was not on the UI
                // We updated the item by adding this currently selected category
                // So we add this menu item to the current UI Menu Items list 

                var newMenuItem = new MenuItem
                {
                    Id = model.Id,
                    Description = model.Description,
                    Icon = model.Icon,
                    Name = model.Name,
                    Price = model.Price,
                };

                MenuItems = [.. MenuItems, newMenuItem];
            }

            // Check if the updated menu item is added in the cart 
            // if yes, update the info in the cart
            var cartItem = CartItems.FirstOrDefault(c => c.ItemId == model.Id);
            if(cartItem != null)
            {
                cartItem.Price = model.Price;
                cartItem.Name = model.Name;
                cartItem.Icon = model.Icon;

                var itemIndex = CartItems.IndexOf(cartItem);

                // It will trigger CollectionChanged event for replacing this item
                // witch will recalculate amounts
                CartItems[itemIndex] = cartItem;
            }
        }
    }
}
