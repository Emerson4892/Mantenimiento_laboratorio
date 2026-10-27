
using System.ComponentModel.DataAnnotations;

namespace Mantenimiento.Application.DTOs
{
    public class ReportarFallaRequest
    {
        [Range(1, int.MaxValue,
            ErrorMessage = "Debe seleccionar un equipo válido.")]
        public int IdEquipo { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La prioridad es obligatoria.")]
        [MaxLength(20)]
        public string Prioridad { get; set; } = "Media";
    }
}
