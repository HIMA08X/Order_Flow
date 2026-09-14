using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order_Flow.App.Orders.Queries.Get_Orders
{
    public record Order_List_Item_DTO(int OrderId, int CustomerId, int ItemCount, decimal Total,string Status);
}
