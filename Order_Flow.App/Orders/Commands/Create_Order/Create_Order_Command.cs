using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Order_Flow.App.Orders.Commands.Create_Order
{
    public record Create_Order_Command(int CustomerId, List<Order_Item_Dto> Items) : IRequest<int>;
    
}
