using Application.Common.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Common.Controllers;

[ApiController]
public abstract class ApiBaseController : ControllerBase
{
    private IMediator _mediator;

    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>()!;
}