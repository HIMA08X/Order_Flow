using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order_Flow.App.Orders.Queries
{
    public record Order_Details_DTO(int Id, int CustomerId, string Status, decimal Total, List<Order_Item_Details_DTO> Items);

    public record Order_Item_Details_DTO( string ProductName, int Quantity, decimal UnitPrice,decimal SubTotal);
}
