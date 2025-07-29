using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class VendaCarro
    {
        public int Id { get; set; }

        [Required]
        public int VendaId { get; set; } 

        [Required]
        public int CarroId { get; set; } 

        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
    }
}