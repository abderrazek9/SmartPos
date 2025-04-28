using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPos.Models
{
    public sealed class CultureChangedMessage : ValueChangedMessage<CultureInfo>
    {
        public CultureChangedMessage(CultureInfo value)
            : base(value)
        {

        }
    }
    
    
}
