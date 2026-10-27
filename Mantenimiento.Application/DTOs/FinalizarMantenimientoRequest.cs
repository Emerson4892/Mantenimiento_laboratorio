
using System.ComponentModel.DataAnnotations;

namespace Mantenimiento.Application.DTOs
{
    public class FinalizarMantenimientoRequest
    {
        // Identificador del mantenimiento que se finalizará
        [Range(1, int.MaxValue,
            ErrorMessage = "Debe indicar un mantenimiento válido.")]
        public int IdMantenimiento { get; set; }

        // Descripción de la solución realizada
        [Required(ErrorMessage = "La solución es obligatoria.")]
        public string Solucion { get; set; } = string.Empty;
    }
}
