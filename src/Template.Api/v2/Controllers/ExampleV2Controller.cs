using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Template.Application.Resources;
using Template.Domain.Interfaces;

namespace Template.Api.v2.Controllers;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{v:apiVersion}/example")]
public class ExampleV2Controller : ControllerBase
{
    private readonly IGlobalizer _strings;
    public ExampleV2Controller(IGlobalizer strings)
    {
        _strings = strings;
    }

    [HttpGet("")]
    public ActionResult<string> Index()
        => Ok(_strings["Agora só em português"]);

}
