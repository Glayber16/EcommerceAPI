using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;
namespace EcommerceAPI.Models
{
    public class Carro
    {
        public int Id { get; set; }
         [Required]
        public string Modelo { get; set; } = string.Empty;
        [Required]
        
        public int CategoriaId { get; set; } 

        [ForeignKey("CategoriaId")]
        public Categoria Categoria { get; set; }
        
        public string Marca { get; set; } = string.Empty;
         [Required]
        public decimal Preco { get; set; }
        public string Foto { get; set; } = string.Empty;
         [Required]
        public int Quantidade { get; set; }
    }
    
}