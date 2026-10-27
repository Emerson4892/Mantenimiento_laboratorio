using System;
using System.Collections.Generic;
using System.Text;

namespace Mantenimiento.Domain.Entities
{
    public class Equipo
    {
        public int IdEquipo { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string Tipo { get; set; } = string.Empty;

        public string? Marca { get; set; }

        public string? Modelo { get; set; }

        public string? NumeroSerie { get; set; }

        public string? Ubicacion { get; set; }

        public string Estado { get; set; } = "Disponible";

        public DateTime FechaRegistro { get; set; }
    }
}