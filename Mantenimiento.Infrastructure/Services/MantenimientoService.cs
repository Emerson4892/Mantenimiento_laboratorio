
using System.Net.Http.Json;
using System.Text.Json;
using Mantenimiento.Application.DTOs;
using Mantenimiento.Application.Interfaces;

namespace Mantenimiento.Infrastructure.Services
{
    public class MantenimientoService : IMantenimientoService
    {
        // ============================================
        // 1. CONFIGURACIÓN DEL SERVICIO
        // ============================================

        private readonly HttpClient _httpClient;

        public MantenimientoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ============================================
        // 2. WF03 - ASIGNAR MANTENIMIENTO
        // ============================================

        public async Task AsignarMantenimientoAsync(
            AsignarMantenimientoRequest solicitud,
            CancellationToken cancellationToken = default)
        {
            // Preparar datos para n8n
            var datos = new
            {
                id_falla = solicitud.IdFalla,
                tecnico = solicitud.Tecnico,
                observacion = solicitud.Observacion
            };

            // Enviar solicitud a WF03
            using var respuesta = await _httpClient.PostAsJsonAsync(
                "webhook/asignaciones",
                datos,
                cancellationToken
            );

            // Comprobar respuesta HTTP
            respuesta.EnsureSuccessStatusCode();

            // Leer respuesta del workflow
            var contenido = await respuesta.Content
                .ReadAsStringAsync(cancellationToken);

            // Detectar rechazos explícitos
            ValidarRechazoExplicito(contenido);
        }

        // ============================================
        // 3. WF04 - FINALIZAR MANTENIMIENTO
        // ============================================

        public async Task FinalizarMantenimientoAsync(
            FinalizarMantenimientoRequest solicitud,
            CancellationToken cancellationToken = default)
        {
            // Preparar los datos de finalización
            var datos = new
            {
                id_mantenimiento = solicitud.IdMantenimiento,
                solucion = solicitud.Solucion
            };

            // Enviar solicitud a WF04
            using var respuesta = await _httpClient.PostAsJsonAsync(
                "webhook/finalizar-mantenimiento",
                datos,
                cancellationToken
            );

            // Comprobar respuesta HTTP
            respuesta.EnsureSuccessStatusCode();

            // Leer respuesta del workflow
            var contenido = await respuesta.Content
                .ReadAsStringAsync(cancellationToken);

            // Exigir confirmación explícita de WF04
            ValidarConfirmacionFinalizacion(contenido);
        }

        // ============================================
        // 4. VALIDAR RECHAZOS EXPLÍCITOS DE N8N
        // ============================================

        private static void ValidarRechazoExplicito(
            string contenido)
        {
            if (string.IsNullOrWhiteSpace(contenido))
            {
                return;
            }

            try
            {
                using var documento = JsonDocument.Parse(contenido);

                var raiz = documento.RootElement;

                if (raiz.ValueKind != JsonValueKind.Object)
                {
                    return;
                }

                bool Rechazado(string propiedad) =>
                    raiz.TryGetProperty(propiedad, out var valor) &&
                    valor.ValueKind == JsonValueKind.False;

                if (Rechazado("ok") ||
                    Rechazado("valido") ||
                    Rechazado("success"))
                {
                    throw new InvalidOperationException(
                        "n8n rechazó la asignación de mantenimiento."
                    );
                }
            }
            catch (JsonException)
            {
                // WF03 podría devolver texto en lugar
                // de JSON. Pendiente de unificar respuestas.
            }
        }

        // ============================================
        // 5. VALIDAR CONFIRMACIÓN DE WF04
        // ============================================

        private static void ValidarConfirmacionFinalizacion(
            string contenido)
        {
            if (string.IsNullOrWhiteSpace(contenido))
            {
                throw new InvalidOperationException(
                    "n8n no devolvió confirmación de finalización."
                );
            }

            try
            {
                using var documento = JsonDocument.Parse(contenido);

                var raiz = documento.RootElement;

                if (raiz.ValueKind != JsonValueKind.Object ||
                    !raiz.TryGetProperty("ok", out var ok) ||
                    ok.ValueKind != JsonValueKind.True)
                {
                    throw new InvalidOperationException(
                        "n8n no confirmó la finalización del mantenimiento."
                    );
                }
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    "WF04 devolvió una respuesta JSON inválida.",
                    ex
                );
            }
        }
    }
}
