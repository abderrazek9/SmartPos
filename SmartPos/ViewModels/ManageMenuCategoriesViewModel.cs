using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SmartPos.Data;
using SmartPos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuCategory = SmartPos.Data.MenuCategory;


namespace SmartPos.ViewModels
{
    public partial class ManageMenuCategoriesViewModel : ObservableObject
    {
        private readonly DataBaseService _db;
        public ManageMenuCategoriesViewModel(DataBaseService db) => _db = db;

        [ObservableProperty]
        private MenuCategoryModel[] _categories = [];

        [ObservableProperty]
        private MenuCategoryModel _selectedCategory = new MenuCategoryModel();

        [ObservableProperty]
        private bool _isLoading;

        [RelayCommand]
        public async Task InitializeAsync()
        {
            IsLoading = true;
            Categories = (await _db.GetMenuCategoriesAsync())
                .Select(MenuCategoryModel.FromEntity)
                .ToArray();
            IsLoading = false;
        }

        [RelayCommand]
        private void EditCategory(MenuCategoryModel model)
        {
            SelectedCategory = new MenuCategoryModel
            {
                Id = model.Id,
                Name = model.Name,
                Icon = model.Icon
            };
        }

        [RelayCommand]
        private async Task SaveCategoryAsync()
        {
            var error = await _db.SaveMenuCategoryAsync(SelectedCategory);
            if (error != null)
                await Shell.Current.DisplayAlert("خطأ", error, "حسناً");
            else
            {
                await InitializeAsync();
                await Toast.Make("New Category Saved Successfully").Show();
                SelectedCategory = new MenuCategoryModel();
                WeakReferenceMessenger.Default.Send(CategoryChangedMessage.From(SelectedCategory));
            }
        }

        [RelayCommand]
        private async Task DeleteCategoryAsync(MenuCategoryModel model)
        {
            if (!await Shell.Current.DisplayAlert("تأكيد", "هل تريد حذف هذه الفئة؟", "نعم", "لا"))
                return;
            await _db.DeleteMenuCategoryAsync(model.Id);
            await InitializeAsync();
            WeakReferenceMessenger.Default.Send(CategoryChangedMessage.From(model));
        }

        [RelayCommand]
        private void Cancel()
        {

        }
    }
}