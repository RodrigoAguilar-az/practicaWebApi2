using System.ComponentModel.DataAnnotations;

namespace practicaWebApi2.Models
{
    public class Libro
    {
        [Key]
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int Anyopublicacion { get; set; }
        public int AutorId { get; set; }
        public int CategoriaId { get; set; }
        public string Resumen { get; set; }


    }
}
