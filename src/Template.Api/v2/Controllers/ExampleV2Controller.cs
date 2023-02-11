using Microsoft.AspNetCore.Mvc;
using Template.Domain.Interfaces;

namespace Template.Api.v2.Controllers;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{v:apiVersion}/example")]
public class ExampleV2Controller : ControllerBase
{
    private readonly IGlobalizer _g;

    public ExampleV2Controller(IGlobalizer g)
    {
        _g = g;
    }

    [HttpGet("")]
    public ActionResult<string> Index()
        => Ok(_g["This is the API Version 2"]);
}
