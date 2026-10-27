using System;
using System.Collections.Generic;
using System.Text;

namespace Mantenimiento.Domain.Entities
{
    public class Mantenimiento
    {
        public int IdMantenimiento { get; set; }

        public int IdFalla { get; set; }

        public string Tecnico { get; set; } = string.Empty;

        public string? Observacion { get; set; }

        public string Estado { get; set; } = "Asignado";

        public DateTime FechaAsignacion { get; set; }

        public string? Solucion { get; set; }

        public DateTime? FechaFinalizacion { get; set; }
    }
}
