using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPos.Models
{
    public class NotificationModel
    {
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string Type { get; set; }  // "LowStock" أو "OutOfStock"
    }
}
