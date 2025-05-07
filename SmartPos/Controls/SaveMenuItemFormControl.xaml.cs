using CommunityToolkit.Mvvm.Input;
using SmartPos.Models;
using SmartPos.Resources.Strings;
using System.Threading.Tasks;

namespace SmartPos.Controls;

public partial class SaveMenuItemFormControl : ContentView
{

    private const string DefaultIcon = "imageadd.png";


    public SaveMenuItemFormControl()
	{
		InitializeComponent();
	}

   // public static readonly BindableProperty CategoriesProperty =
   //     BindableProperty.Create(nameof(Categories), typeof(MenuCategoryModel[]), typeof(SaveMenuItemFormControl), Array.Empty<MenuCategoryModel>());



     //  public MenuCategoryModel[] Categories
     //  {

       // get => (MenuCategoryModel[])GetValue(CategoriesProperty);

       // set => SetValue(CategoriesProperty, value);

     //   }


    public static readonly BindableProperty ItemProperty =
        BindableProperty.Create(nameof(Item), typeof(MenuItemModel), typeof(SaveMenuItemFormControl), new MenuItemModel(), propertyChanged : OnItemChanged);
    public MenuItemModel Item
    {
        get => (MenuItemModel)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value); 
    }

    public static void OnItemChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if(newValue is MenuItemModel menuItemModel)
        {
            if(bindable is SaveMenuItemFormControl thisControl)
            {
                if(menuItemModel.Id > 0)
                {
                    // thisControl.itemIcon.Source = menuItemModel.Icon;
                    // thisControl.itemIcon.HeightRequest = thisControl.itemIcon.WidthRequest = 60;

                    thisControl.SetIconImage(false, menuItemModel.Icon, thisControl);
                    thisControl.ExistingIcon = menuItemModel.Icon;
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

    [RelayCommand]
    private void ToggleCategorySelection(MenuCategoryModel category) =>

        category.IsSelected = !category.IsSelected;

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

            Item.Icon = localPath;
        }
        else
        {
            // User Canceled from image picker dialog

            // itemIcon.Source = "imageadd.png";
            // itemIcon.HeightRequest = itemIcon.WidthRequest = 25;

            if(ExistingIcon != null)
            {
                SetIconImage(isDefault: false, ExistingIcon);

            }
            else
            {
                SetIconImage(isDefault: true);
            }
        }
    }

    public void SetIconImage(bool isDefault, string? iconSource=null, SaveMenuItemFormControl? control = null)
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

    public event Action<MenuItemModel>? OnSaveItem;

    [RelayCommand]
    private async Task SaveMenuItemAsync()
    {
        // validation

        if(string.IsNullOrWhiteSpace(Item.NameK) || string.IsNullOrWhiteSpace(Item.DescriptionKey))
        {
            await ErrorAlertAsync($"{AppResources.AlertSaveMenuItms}");
            return;
        }


        if(//!Item.Categories.Any(c=> c.IsSelected) ||
            Item.SelectedCategories.Length == 0
            )
        {

            await ErrorAlertAsync($"{AppResources.SElectAtLeastCategory}");
            return;
        }


        if(Item.Icon == DefaultIcon)
        {
            await ErrorAlertAsync($"{AppResources.AlertSaveMenuIcon}");
            return;
        }


        OnSaveItem?.Invoke(Item);

        static async Task ErrorAlertAsync(string message) =>
            await Shell.Current.DisplayAlert($"{AppResources.ValidationMesag}", message, $"{AppResources.Prompt_Ok}");

    }
}