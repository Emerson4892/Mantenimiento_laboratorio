
using Mantenimiento.Application.DTOs;

namespace Mantenimiento.Application.Interfaces
{
    public interface IEquipoService
    {
        Task RegistrarEquipoAsync(
            RegistrarEquipoRequest solicitud,
            CancellationToken cancellationToken = default
        );
    }
}
