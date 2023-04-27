using System.ComponentModel.DataAnnotations.Schema;

namespace MvcApiHospitalPractica.Models
{
    public class Hospital
    {
        public int Hospital_cod { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telelfono { get; set; }
        public int Num_cama { get; set; }
    }
}
