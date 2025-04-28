using CommunityToolkit.Mvvm.Input;
using SmartPos.Data;
using SmartPos.Models;
namespace SmartPos.Controls;

public partial class SaveMenuCategoriesFormControl : ContentView
{

    private const string DefaultIcon = "imageadd.png";

    public SaveMenuCategoriesFormControl()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty CatProperty =
    BindableProperty.Create(nameof(Cat), typeof(MenuCategoryModel), typeof(SaveMenuCategoriesFormControl), new MenuCategoryModel(), propertyChanged: OnItemChanged);

    public MenuCategoryModel Cat
    {
        get => (MenuCategoryModel)GetValue(CatProperty);
        set => SetValue(CatProperty, value);
    }

    public static void OnItemChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (newValue is MenuCategoryModel menuCategoryModel)
        {
            if (bindable is SaveMenuCategoriesFormControl thisControl)
            {
                if (menuCategoryModel.Id > 0)
                {
                    // thisControl.itemIcon.Source = menuItemModel.Icon;
                    // thisControl.itemIcon.HeightRequest = thisControl.itemIcon.WidthRequest = 60;

                    thisControl.SetIconImage(false, menuCategoryModel.Icon, thisControl);
                    thisControl.ExistingIcon = menuCategoryModel.Icon;
                }
                else
                {
                    // thisControl.itemIcon.Source = "imageadd.png";
                    // thisControl.itemIcon.HeightRequest = thisControl.itemIcon.WidthRequest = 25;

                    thisControl.SetIconImage(true, null, thisControl);

                }
            }
        }
    }

    public string? ExistingIcon { get; set; }





    public event Action? OnCancel;

    [RelayCommand]
    private void Cancel() => OnCancel?.Invoke();

    private async void PickImageButton_Clicked(object sender, EventArgs e)
    {
        var fileResult = await MediaPicker.PickPhotoAsync();
        if (fileResult != null)
        {
            // User selected an image from the image picker dialog

            // Upload / Save image

            var localPath = Path.Combine(FileSystem.AppDataDirectory, fileResult.FileName);

            var imageStream = await fileResult.OpenReadAsync();

            using var fs = new FileStream(localPath, FileMode.Create, FileAccess.Write);
            await imageStream.CopyToAsync(fs);

            // Update the imageIcon

            // itemIcon.Source = localPath;
            // itemIcon.HeightRequest = itemIcon.WidthRequest = 60;

            SetIconImage(isDefault: false, localPath);

            Cat.Icon = localPath;
        }
        else
        {
            // User Canceled from image picker dialog

            // itemIcon.Source = "imageadd.png";
            // itemIcon.HeightRequest = itemIcon.WidthRequest = 25;

            if (ExistingIcon != null)
            {
                SetIconImage(isDefault: false, ExistingIcon);

            }
            else
            {
                SetIconImage(isDefault: true);
            }
        }
    }


    public void SetIconImage(bool isDefault, string? iconSource = null, SaveMenuCategoriesFormControl? control = null)
    {
        int size = 50;
        if (isDefault)
        {
            iconSource = DefaultIcon;
            size = 22;
        }
        control = control ?? this;

        control.itemIcon.Source = iconSource;
        control.itemIcon.HeightRequest = control.itemIcon.WidthRequest = size;
    }

    public event Action<MenuCategoryModel>? OnSaveItem;

    [RelayCommand]
    private async Task SaveMenuCategoryAsync()
    {
        // validation

        if (string.IsNullOrWhiteSpace(Cat.NameKey))
        {
            await ErrorAlertAsync("Category Name are mendatory");
            return;
        }





        if (Cat.Icon == DefaultIcon)
        {
            await ErrorAlertAsync("Icon Image is mendatory");
            return;
        }


        OnSaveItem?.Invoke(Cat);

        static async Task ErrorAlertAsync(string message) =>
            await Shell.Current.DisplayAlert("Validation Error", message, "OK");

    }




}