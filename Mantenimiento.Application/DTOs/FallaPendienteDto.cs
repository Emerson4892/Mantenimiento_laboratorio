
namespace Mantenimiento.Application.DTOs
{
    public sealed class FallaPendienteDto
    {
        public int IdFalla { get; set; }
        public string Descripcion { get; set; } = "";
        public string Prioridad { get; set; } = "";
        public string Estado { get; set; } = "";
        public string CodigoEquipo { get; set; } = "";
        public string NombreEquipo { get; set; } = "";
    }
}
