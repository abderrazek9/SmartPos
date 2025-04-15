using SmartPos.ViewModels;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SmartPos.Pages;

public partial class ManageMenuItemPage : ContentPage
{
    private readonly ManageMenuItemsViewModel _manageMenuItemsViewModel;

    public ManageMenuItemPage(ManageMenuItemsViewModel manageMenuItemsViewModel)
	{
		InitializeComponent();
        _manageMenuItemsViewModel = manageMenuItemsViewModel;
        BindingContext = _manageMenuItemsViewModel;
        InitializeAsync();
    }
    private async void InitializeAsync() =>
        await _manageMenuItemsViewModel.InitializeAsync();


   // private async Task CategoriesListControl_OnCategorySelected(Models.MenuCategoryModel category) =>
      //  await _manageMenuItemsViewModel.SelectCategoryCommand.ExecuteAsync(category.Id);


   // private async Task MenuItemsListControl_OnSelectItem(Data.MenuItem menuItem) =>
      //  await _manageMenuItemsViewModel.EditMenuItemCommand.ExecuteAsync(menuItem);

    private async void CategoriesListControl_OnCategorySelected(object obj)
    {
        if (obj is Models.MenuCategoryModel category)
        {
            await _manageMenuItemsViewModel.SelectCategoryCommand.ExecuteAsync(category.Id);
        }
    }

    private async void MenuItemsListControl_OnSelectItem(object obj)
    {
        if (obj is Data.MenuItem menuItem)
        {
            await _manageMenuItemsViewModel.EditMenuItemCommand.ExecuteAsync(menuItem);
        }
    }

    private void SaveMenuItemFormControl_OnCancel()
    {
        _manageMenuItemsViewModel.CancelCommand.Execute(null);
    }

    private async void SaveMenuItemFormControl_OnSaveItem(Models.MenuItemModel menuItemModel)
    {
        //Debug.WriteLine(">> SaveMenuItemFormControl_OnSaveItem triggered");
        await _manageMenuItemsViewModel.SaveMenuItemCommand.ExecuteAsync(menuItemModel);
        
    }
}