using SmartPos.ViewModels;

namespace SmartPos.Pages;

public partial class ManageMenuCategoriesPage : ContentPage
{
    private readonly ManageMenuCategoriesViewModel _vm;
    public ManageMenuCategoriesPage(ManageMenuCategoriesViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitializeAsync();
    }
}
