using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartPos.Data;
using SmartPos.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPos.ViewModels
{
    public partial class OrdersViewModel : ObservableObject
    {
        private readonly DataBaseService _dataBaseService;

        public OrdersViewModel(DataBaseService dataBaseService)
        {
            _dataBaseService = dataBaseService;
        }

        public ObservableCollection<OrderModel> Orders { get; set; } = [];

        // Return true if the order creation was succesfully , false otherwise
        public async Task<bool> PlaceOrderAsync(CartModel[] cartItems, bool isPaidOnline)
        {
            var orderItems = cartItems.Select(c => new OrdersItem
            {
                Icon = c.Icon,
                ItemId = c.ItemId,
                Name = c.Name,
                Price =c.Price,
                Quantity = c.Quantity,
            }).ToArray();
            var orderModel = new OrderModel
            {
                OrderDate = DateTime.Now,
                PaymentMode = isPaidOnline? "Online" : "Cash",
                TotalAmountPaid = cartItems.Sum(c=> c.Amount),
                TotalItemsCount = cartItems.Length,
                Items = orderItems
            };

            var errorMessage = await _dataBaseService.PlaceOrderAsync(orderModel);
            if (string.IsNullOrEmpty(errorMessage))
            {
                // Order Creation was succesfully
                Orders.Add(orderModel);
            await Toast.Make("Order placed succesfully").Show();
            return true;
       
            }    
               // Order Creation failed
                await Shell.Current.DisplayAlert("Error", errorMessage, "ok");
                return false;       
            
        }

        private bool _isInitialized;

        [ObservableProperty]
        private bool _isLoading;

        public async ValueTask InitializeAsync()
        {
            if (_isInitialized)
                return;

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
                TotalItemsCount = o.TotalItemsCount,
                
            });
            foreach (var order in orders)
            {
                Orders.Add(order);
            }
            IsLoading = false;
        }


         [ObservableProperty]
          private OrdersItem[] _orderItems = [];


        [RelayCommand]
        private async Task SelectOrderAsync(OrderModel? order)
        {

            var preSelectedOrder = Orders.FirstOrDefault(o => o.IsSelected);
            if(preSelectedOrder != null)
            {
                preSelectedOrder.IsSelected = false;
                if(preSelectedOrder.Id == order?.Id)
                {
                    OrderItems = [];
                    return;
                }
            }

            if (order == null || order.Id == 0)
            {
                OrderItems = [];
                OnPropertyChanged(nameof(OrderItems));
                return;
            }
            IsLoading = true;
            order.IsSelected = true;
             OrderItems = await _dataBaseService.GetOrdersItemsAsync(order.Id);
            OnPropertyChanged(nameof(OrderItems));
            IsLoading = false;

        }

       

    }
}
