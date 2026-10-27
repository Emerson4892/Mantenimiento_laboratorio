
using Microsoft.Extensions.Configuration;
using Npgsql;
using Mantenimiento.Application.DTOs;
using Mantenimiento.Application.Interfaces;

namespace Mantenimiento.Infrastructure.Services
{
    public sealed class ConsultaMantenimientoService
        : IConsultaMantenimientoService
    {
        private readonly string _connectionString;

        public ConsultaMantenimientoService(
            IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("MantenimientoDb")
                ?? throw new InvalidOperationException(
                    "Falta configurar la conexión MantenimientoDb.");
        }

        // ==========================================
        // 1. CONSULTAR FALLAS SIN ASIGNACIÓN
        // ==========================================

        public async Task<List<FallaPendienteDto>>
            ObtenerFallasPendientesAsync(
                CancellationToken cancellationToken = default)
        {
            var resultados = new List<FallaPendienteDto>();

            const string sql = """
                SELECT
                    f.id_falla,
                    f.descripcion,
                    f.prioridad,
                    f.estado,
                    e.codigo,
                    e.nombre
                FROM fallas f
                INNER JOIN equipos e
                    ON f.id_equipo = e.id_equipo
                WHERE f.estado = 'Pendiente'
                  AND NOT EXISTS (
                      SELECT 1
                      FROM mantenimientos m
                      WHERE m.id_falla = f.id_falla
                  )
                ORDER BY f.id_falla DESC;
                """;

            await using var conexion =
                new NpgsqlConnection(_connectionString);

            await conexion.OpenAsync(cancellationToken);

            await using var comando =
                new NpgsqlCommand(sql, conexion);

            await using var lector =
                await comando.ExecuteReaderAsync(cancellationToken);

            while (await lector.ReadAsync(cancellationToken))
            {
                resultados.Add(new FallaPendienteDto
                {
                    IdFalla = lector.GetInt32(0),
                    Descripcion = lector.GetString(1),
                    Prioridad = lector.GetString(2),
                    Estado = lector.GetString(3),
                    CodigoEquipo = lector.GetString(4),
                    NombreEquipo = lector.GetString(5)
                });
            }

            return resultados;
        }

        // ==========================================
        // 2. CONSULTAR MANTENIMIENTOS ASIGNADOS
        // ==========================================

        public async Task<List<MantenimientoAsignadoDto>>
            ObtenerMantenimientosAsignadosAsync(
                CancellationToken cancellationToken = default)
        {
            var resultados = new List<MantenimientoAsignadoDto>();

            const string sql = """
                SELECT
                    m.id_mantenimiento,
                    m.tecnico,
                    m.estado,
                    f.descripcion,
                    e.nombre
                FROM mantenimientos m
                INNER JOIN fallas f
                    ON m.id_falla = f.id_falla
                INNER JOIN equipos e
                    ON f.id_equipo = e.id_equipo
                WHERE m.estado = 'Asignado'
                ORDER BY m.id_mantenimiento DESC;
                """;

            await using var conexion =
                new NpgsqlConnection(_connectionString);

            await conexion.OpenAsync(cancellationToken);

            await using var comando =
                new NpgsqlCommand(sql, conexion);

            await using var lector =
                await comando.ExecuteReaderAsync(cancellationToken);

            while (await lector.ReadAsync(cancellationToken))
            {
                resultados.Add(new MantenimientoAsignadoDto
                {
                    IdMantenimiento = lector.GetInt32(0),
                    Tecnico = lector.GetString(1),
                    Estado = lector.GetString(2),
                    DescripcionFalla = lector.GetString(3),
                    NombreEquipo = lector.GetString(4)
                });
            }

            return resultados;
        }
    }
}
