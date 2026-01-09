using Application.UseCases.Categories.Commands;
using Application.UseCases.Categories.Models;
using Application.UseCases.Categories.Queries;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Common;

namespace WebAPI.Controllers;

[Route("api/categories")]
public class CategoriesController : ApiBaseController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoryCreateCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.SendAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await Mediator.SendAsync(new CategoryGetAllQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.SendAsync(new CategoryGetByIdQuery { Id = id }, cancellationToken);
        
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.SendAsync(new CategoryDeleteCommand { Id = id }, cancellationToken);
        
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}
