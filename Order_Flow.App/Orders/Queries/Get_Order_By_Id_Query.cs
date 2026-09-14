using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Order_Flow.App.Orders.Queries
{
    public record Get_Order_By_Id_Query(int OrderId) : IRequest<Order_Details_DTO>;
}
