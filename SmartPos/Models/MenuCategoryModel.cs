using CommunityToolkit.Mvvm.ComponentModel;
using SmartPos.Data;
using SmartPos.Resources.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPos.Models
{
    public partial class MenuCategoryModel : ObservableObject
    {
        public int Id { get; set; }

        public string NameKey { get; set; }

        public string Icon { get; set; }

        public string DisplayName => AppResources.ResourceManager.GetString(NameKey, AppResources.Culture) ?? NameKey;

        [ObservableProperty]
        private bool _isSelected;

        public static MenuCategoryModel FromEntity(MenuCategory entity) =>
            new()
            {
                Id = entity.Id,
                NameKey = entity.NameKey,
                Icon = entity.Icon,

            };
    }
}
