using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order_Flow.App.Orders.Queries.Get_Dashboard_Orders;

namespace Order_Flow.API.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new Get_Dashboard_Orders_Query(),
            cancellationToken);

        return Ok(result);
    }
}