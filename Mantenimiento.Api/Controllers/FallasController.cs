
using Microsoft.AspNetCore.Mvc;
using Mantenimiento.Application.DTOs;
using Mantenimiento.Application.Interfaces;

namespace Mantenimiento.Api.Controllers
{
    [ApiController]
    [Route("api/fallas")]
    public class FallasController : ControllerBase
    {
        private readonly IFallaService _fallaService;

        public FallasController(IFallaService fallaService)
        {
            _fallaService = fallaService;
        }

        [HttpPost]
        public async Task<IActionResult> ReportarFalla(
            [FromBody] ReportarFallaRequest solicitud,
            CancellationToken cancellationToken)
        {
            try
            {
                await _fallaService.ReportarFallaAsync(
                    solicitud,
                    cancellationToken
                );

                return Ok(new
                {
                    ok = true,
                    mensaje = "Solicitud de falla procesada por n8n."
                });
            }
            catch (HttpRequestException)
            {
                return StatusCode(502, new
                {
                    ok = false,
                    mensaje = "Error al comunicarse con el workflow de fallas."
                });
            }
            catch (TaskCanceledException)
                when (!cancellationToken.IsCancellationRequested)
            {
                return StatusCode(504, new
                {
                    ok = false,
                    mensaje = "Tiempo de espera agotado."
                });
            }
        }
    }
}
