
using System.Net.Http.Json;
using Mantenimiento.Application.DTOs;
using Mantenimiento.Application.Interfaces;

namespace Mantenimiento.Infrastructure.Services
{
    public class FallaService : IFallaService
    {
        private readonly HttpClient _httpClient;

        public FallaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task ReportarFallaAsync(
            ReportarFallaRequest solicitud,
            CancellationToken cancellationToken = default)
        {
            var datos = new
            {
                id_equipo = solicitud.IdEquipo,
                descripcion = solicitud.Descripcion,
                prioridad = solicitud.Prioridad
            };

            using var respuesta = await _httpClient.PostAsJsonAsync(
                "webhook/fallas",
                datos,
                cancellationToken
            );

            respuesta.EnsureSuccessStatusCode();
        }
    }
}
