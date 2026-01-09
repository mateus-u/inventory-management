using Application.UseCases.Suppliers.Commands;
using Application.UseCases.Suppliers.Models;
using Application.UseCases.Suppliers.Queries;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Common;

namespace WebAPI.Controllers;

[Route("api/suppliers")]
public class SuppliersController : ApiBaseController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SupplierCreateCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.SendAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SupplierResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.SendAsync(new SupplierGetByIdQuery { Id = id }, cancellationToken);
        
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SupplierResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await Mediator.SendAsync(new SupplierGetAllQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.SendAsync(new SupplierDeleteCommand { Id = id }, cancellationToken);
        
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}
