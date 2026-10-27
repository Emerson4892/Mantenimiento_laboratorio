using System;
using System.Collections.Generic;
using System.Text;


namespace Mantenimiento.Domain.Entities
{
    public class Falla
    {
        public int IdFalla { get; set; }

        public int IdEquipo { get; set; }

        public string Descripcion { get; set; } = string.Empty;

        public string Prioridad { get; set; } = "Media";

        public string Estado { get; set; } = "Pendiente";

        public DateTime FechaReporte { get; set; }
    }
}
