
using System.Net.Http;
using System.Text.Json;
using Mantenimiento.Application.Interfaces;

namespace Mantenimiento.Infrastructure.Services
{
    public class ReporteService : IReporteService
    {
        private readonly HttpClient _httpClient;

        public ReporteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<JsonElement> ObtenerReporteAsync(
            CancellationToken cancellationToken = default)
        {
            // WF06 - Reporte de mantenimiento
            using var respuesta = await _httpClient.GetAsync(
                "webhook/reporte-mantenimiento",
                cancellationToken
            );

            // Detectar errores HTTP de n8n
            respuesta.EnsureSuccessStatusCode();

            // Leer respuesta real
            var contenido = await respuesta.Content
                .ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(contenido))
            {
                throw new InvalidOperationException(
                    "WF06 devolvió una respuesta vacía."
                );
            }

            try
            {
                using var documento = JsonDocument.Parse(contenido);

                var raiz = documento.RootElement;

                // Detectar rechazo explícito de n8n
                if (raiz.ValueKind == JsonValueKind.Object &&
                    raiz.TryGetProperty("ok", out var ok) &&
                    ok.ValueKind == JsonValueKind.False)
                {
                    throw new InvalidOperationException(
                        "WF06 indicó que no pudo generar el reporte."
                    );
                }

                // Conservar el JSON fuera del documento
                return raiz.Clone();
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    "WF06 no devolvió un JSON válido.",
                    ex
                );
            }
        }
    }
}
