using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order_Flow.App.Orders.Commands.Create_Order;
using Order_Flow.App.Orders.Queries;
using Order_Flow.App.Orders.Queries.Get_Dashboard_Orders;
using Order_Flow.App.Orders.Queries.Get_Orders;

namespace Order_Flow.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        Create_Order_Command command,
        CancellationToken cancellationToken)
    {
        var orderId = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(new
        {
            id = orderId
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new Get_Order_By_Id_Query(id),
            cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new Get_Orders_Query(),
            cancellationToken);

        return Ok(result);
    }
}