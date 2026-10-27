
using System.Net.Http.Json;
using System.Text.Json;
using Mantenimiento.Application.DTOs;
using Mantenimiento.Application.Interfaces;

namespace Mantenimiento.Infrastructure.Services
{
    public class EquipoService : IEquipoService
    {
        private readonly HttpClient _httpClient;

        public EquipoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task RegistrarEquipoAsync(
            RegistrarEquipoRequest solicitud,
            CancellationToken cancellationToken = default)
        {
            // Convertimos los datos al formato que
            // utilizaremos para comunicarnos con n8n.
            var datos = new
            {
                codigo = solicitud.Codigo,
                nombre = solicitud.Nombre,
                tipo = solicitud.Tipo,
                marca = solicitud.Marca,
                modelo = solicitud.Modelo,
                numero_serie = solicitud.NumeroSerie,
                ubicacion = solicitud.Ubicacion
            };

            using var respuesta = await _httpClient.PostAsJsonAsync(
                "webhook/equipos",
                datos,
                cancellationToken
            );

            // Un HTTP 200 no siempre garantiza que
            // la operación de negocio se completó.
            if (!respuesta.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Error de comunicación con n8n: {(int)respuesta.StatusCode}"
                );
            }

            var contenido = await respuesta.Content
                .ReadAsStringAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(contenido))
            {
                try
                {
                    using var documento = JsonDocument.Parse(contenido);

                    if (documento.RootElement.ValueKind ==
                        JsonValueKind.Object &&
                        documento.RootElement.TryGetProperty(
                            "ok", out var resultado) &&
                        resultado.ValueKind == JsonValueKind.False)
                    {
                        throw new InvalidOperationException(
                            "n8n indicó que no pudo registrar el equipo."
                        );
                    }
                }
                catch (JsonException)
                {
                    // Algunos workflows devuelven texto
                    // en lugar de JSON. Verificaremos
                    // después el contrato exacto de respuesta.
                }
            }
        }
    }
}
