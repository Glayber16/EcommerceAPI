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

        [HttpPost]
        public ActionResult Inserir([FromBody] Carro carro)
        {
            if (_dao.Inserir(carro))
                return Created("", carro);
            else
                return BadRequest("Erro ao inserir carro.");
        }

        [HttpPut("{id}")]
        public ActionResult Atualizar(int id, [FromBody] Carro carro)
        {
            carro.Id = id;

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