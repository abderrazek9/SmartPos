using SQLite;

namespace SmartPos.Data
{
    public class MenuItem
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; } 

       // public int LowStockThreshold { get; set; } = 4;
    }

}
