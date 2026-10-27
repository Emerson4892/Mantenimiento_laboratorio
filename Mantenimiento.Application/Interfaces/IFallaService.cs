
using Mantenimiento.Application.DTOs;

namespace Mantenimiento.Application.Interfaces
{
    public interface IFallaService
    {
        Task ReportarFallaAsync(
            ReportarFallaRequest solicitud,
            CancellationToken cancellationToken = default
        );
    }
}
