using EcommerceAPI.DataAccess;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendaController : ControllerBase
    {
        private readonly VendaDAO _vendaDAO;

        public VendaController()
        {
            _vendaDAO = new VendaDAO();
        }

        [HttpPost]
        public IActionResult CriarVenda([FromBody] Venda venda)
        {
            if (!ModelState.IsValid || venda.Itens == null || venda.Itens.Count == 0)
                return BadRequest("Venda inválida. Deve conter ao menos um item.");

            var sucesso = _vendaDAO.InserirComCarros(venda);
            if (sucesso)
                return Ok("Venda registrada com sucesso.");

            return StatusCode(500, "Erro ao registrar venda.");
        }

        [HttpGet]
        public IActionResult ObterTodas()
        {
            var vendas = _vendaDAO.ObterTodasComItens();
            return Ok(vendas);
        }

        [HttpGet("usuario/{usuarioId}")]
        public IActionResult ObterPorUsuario(int usuarioId)
        {
            var vendas = _vendaDAO.ObterPorUsuario(usuarioId);
            if (vendas == null || vendas.Count == 0)
                return NotFound("Nenhuma venda encontrada para este usuário.");

            return Ok(vendas);
        }
    }
}
