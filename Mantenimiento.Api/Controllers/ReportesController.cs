
using Microsoft.AspNetCore.Mvc;
using Mantenimiento.Application.Interfaces;

namespace Mantenimiento.Api.Controllers
{
    [ApiController]
    [Route("api/reportes")]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteService _reporteService;

        public ReportesController(
            IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        // ============================================
        // WF06 - CONSULTAR REPORTE DE MANTENIMIENTO
        // GET /api/reportes/mantenimientos
        // ============================================

        [HttpGet("mantenimientos")]
        public async Task<IActionResult> ObtenerReporte(
            CancellationToken cancellationToken)
        {
            try
            {
                var reporte = await _reporteService
                    .ObtenerReporteAsync(cancellationToken);

                return Ok(reporte);
            }
            catch (HttpRequestException)
            {
                return StatusCode(502, new
                {
                    ok = false,
                    mensaje = "Error al comunicarse con WF06."
                });
            }
            catch (InvalidOperationException)
            {
                return StatusCode(502, new
                {
                    ok = false,
                    mensaje = "WF06 no devolvió un reporte válido."
                });
            }
            catch (TaskCanceledException)
                when (!cancellationToken.IsCancellationRequested)
            {
                return StatusCode(504, new
                {
                    ok = false,
                    mensaje = "Se agotó el tiempo de espera de WF06."
                });
            }
        }
    }
}
