using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order_Flow.App.Orders.Commands.Create_Order
{
    public record Order_Item_Dto(string ProductName, int Quantity, decimal UnitPrice);
}
