
namespace Mantenimiento.Application.DTOs
{
    public sealed class MantenimientoAsignadoDto
    {
        public int IdMantenimiento { get; set; }
        public string Tecnico { get; set; } = "";
        public string Estado { get; set; } = "";
        public string DescripcionFalla { get; set; } = "";
        public string NombreEquipo { get; set; } = "";
    }
}
