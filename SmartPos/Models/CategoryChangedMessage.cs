using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Threading.Tasks;

namespace SmartPos.Models
{
    public class CategoryChangedMessage : ValueChangedMessage<MenuCategoryModel>
    {
        public CategoryChangedMessage(MenuCategoryModel value) : base(value) { }
        public static CategoryChangedMessage From(MenuCategoryModel value) => new(value);
    }
}
