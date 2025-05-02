using SmartPos.Resources.Strings;
using SQLite;

namespace SmartPos.Data
{
    public class MenuItem
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string NameK { get; set; }

        public string DisplayNameK =>
                                        AppResources.ResourceManager.GetString(NameK, AppResources.Culture)
                                                                                      ?? NameK;
        public string Icon { get; set; }

        public string DescriptionKey { get; set; }

        public string DisplayDescription =>
                                        AppResources.ResourceManager.GetString(DescriptionKey, AppResources.Culture)
                                                                                      ?? DescriptionKey;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; } 

       // public int LowStockThreshold { get; set; } = 4;
    }

}
