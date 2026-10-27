
using Microsoft.AspNetCore.Mvc;
using Mantenimiento.Application.Interfaces;

namespace Mantenimiento.Api.Controllers
{
    [ApiController]
    public class ConsultasController : ControllerBase
    {
        private readonly IConsultaMantenimientoService _consultas;

        public ConsultasController(
            IConsultaMantenimientoService consultas)
        {
            _consultas = consultas;
        }

        // ==========================================
        // 1. CONSULTAR FALLAS PENDIENTES
        // GET: /api/fallas/pendientes
        // ==========================================

        [HttpGet("/api/fallas/pendientes")]
        public async Task<IActionResult> ObtenerFallasPendientes(
            CancellationToken cancellationToken)
        {
            var fallas = await _consultas
                .ObtenerFallasPendientesAsync(cancellationToken);

            return Ok(fallas);
        }

        // ==========================================
        // 2. CONSULTAR MANTENIMIENTOS ASIGNADOS
        // GET: /api/mantenimientos/asignados
        // ==========================================

        [HttpGet("/api/mantenimientos/asignados")]
        public async Task<IActionResult> ObtenerMantenimientosAsignados(
            CancellationToken cancellationToken)
        {
            var mantenimientos = await _consultas
                .ObtenerMantenimientosAsignadosAsync(cancellationToken);

            return Ok(mantenimientos);
        }
    }
}
