using SmartPos.ViewModels;

namespace SmartPos.Pages;

public partial class ManageMenuCategoriesPage : ContentPage
{
    private readonly ManageMenuCategoriesViewModel _manageMenuCategoriesViewModel;
    public ManageMenuCategoriesPage(ManageMenuCategoriesViewModel manageMenuCategoriesViewModel)
    {
        InitializeComponent();
        _manageMenuCategoriesViewModel = manageMenuCategoriesViewModel;
        BindingContext = _manageMenuCategoriesViewModel;
        InitializeAsync();
    }

    private async void InitializeAsync() =>
    await _manageMenuCategoriesViewModel.InitializeAsync();

    private void SaveMenuCategoriesFormControl_OnCancel()
    {
        _manageMenuCategoriesViewModel.CancelCommand.Execute(null);
    }

    private async void SaveMenuCategoriesFormControl_OnSaveItem(Models.MenuCategoryModel menuCategoryModel)
    {
        await _manageMenuCategoriesViewModel.SaveCategoryCommand.ExecuteAsync(menuCategoryModel);
    }
}
