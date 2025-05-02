using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SmartPos.Data;
using SmartPos.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuItem = SmartPos.Data.MenuItem;

namespace SmartPos.ViewModels
{
    public partial class ManageMenuItemsViewModel : ObservableObject
    {
        private readonly DataBaseService _dataBaseService;

        public ManageMenuItemsViewModel(DataBaseService dataBaseService)
        {
            _dataBaseService = dataBaseService;
            WeakReferenceMessenger.Default.Register<CultureChangedMessage>(this, async (r, msg) =>
            {
                await InitializeAsync();
            });
            WeakReferenceMessenger.Default.Register<CategoryChangedMessage>(this, async (r, msg) =>
            {
                await InitializeAsync();
            });
        }


        [ObservableProperty]
        private MenuCategoryModel[] _categories = [];

        [ObservableProperty]
        private MenuItem[] _menuItems = [];

        [ObservableProperty]
        private MenuCategoryModel? _SelectedCategory = null;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private MenuItemModel _menuItem = new();

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

            SetEmptyCategoriesToItem();



            IsLoading = false;

        }

        private void SetEmptyCategoriesToItem()
        {
            MenuItem.Categories.Clear();
            foreach (var category in Categories)
            {
                var categoryOfItem = new MenuCategoryModel
                {
                    Id = category.Id,
                    Icon = category.Icon,
                    NameKey = category.NameKey,
                    IsSelected = false
                };
                MenuItem.Categories.Add(categoryOfItem);
            }
        }

        [RelayCommand]
        private async Task SelectCategoryAsync(int categoryId)
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


        [RelayCommand]
        private async Task EditMenuItemAsync(MenuItem menuItem)
        {
            //await Shell.Current.DisplayAlert("Edit", "Edit Menu Item", "ok");

            var menuItemModel = new MenuItemModel
            {
                DescriptionKey = menuItem.DescriptionKey,
                Icon = menuItem.Icon,
                Id = menuItem.Id,
                NameK = menuItem.NameK,
                Price = menuItem.Price,
                StockQuantity = menuItem.StockQuantity
            };
            var itemCategories = await _dataBaseService.GetCategoriesOfMenuItem(menuItem.Id);

            foreach (var category in Categories)
            {
                var categoryOfItem = new MenuCategoryModel
                {
                    Icon = category.Icon,
                    Id = category.Id,
                    NameKey = category.NameKey
                };
                if (itemCategories.Any(c => c.Id == category.Id))
                    categoryOfItem.IsSelected = true;
                else
                    categoryOfItem.IsSelected = false;

                menuItemModel.Categories.Add(categoryOfItem);
            }

            MenuItem = menuItemModel;

        }

        [RelayCommand]
        private void Cancel()
        {
            MenuItem = new();
            SetEmptyCategoriesToItem();
        }

        [RelayCommand]
        private async Task SaveMenuItemAsync(MenuItemModel model)
        {
            //IsLoading = true;
            //Debug.WriteLine($"[DEBUG] SaveMenuItemCommand triggered with ID: {model.Id}");

            //try
            //{
            //    var errorMessage = await _dataBaseService.SaveMenuItemAsync(model);

            //    if (errorMessage != null)
            //    {
            //    await Shell.Current.DisplayAlert("Error", errorMessage, "ok");
            //    }
            //     else
            //     {
            //        // await Toast.Make("Menu Item Saved Successfully").Show();
            //        await Shell.Current.DisplayAlert("Succes", "Item saved", "ok");
            //    Cancel();
            //     }
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine($"[ERROR] Exception in SaveMenuItemCommand: {ex.Message}");
            //    await Shell.Current.DisplayAlert("Exception", "Somthing went wrong while saving", "ok");
            //}
            //        IsLoading = false;
            IsLoading = true;

            // Save this Item to database
            var errorMessage = await _dataBaseService.SaveMenuItemAsync(model);

            if (errorMessage != null)
            {
                await Shell.Current.DisplayAlert("Error", errorMessage, "ok");
            }
            else
            {
                await Toast.Make("Menu Item Saved Successfully").Show();
                HandleMenuItemChanged(model);

                // Send the updated menu item details to the other parts of app

                WeakReferenceMessenger.Default.Send(MenuItemChangedMessage.From(model));
                Cancel();
            }

            IsLoading = false;
        }
        private void HandleMenuItemChanged(MenuItemModel model)
        {
            var menuItem = MenuItems.FirstOrDefault(m => m.Id == model.Id);
            if(menuItem != null)
            {
                // This menu item is on the screen the right now

                // Check if this item still has a mapping to selected category
                if(!model.SelectedCategories.Any(c=>c.Id == SelectedCategory.Id))
                {
                    // This item no longer belong to the selected category 
                    // Remove this item from the current UI Menu Items list
                    MenuItems = [.. MenuItems.Where(m => m.Id != model.Id)];
                    return;
                }
                // Update the details
                menuItem.Price = model.Price;
                menuItem.DescriptionKey = model.DescriptionKey;
                menuItem.Icon = model.Icon;
                menuItem.NameK = model.NameK;
                menuItem.StockQuantity = model.StockQuantity;

                MenuItems = [.. MenuItems];
            }
            else if (model.SelectedCategories.Any(c=>c.Id == SelectedCategory.Id))
            {
                // This item was not on the UI
                // We updated the item by adding this currently selected category
                // So we add this menu item to the current UI Menu Items list 

                var newMenuItem = new MenuItem
                {
                    Id = model.Id,
                    DescriptionKey = model.DescriptionKey,
                    Icon = model.Icon,
                    NameK = model.NameK,
                    Price = model.Price,
                    StockQuantity = model.StockQuantity,
                };

                MenuItems = [.. MenuItems, newMenuItem];
            }
        }
    }


}
