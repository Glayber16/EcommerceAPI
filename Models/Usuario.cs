using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; } = string.Empty;
         [Required]
        public string Endereco { get; set; } = string.Empty;
         [Required]
        public string Email { get; set; } = string.Empty;
         [Required]
        public string Login { get; set; } = string.Empty;
         [Required]
        public string Senha { get; set; } = string.Empty;
        public bool Adm { get; set; }
    }
}