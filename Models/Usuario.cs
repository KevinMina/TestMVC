using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestMVC.Models
{
    public partial class Usuario
    {
        public int IdUser { get; set; }

        public string NombreUser { get; set; } 

        public bool EstadoUser { get; set; }

        public virtual ICollection<VentasDetalle> VentasDetalles { get; set; } = new List<VentasDetalle>();
    }
}