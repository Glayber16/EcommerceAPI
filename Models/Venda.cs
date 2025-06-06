using System.ComponentModel.DataAnnotations;
namespace EcommerceAPI.Models
{
    public class Venda
    {
        public int Id { get; set; }
        public DateTime data { get; set; }
        [Required]
        public int UsuarioId { get; set; }
    }
}