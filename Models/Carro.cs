using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class Carro
    {
        public int Id { get; set; }
         [Required]
        public string Modelo { get; set; } = string.Empty;
        [Required]
        public Categoria categoria { get; set; }
        
        public string Marca { get; set; } = string.Empty;
         [Required]
        public decimal Preco { get; set; }
        public string Foto { get; set; } = string.Empty;
         [Required]
        public int Quantidade { get; set; }
    }
    
}