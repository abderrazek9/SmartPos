using CommunityToolkit.Mvvm.ComponentModel;
using SmartPos.Resources.Strings;
using System.Collections.ObjectModel;

namespace SmartPos.Models
{
    public partial class MenuItemModel : ObservableObject
    {
        public int Id { get; set; }

        //[ObservableProperty]
        //private string? _name;

           // مفتاح الترجمة للاسم
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayNameK))]
        private string nameK;

        public string DisplayNameK =>
                                       AppResources.ResourceManager.GetString(NameK, AppResources.Culture)
                                                                                                         ?? NameK;

        [ObservableProperty]
        private decimal _price;

        [ObservableProperty]
        private int _stockQuantity;

       // [ObservableProperty]
       // private int _lowStockThreshold;

        [ObservableProperty]
        private string? _icon;

        //[ObservableProperty]
        //private string? _description;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayDescription))]
        private string descriptionKey;

        public string DisplayDescription =>
                                            AppResources.ResourceManager.GetString(DescriptionKey, AppResources.Culture)
                                                                                                        ?? DescriptionKey;

        public ObservableCollection<MenuCategoryModel> Categories { get; set; } = [];

        public MenuCategoryModel[] SelectedCategories => Categories.Where(c => c.IsSelected).ToArray();
    }
}
