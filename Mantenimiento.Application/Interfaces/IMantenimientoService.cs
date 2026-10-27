
using Mantenimiento.Application.DTOs;

namespace Mantenimiento.Application.Interfaces
{
    public interface IMantenimientoService
    {
        Task AsignarMantenimientoAsync(
            AsignarMantenimientoRequest solicitud,
            CancellationToken cancellationToken = default
        );

        Task FinalizarMantenimientoAsync(
            FinalizarMantenimientoRequest solicitud,
            CancellationToken cancellationToken = default
        );
    }
}
