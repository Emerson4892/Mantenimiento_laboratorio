
using Microsoft.AspNetCore.Mvc;
using Mantenimiento.Application.DTOs;
using Mantenimiento.Application.Interfaces;

namespace Mantenimiento.Api.Controllers
{
    [ApiController]
    [Route("api/mantenimientos")]
    public class MantenimientosController : ControllerBase
    {
        // ============================================
        // 1. SERVICIO DE MANTENIMIENTO
        // ============================================

        private readonly IMantenimientoService _mantenimientoService;

        public MantenimientosController(
            IMantenimientoService mantenimientoService)
        {
            _mantenimientoService = mantenimientoService;
        }

        // ============================================
        // 2. WF03 - ASIGNAR MANTENIMIENTO
        // POST: /api/mantenimientos/asignar
        // ============================================

        [HttpPost("asignar")]
        public async Task<IActionResult> Asignar(
            [FromBody] AsignarMantenimientoRequest solicitud,
            CancellationToken cancellationToken)
        {
            try
            {
                await _mantenimientoService.AsignarMantenimientoAsync(
                    solicitud,
                    cancellationToken
                );

                return Accepted(new
                {
                    ok = true,
                    mensaje = "Solicitud procesada por WF03. Verifique el registro en PostgreSQL."
                });
            }
            catch (HttpRequestException)
            {
                return StatusCode(502, new
                {
                    ok = false,
                    mensaje = "Error de comunicación con WF03."
                });
            }
            catch (InvalidOperationException)
            {
                return StatusCode(502, new
                {
                    ok = false,
                    mensaje = "WF03 rechazó la asignación."
                });
            }
            catch (TaskCanceledException)
                when (!cancellationToken.IsCancellationRequested)
            {
                return StatusCode(504, new
                {
                    ok = false,
                    mensaje = "Tiempo de espera agotado en WF03."
                });
            }
        }

        // ============================================
        // 3. WF04 - FINALIZAR MANTENIMIENTO
        // POST: /api/mantenimientos/finalizar
        // ============================================

        [HttpPost("finalizar")]
        public async Task<IActionResult> Finalizar(
            [FromBody] FinalizarMantenimientoRequest solicitud,
            CancellationToken cancellationToken)
        {
            try
            {
                await _mantenimientoService.FinalizarMantenimientoAsync(
                    solicitud,
                    cancellationToken
                );

                return Ok(new
                {
                    ok = true,
                    mensaje = "WF04 confirmó la finalización del mantenimiento."
                });
            }
            catch (HttpRequestException)
            {
                return StatusCode(502, new
                {
                    ok = false,
                    mensaje = "Error de comunicación con WF04."
                });
            }
            catch (InvalidOperationException)
            {
                return StatusCode(502, new
                {
                    ok = false,
                    mensaje = "WF04 no confirmó la finalización."
                });
            }
            catch (TaskCanceledException)
                when (!cancellationToken.IsCancellationRequested)
            {
                return StatusCode(504, new
                {
                    ok = false,
                    mensaje = "Tiempo de espera agotado en WF04."
                });
            }
        }
    }
}
