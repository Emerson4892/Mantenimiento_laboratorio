
using System.Text.Json;

namespace Mantenimiento.Application.Interfaces
{
    public interface IReporteService
    {
        Task<JsonElement> ObtenerReporteAsync(
            CancellationToken cancellationToken = default
        );
    }
}
