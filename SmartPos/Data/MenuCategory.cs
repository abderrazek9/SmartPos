using SmartPos.Models;
using SQLite;

namespace SmartPos.Data
{
    public class MenuCategory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string NameKey { get; set; }

        public string Icon { get; set; }


    }

}
