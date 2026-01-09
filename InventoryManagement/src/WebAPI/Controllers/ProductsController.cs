using Application.UseCases.Products.Commands;
using Application.UseCases.Products.Queries;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Common;

namespace WebAPI.Controllers;

[Route("api/products")]
public class ProductsController : ApiBaseController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductCreateCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.SendAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.SendAsync(new ProductGetByIdQuery { Id = id }, cancellationToken);
        
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await Mediator.SendAsync(new ProductGetAllQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductUpdateStatusCommand command, CancellationToken cancellationToken)
    {
        command.SetId(id);

        var result = await Mediator.SendAsync(command, cancellationToken);
        return Ok(result);
    }
}
