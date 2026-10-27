
using System.ComponentModel.DataAnnotations;

namespace Mantenimiento.Application.DTOs
{
    public class AsignarMantenimientoRequest
    {
        [Range(1, int.MaxValue,
            ErrorMessage = "Debe indicar una falla válida.")]
        public int IdFalla { get; set; }

        [Required(ErrorMessage = "El técnico es obligatorio.")]
        [MaxLength(150,
            ErrorMessage = "El nombre del técnico no puede superar 150 caracteres.")]
        public string Tecnico { get; set; } = string.Empty;

        public string? Observacion { get; set; }
    }
}
