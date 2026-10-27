using Microsoft.AspNetCore.Mvc;

namespace Mantenimiento.Api.Controllers
{
    [ApiController]
    [Route("api/sistema")]
    public class SistemaController : ControllerBase
    {
        [HttpGet("estado")]
        public IActionResult ObtenerEstado()
        {
            return Ok(new
            {
                ok = true,
                mensaje = "API de mantenimiento funcionando",
                version = "1.0"
            });
        }
    }
}
