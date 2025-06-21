using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.DataAccess;
using EcommerceAPI.Models;
using System.Collections.Generic;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaController : ControllerBase
    {
        private readonly CategoriaDAO _dao;

        public CategoriaController()
        {
            _dao = new CategoriaDAO();
        }

        
        [HttpGet]
        public ActionResult<List<Categoria>> ListarCategorias()
        {
            var categorias = _dao.Listar();
            return Ok(categorias);
        }

       
        [HttpPost]
        public ActionResult Inserir ([FromBody] Categoria categoria)
        {
            if (categoria == null || string.IsNullOrWhiteSpace(categoria.descricao))
            {
                return BadRequest("Descrição é obrigatória.");
            }

            var sucesso = _dao.Inserir(categoria.descricao);

            if (sucesso)
            {
                return CreatedAtAction(nameof(ListarCategorias), new { descricao = categoria.descricao }, categoria);
            }

            return StatusCode(500, "Erro ao inserir a categoria.");
        }
    }
}
