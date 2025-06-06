using System.ComponentModel.DataAnnotations;
namespace EcommerceAPI.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string descricao { get; set; } = string.Empty;
    }
}