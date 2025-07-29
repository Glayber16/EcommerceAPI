using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace EcommerceAPI.Models
{
    public class Venda
    {
        public int Id { get; set; }
        public DateTime data { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        public List<VendaCarro> Itens { get; set; } = new List<VendaCarro>();
    }
}