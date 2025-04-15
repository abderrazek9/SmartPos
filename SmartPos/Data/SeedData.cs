using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPos.Data;
public static class SeedData
{
    public static List<MenuCategory> GetMenuCategories()
    {
        return new List<MenuCategory>
        {
            new MenuCategory { Id = 1, Name = "المشروبات", Icon = "drinks.png" },
            new MenuCategory { Id = 2, Name = "المواد الأساسية", Icon = "basics.png" },
            new MenuCategory { Id = 3, Name = "المعلبات والمجمدات", Icon = "canned_frozen.png" },
            new MenuCategory { Id = 4, Name = "منتجات الألبان", Icon = "dairy.png" },
            new MenuCategory { Id = 5, Name = "الوجبات الخفيفة", Icon = "snacks.png" },
            new MenuCategory { Id = 6, Name = "مواد التنظيف", Icon = "cleaning.png" }
        };
    }

    public static List<MenuItem> GetMenuItems()
    {
        return new List<MenuItem>
        {
            // 🥤 المشروبات (1-5)
            new MenuItem { Id = 1, Name = "ماء معدني", Icon = "water.png", Description = "قارورة ماء 1.5 لتر", Price = 50  },
            new MenuItem { Id = 2, Name = "مشروب غازي", Icon = "soda.png", Description = "عبوة 1 لتر", Price = 120  },
            new MenuItem { Id = 3, Name = "حليب معقم", Icon = "milk.png", Description = "لتر حليب كامل الدسم", Price = 75  },
            new MenuItem { Id = 4, Name = "عصير طبيعي", Icon = "juice.png", Description = "عبوة 1 لتر", Price = 150  },
            new MenuItem { Id = 5, Name = "قهوة ", Icon = "coffee.png", Description = "علبة 200 جرام", Price = 250  },

            // 🍚 المواد الأساسية (6-10)
            new MenuItem { Id = 6, Name = "طحين القمح", Icon = "flour.png", Description = "كيس 2 كجم", Price = 210  },
            new MenuItem { Id = 7, Name = "أرز ", Icon = "rice.png", Description = "كيس 1 كجم", Price = 450 },
            new MenuItem { Id = 8, Name = "سكر أبيض", Icon = "sugar.png", Description = "كيس 1 كجم", Price = 95  },
            new MenuItem { Id = 9, Name = "زيت نباتي", Icon = "oil.png", Description = "عبوة 1 لتر", Price = 125  },
            new MenuItem { Id = 10, Name = "سميد", Icon = "semolina.png", Description = "كيس 2 كجم", Price = 320  },

            // 🥫 المعلبات والمجمدات (11-15)
            new MenuItem { Id = 11, Name = "فول معلب", Icon = "beans.png", Description = "علبة 400 جرام", Price = 230 },
            new MenuItem { Id = 12, Name = "طماطم مصبرة", Icon = "tomato.png", Description = "علبة 400 جرام", Price = 180 },
            new MenuItem { Id = 13, Name = "سردين معلب", Icon = "sardines.png", Description = "علبة 125 جرام", Price = 320 },
            new MenuItem { Id = 14, Name = "لحم مجمد", Icon = "frozen_meat.png", Description = "كيلو لحم مستورد", Price = 1250 },
            new MenuItem { Id = 15, Name = "دجاج مجمد", Icon = "frozen_chicken.png", Description = "دجاجة كاملة 1.2 كجم", Price = 300 },

            // 🥛 منتجات الألبان (16-20)
            new MenuItem { Id = 16, Name = "لبن طبيعي", Icon = "yogurt.png", Description = "علبة 500 جرام", Price = 100 },
            new MenuItem { Id = 17, Name = "جبن مثلثات", Icon = "cheese_triangles.png", Description = "علبة 8 قطع", Price = 160 },
            new MenuItem { Id = 18, Name = "زبدة", Icon = "butter.png", Description = "عبوة 250 جرام", Price = 220 },
            new MenuItem { Id = 19, Name = "جبن رومي", Icon = "cheese.png", Description = "200 جرام", Price = 450 },
            new MenuItem { Id = 20, Name = "حليب بودرة", Icon = "powdered_milk.png", Description = "علبة 500 جرام", Price = 560 },

            // 🍫 الوجبات الخفيفة (21-25)
            new MenuItem { Id = 21, Name = "بسكويت", Icon = "biscuit.png", Description = "علبة 300 جرام", Price = 200 },
            new MenuItem { Id = 22, Name = "شوكولاتة", Icon = "chocolate.png", Description = "لوح 100 جرام", Price = 250 },
            new MenuItem { Id = 23, Name = "مكسرات", Icon = "nuts.png", Description = "كيس 250 جرام", Price = 580 },
            new MenuItem { Id = 24, Name = "رقائق البطاطس", Icon = "chips.png", Description = "كيس 150 جرام", Price = 190 },
            new MenuItem { Id = 25, Name = "علكة", Icon = "gum.png", Description = "عبوة 10 قطع", Price = 75 },

            // 🧼 مواد التنظيف (26-30)
            new MenuItem { Id = 26, Name = "صابون سائل", Icon = "soap.png", Description = "عبوة 1 لتر", Price = 250 },
            new MenuItem { Id = 27, Name = "مسحوق غسيل", Icon = "detergent.png", Description = "كيس 3 كجم", Price = 575 },
            new MenuItem { Id = 28, Name = "معطر جو", Icon = "air_freshener.png", Description = "عبوة 300 مل", Price = 350 },
            new MenuItem { Id = 29, Name = "إسفنج أطباق", Icon = "sponge.png", Description = "عبوة 5 قطع", Price = 120 },
            new MenuItem { Id = 30, Name = "مناديل ورقية", Icon = "tissue.png", Description = "رزمة 150 ورقة", Price = 180 }
        };
    }

    public static List<MenuItemCategoryMapping> GetMenuItemCategoryMappings()
    {
        return new List<MenuItemCategoryMapping>
        {
            new MenuItemCategoryMapping { MenuItemId = 1, MenuCategoryId = 1 },
            new MenuItemCategoryMapping { MenuItemId = 2, MenuCategoryId = 1 },
            new MenuItemCategoryMapping { MenuItemId = 3, MenuCategoryId = 1 },
            new MenuItemCategoryMapping { MenuItemId = 4, MenuCategoryId = 1 },
            new MenuItemCategoryMapping { MenuItemId = 5, MenuCategoryId = 1 },
            new MenuItemCategoryMapping { MenuItemId = 6, MenuCategoryId = 2 },
            new MenuItemCategoryMapping { MenuItemId = 7, MenuCategoryId = 2 },
            new MenuItemCategoryMapping { MenuItemId = 8, MenuCategoryId = 2 },
            new MenuItemCategoryMapping { MenuItemId = 9, MenuCategoryId = 2 },
            new MenuItemCategoryMapping { MenuItemId = 10, MenuCategoryId = 2 },
            new MenuItemCategoryMapping { MenuItemId = 11, MenuCategoryId = 3 },
            new MenuItemCategoryMapping { MenuItemId = 12, MenuCategoryId = 3 },
            new MenuItemCategoryMapping { MenuItemId = 13, MenuCategoryId = 3 },
            new MenuItemCategoryMapping { MenuItemId = 14, MenuCategoryId = 3 },
            new MenuItemCategoryMapping { MenuItemId = 15, MenuCategoryId = 3 },
            new MenuItemCategoryMapping { MenuItemId = 16, MenuCategoryId = 4 },
            new MenuItemCategoryMapping { MenuItemId = 17, MenuCategoryId = 4 },
            new MenuItemCategoryMapping { MenuItemId = 18, MenuCategoryId = 4 },
            new MenuItemCategoryMapping { MenuItemId = 19, MenuCategoryId = 4 },
            new MenuItemCategoryMapping { MenuItemId = 20, MenuCategoryId = 4 },
            new MenuItemCategoryMapping { MenuItemId = 21, MenuCategoryId = 5 },
            new MenuItemCategoryMapping { MenuItemId = 22, MenuCategoryId = 5 },
            new MenuItemCategoryMapping { MenuItemId = 23, MenuCategoryId = 5 },
            new MenuItemCategoryMapping { MenuItemId = 24, MenuCategoryId = 5 },
            new MenuItemCategoryMapping { MenuItemId = 25, MenuCategoryId = 5 },
            new MenuItemCategoryMapping { MenuItemId = 26, MenuCategoryId = 6 },
            new MenuItemCategoryMapping { MenuItemId = 27, MenuCategoryId = 6 },
            new MenuItemCategoryMapping { MenuItemId = 28, MenuCategoryId = 6 },
            new MenuItemCategoryMapping { MenuItemId = 29, MenuCategoryId = 6 },
            new MenuItemCategoryMapping { MenuItemId = 30, MenuCategoryId = 6 }
        };
    }
}

