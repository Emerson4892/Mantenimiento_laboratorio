
using Mantenimiento.Application.DTOs;

namespace Mantenimiento.Application.Interfaces
{
    public interface IConsultaMantenimientoService
    {
        Task<List<FallaPendienteDto>> ObtenerFallasPendientesAsync(
            CancellationToken cancellationToken = default);

        Task<List<MantenimientoAsignadoDto>> ObtenerMantenimientosAsignadosAsync(
            CancellationToken cancellationToken = default);
    }
}
