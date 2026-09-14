using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Order_Flow.App.Orders.Queries.Get_Orders
{
    public record Get_Orders_Query() : IRequest<List<Order_List_Item_DTO>>;
}
