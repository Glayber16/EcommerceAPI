using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Models;
using EcommerceAPI.DataAccess;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioDAO _dao;

        public UsuarioController()
        {
            _dao = new UsuarioDAO(); 
        }

        [HttpPost("cadastrar")]
        public IActionResult Cadastrar([FromBody] Usuario usuario)
        {
            var sucesso = _dao.InserirUsuario(usuario);

            if (sucesso)
                return Ok("Usuário cadastrado com sucesso.");
            else
                return BadRequest("Erro ao cadastrar usuário.");
        }

        [HttpPost("login")]
        public ActionResult<Usuario> Login([FromBody] Usuario usuario)
        {
            var encontrado = _dao.Obter(usuario.Login, usuario.Senha);

            if (encontrado is null)
                return Unauthorized("Login ou senha inválidos.");

            return Ok(encontrado);
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, [FromBody] Usuario usuario)
        {
            var sucesso = _dao.Atualizar(
                usuario.Nome,
                usuario.Endereco,
                usuario.Email,
                usuario.Login,
                usuario.Senha,
                id
            );

            if (sucesso)
                return Ok("Usuário atualizado.");
            else
                return NotFound("Usuário não encontrado.");
        }

        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            var sucesso = _dao.Remover(id);

            if (sucesso)
                return Ok("Usuário removido.");
            else
                return NotFound("Usuário não encontrado.");
        }

        [HttpGet]
        public IActionResult ver ()
        {
             var usuarios = _dao.ObterTodos();
              if (usuarios == null || usuarios.Count == 0)
            {
                return NotFound("Nenhum usuário encontrado.");
            }

            return Ok(usuarios);
        }
    }
}
