using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.DataAccess;
using EcommerceAPI.Models;
using System.Collections.Generic;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarroController : ControllerBase
    {
        private readonly CarroDAO _dao;

        public CarroController()
        {
            _dao = new CarroDAO();
        }

        [HttpGet]
        public ActionResult<List<Carro>> Listar()
        {
            return Ok(_dao.Listar());
        }

        [HttpGet("{id}")]
        public ActionResult<Carro> ObterPorId(int id)
        {
            var carro = _dao.ObterPorId(id);

            if (carro == null)
                return NotFound($"Carro com ID {id} não encontrado.");

            return Ok(carro);
        }

        [HttpPost]
        public ActionResult Inserir([FromBody] Carro carro)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (carro.Categoria == null || carro.Categoria.Id <= 0)
            {
                ModelState.AddModelError("Categoria.Id", "O ID da Categoria é obrigatório e deve ser válido.");
                return BadRequest(ModelState);
            }

            if (_dao.Inserir(carro))
                return Created("", carro);
            else
                return BadRequest("Erro ao inserir carro.");
        }

        [HttpPut("{id}")]
        public ActionResult Atualizar(int id, [FromBody] Carro carro)
        {
            carro.Id = id;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (carro.Categoria == null || carro.Categoria.Id <= 0)
            {
                ModelState.AddModelError("Categoria.Id", "O ID da Categoria é obrigatório e deve ser válido para atualização.");
                return BadRequest(ModelState);
            }

            if (_dao.Atualizar(carro))
                return Ok("Carro atualizado com sucesso.");
            else
                return NotFound("Carro não encontrado.");
        }

        [HttpDelete("{id}")]
        public ActionResult Remover(int id)
        {
            if (_dao.Remover(id))
                return Ok("Carro removido com sucesso.");
            else
                return NotFound("Carro não encontrado.");
        }

        [HttpGet("buscar")]
        public ActionResult<List<Carro>> Buscar([FromQuery] string termo)
        {
            var resultado = _dao.Buscar(termo);

            if (resultado.Count == 0)
                return NotFound("Nenhum carro encontrado com esse modelo ou marca.");

            return Ok(resultado);
        }
    }
}
