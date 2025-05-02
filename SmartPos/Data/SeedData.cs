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
            new MenuCategory { Id = 1, NameKey = "Category_Basics", Icon = "basics.png" },
            new MenuCategory { Id = 2, NameKey = "Category_Drinks", Icon = "drinks.png" },
            new MenuCategory { Id = 3, NameKey = "Category_CannedFrozen", Icon = "canned_frozen.png" },
            new MenuCategory { Id = 4, NameKey = "Category_Dairy", Icon = "dairy.png" },
            new MenuCategory { Id = 5, NameKey = "Category_Snacks", Icon = "snacks.png" },
            new MenuCategory { Id = 6, NameKey = "Category_Cleaning", Icon = "cleaning.png" }
        };
    }

    public static List<MenuItem> GetMenuItems()
    {
        return new List<MenuItem>
        {  
            // 🍚 المواد الأساسية (1-5)
            new MenuItem { Id = 1, NameK = "Item_6_Name", Icon = "flour.png", DescriptionKey = "Item_6_Desc", Price = 210, StockQuantity = 30  },
            new MenuItem { Id = 2, NameK = "Item_7_Name", Icon = "rice.png", DescriptionKey = "Item_7_Desc", Price = 450, StockQuantity = 30 },
            new MenuItem { Id = 3, NameK = "Item_8_Name", Icon = "sugar.png", DescriptionKey = "Item_8_Desc", Price = 95, StockQuantity = 30  },
            new MenuItem { Id = 4, NameK = "Item_9_Name", Icon = "oil.png", DescriptionKey = "Item_9_Desc", Price = 125, StockQuantity = 30  },
            new MenuItem { Id = 5, NameK = "Item_10_Name", Icon = "semolina.png", DescriptionKey = "Item_10_Desc", Price = 320, StockQuantity = 30  },

            // 🥤 المشروبات (6-10)
            new MenuItem { Id = 6, NameK = "Item_1_Name", Icon = "water.png", DescriptionKey = "Item_1_Desc", Price = 50, StockQuantity = 30  },
            new MenuItem { Id = 7, NameK = "Item_2_Name", Icon = "soda.png", DescriptionKey = "Item_2_Desc", Price = 120, StockQuantity = 30  },
            new MenuItem { Id = 8, NameK = "Item_3_Name", Icon = "milk.png", DescriptionKey = "Item_3_Desc", Price = 75, StockQuantity = 30  },
            new MenuItem { Id = 9, NameK = "Item_4_Name", Icon = "juice.png", DescriptionKey = "Item_4_Desc", Price = 150, StockQuantity = 30  },
            new MenuItem { Id = 10, NameK = "Item_5_Name", Icon = "coffee.png", DescriptionKey = "Item_5_Desc", Price = 250, StockQuantity = 30  },

            // 🥫 المعلبات والمجمدات (11-15)
            new MenuItem { Id = 11, NameK = "Item_11_Name", Icon = "beans.png", DescriptionKey = "Item_11_Desc", Price = 230, StockQuantity = 30 },
            new MenuItem { Id = 12, NameK = "Item_12_Name", Icon = "tomato.png", DescriptionKey = "Item_12_Desc", Price = 180, StockQuantity = 30 },
            new MenuItem { Id = 13, NameK = "Item_13_Name", Icon = "sardines.png", DescriptionKey = "Item_13_Desc", Price = 320, StockQuantity = 30 },
            new MenuItem { Id = 14, NameK = "Item_14_Name", Icon = "frozen_meat.png", DescriptionKey = "Item_14_Desc", Price = 1250, StockQuantity = 30 },
            new MenuItem { Id = 15, NameK = "Item_15_Name", Icon = "frozen_chicken.png", DescriptionKey = "Item_15_Desc", Price = 300, StockQuantity = 30 },

            // 🥛 منتجات الألبان (16-20)
            new MenuItem { Id = 16, NameK = "Item_16_Name", Icon = "yogurt.png", DescriptionKey = "Item_16_Desc", Price = 100, StockQuantity = 30 },
            new MenuItem { Id = 17, NameK = "Item_17_Name", Icon = "cheese_triangles.png", DescriptionKey = "Item_17_Desc", Price = 160, StockQuantity = 30 },
            new MenuItem { Id = 18, NameK = "Item_18_Name", Icon = "butter.png", DescriptionKey = "Item_18_Desc", Price = 220, StockQuantity = 30 },
            new MenuItem { Id = 19, NameK = "Item_19_Name", Icon = "cheese.png", DescriptionKey = "Item_19_Desc", Price = 450, StockQuantity = 30 },
            new MenuItem { Id = 20, NameK = "Item_20_Name", Icon = "powdered_milk.png", DescriptionKey = "Item_20_Desc", Price = 560, StockQuantity = 30 },

            // 🍫 الوجبات الخفيفة (21-25)
            new MenuItem { Id = 21, NameK = "Item_21_Name", Icon = "biscuit.png", DescriptionKey = "Item_21_Desc", Price = 200, StockQuantity = 30 },
            new MenuItem { Id = 22, NameK = "Item_22_Name", Icon = "chocolate.png", DescriptionKey = "Item_22_Desc", Price = 250 , StockQuantity = 30},
            new MenuItem { Id = 23, NameK = "Item_23_Name", Icon = "nuts.png", DescriptionKey = "Item_23_Desc", Price = 580 , StockQuantity = 30},
            new MenuItem { Id = 24, NameK = "Item_24_Name", Icon = "chips.png", DescriptionKey = "Item_24_Desc", Price = 190 , StockQuantity = 30},
            new MenuItem { Id = 25, NameK = "Item_25_Name", Icon = "gum.png", DescriptionKey = "Item_25_Desc", Price = 75 , StockQuantity = 30},

            // 🧼 مواد التنظيف (26-30)
            new MenuItem { Id = 26, NameK = "Item_26_Name", Icon = "soap.png", DescriptionKey = "Item_26_Desc", Price = 250 , StockQuantity = 30},
            new MenuItem { Id = 27, NameK = "Item_27_Name", Icon = "detergent.png", DescriptionKey = "Item_27_Desc", Price = 575 , StockQuantity = 30},
            new MenuItem { Id = 28, NameK = "Item_28_Name", Icon = "air_freshener.png", DescriptionKey = "Item_28_Desc", Price = 350 , StockQuantity = 30},
            new MenuItem { Id = 29, NameK = "Item_29_Name", Icon = "sponge.png", DescriptionKey = "Item_29_Desc", Price = 120 , StockQuantity = 30},
            new MenuItem { Id = 30, NameK = "Item_30_Name", Icon = "tissue.png", DescriptionKey = "Item_30_Desc", Price = 180 , StockQuantity = 30}
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

