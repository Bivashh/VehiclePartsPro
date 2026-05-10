using Microsoft.AspNetCore.Mvc;

namespace VehiclePartsPro.Controllers;

[ApiController]
[Route("api/placeholder")]
public class PlaceholderController : ControllerBase
{
    [HttpGet]
    public IActionResult Test()
    {
        return Ok("Controller working");
    }
}