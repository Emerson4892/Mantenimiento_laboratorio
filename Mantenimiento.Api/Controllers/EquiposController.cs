
using Microsoft.AspNetCore.Mvc;
using Mantenimiento.Application.DTOs;
using Mantenimiento.Application.Interfaces;

namespace Mantenimiento.Api.Controllers
{
    [ApiController]
    [Route("api/equipos")]
    public class EquiposController : ControllerBase
    {
        private readonly IEquipoService _equipoService;

        public EquiposController(IEquipoService equipoService)
        {
            _equipoService = equipoService;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarEquipo(
            [FromBody] RegistrarEquipoRequest solicitud,
            CancellationToken cancellationToken)
        {
            try
            {
                await _equipoService.RegistrarEquipoAsync(
                    solicitud,
                    cancellationToken
                );

                return Ok(new
                {
                    ok = true,
                    mensaje = "Solicitud de registro procesada por n8n."
                });
            }
            catch (HttpRequestException)
            {
                return StatusCode(502, new
                {
                    ok = false,
                    mensaje = "No fue posible comunicarse correctamente con n8n."
                });
            }
            catch (InvalidOperationException)
            {
                return StatusCode(502, new
                {
                    ok = false,
                    mensaje = "n8n no confirmó el registro del equipo."
                });
            }
            catch (TaskCanceledException)
                when (!cancellationToken.IsCancellationRequested)
            {
                return StatusCode(504, new
                {
                    ok = false,
                    mensaje = "Se agotó el tiempo de respuesta de n8n."
                });
            }
        }
    }
}
