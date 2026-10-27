using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Mantenimiento.Application.DTOs
{
    public class RegistrarEquipoRequest
    {
        [Required(ErrorMessage = "El código es obligatorio.")]
        [MaxLength(30, ErrorMessage = "El código no puede superar 30 caracteres.")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El tipo no puede superar 50 caracteres.")]
        public string Tipo { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Marca { get; set; }

        [MaxLength(50)]
        public string? Modelo { get; set; }

        [MaxLength(100)]
        public string? NumeroSerie { get; set; }

        [MaxLength(100)]
        public string? Ubicacion { get; set; }
    }
}
