using SmartPos.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPos.Data
{
   public class DataBaseService : IAsyncDisposable
   {
        private readonly SQLiteAsyncConnection _connection;
        public DataBaseService()
        {
            var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "restpos.db3");
            _connection = new SQLiteAsyncConnection(dbpath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache);
        }

        public async Task InitializeDataBaseAsync()
        {
            await _connection.CreateTableAsync<MenuCategory>();
            await _connection.CreateTableAsync<MenuItem>();
            await _connection.CreateTableAsync<MenuItemCategoryMapping>();
            await _connection.CreateTableAsync<Order>();
            await _connection.CreateTableAsync<OrdersItem>();

            await SeedDataAsync();
        }
        private async Task SeedDataAsync()
        {
            var firstCategory = await _connection.Table<MenuCategory>().FirstOrDefaultAsync();

            if (firstCategory != null)
                return; // data base seeded

            var categories = SeedData.GetMenuCategories();
            var menuItem = SeedData.GetMenuItems();
            var mapping = SeedData.GetMenuItemCategoryMappings();

            await _connection.InsertAllAsync(categories);
            await _connection.InsertAllAsync(menuItem);
            await _connection.InsertAllAsync(mapping);
        }


        public async Task <MenuCategory[]> GetMenuCategoriesAsync() => await _connection.Table<MenuCategory>().ToArrayAsync();

        public async Task<MenuItem[]> GetMenuItemsByCategoryAsync(int CategoryId)
        {
            var query = @"
                         SELECT menu.*
                         FROM MenuItem AS menu 
                         INNER JOIN MenuItemCategoryMapping AS mapping
                                    ON menu.Id = mapping.MenuItemId
                         WHERE mapping.MenuCategoryId = ?
                         ";
            var menuItems = await _connection.QueryAsync<MenuItem>(query, CategoryId);
            return [.. menuItems];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Returns Error message or null (if the opeation was succesfully)</returns>
        public async Task <string?> PlaceOrderAsync(OrderModel model)
        {
            var order = new Order
            {
                OrderDate = model.OrderDate,
                PaymentMode = model.PaymentMode,
                TotalAmountPaid = model.TotalAmountPaid,
                TotalItemsCount = model.TotalItemsCount,
            };
           
            if (await _connection.InsertAsync(order) > 0)
            {
                // Order Insert succesfully 
                // Now we have newly inserted order id in order.Id
                // we can add the orderId to the OrderItems and Insert the OrderItems in the database
                foreach (var item in model.Items )
                {
                    item.OrderId = order.Id;
                }
                if (await _connection.InsertAllAsync(model.Items) ==0 )
                {
                    // OrderItems insert operation failed
                    // Remove the newly inserted order in the method

                    await _connection.DeleteAsync(order);
                    return "Error in the inserting order items";
                }
            }
            else
            {
                return "Error in the inserting order";
            }
            model.Id = order.Id;
            return null;
        }

        public async Task<Order[]> GetOrdersAsync() =>
            await _connection.Table<Order>().ToArrayAsync();

        public async Task<OrdersItem[]> GetOrdersItemsAsync(long orderId) =>
            await _connection.Table<OrdersItem>().Where(oi => oi.OrderId == orderId).ToArrayAsync();

        public async Task<MenuCategory[]> GetCategoriesOfMenuItem(int menuItemId)
        {
            var query = @"
                SELECT cat.* 
                FROM MenuCategory cat
                INNER JOIN MenuItemCategoryMapping map
                ON cat.Id = map.MenuCategoryId
                WHERE map.MenuItemId = ?
            ";

            var categories = await _connection.QueryAsync<MenuCategory>(query, menuItemId);
            return [.. categories];
        }

        public async Task<string?> SaveMenuItemAsync(MenuItemModel model)
        {
            if(model.Id == 0)
            {
                // Creating a new menu item

                MenuItem menuItem = new()
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    Icon = model.Icon,
                    Price = model.Price,
                    StockQuantity = model.StockQuantity,
                   // LowStockThreshold = model.LowStockThreshold
                };

                if(await _connection.InsertAsync(menuItem) > 0)
                {
                    var categoryMapping = model.SelectedCategories
                        .Select(c => new MenuItemCategoryMapping
                        {
                            Id = c.Id,
                            MenuCategoryId = c.Id,
                            MenuItemId = menuItem.Id
                        });

                    if(await _connection.InsertAllAsync(categoryMapping) > 0)
                    {
                        model.Id = menuItem.Id;
                        return null;
                    }
                    else
                    {
                        // Menu Item Insert was Succesfull
                        // but Category Mapping Insert operation failed
                        // we should remove the newly inserted menu item from the database
                        await _connection.DeleteAsync(menuItem);
                    }

                }
                return "Error in saving menu item";

            }
            else
            {
                // Updatind an Existing menu item 

                string? errorMessage = null;

                await _connection.RunInTransactionAsync(db => 
                {
                    var menuItem = db.Find<MenuItem>(model.Id);

                    //if (menuItem == null)
                    //{
                    //    errorMessage = "Menu Item not found in database";
                    //    throw new Exception(); // stop the operation
                    //}

                    menuItem.Name = model.Name;
                    menuItem.Description = model.Description;
                    menuItem.Icon = model.Icon;
                    menuItem.Price = model.Price;

                    menuItem.StockQuantity = model.StockQuantity;
                   // menuItem.LowStockThreshold = model.LowStockThreshold;

                    if (db.Update(menuItem) == 0)
                    {
                        // this operation failled
                        // return error message
                        errorMessage = "Error in Updating menu item";
                        throw new Exception(); // to trigger rollback
                    }

                    var deleteQuery = @"
                                        DELETE FROM MenuItemCategoryMapping
                                        WHERE MenuItemId = ?";
                    db.Execute(deleteQuery, menuItem.Id);

                    var categoryMapping = model.SelectedCategories
                                          .Select(c => new MenuItemCategoryMapping
                                          {
                                            // Id = c.Id,
                                             MenuCategoryId = c.Id,
                                             MenuItemId = menuItem.Id
                                          });

                    //if (categoryMapping.Any() && db.InsertAll(categoryMapping) == 0)
                    //{
                    //    errorMessage = "Error in Updating menu item ";
                    //    throw new Exception();
                    //}
                    if (db.InsertAll(categoryMapping) == 0)
                    {
                        // this operation failled
                        // return error message
                        errorMessage = "Error in Updating menu item";
                        throw new Exception(); // to trigger rollback
                    }
                    
                });

                return errorMessage;
            }
        }

        /// <summary>
        /// تقليل المخزون لكل بند في الطلب
        /// </summary>
        public async Task ReduceStockAsync(OrdersItem[] items)
        {
            foreach (var oi in items)
            {
                var mi = await _connection.FindAsync<MenuItem>(oi.ItemId);
                if (mi != null)
                {
                    mi.StockQuantity = Math.Max(0, mi.StockQuantity - oi.Quantity);
                    await _connection.UpdateAsync(mi);
                }
            }
        }

        /// <summary>
        /// استرجاع كل الأصناف للتأكد من مستوى المخزون
        /// </summary>
        public async Task<MenuItem[]> GetAllMenuItemsAsync() =>
            await _connection.Table<MenuItem>().ToArrayAsync();

        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
                await _connection.CloseAsync();
        }


    }
}
