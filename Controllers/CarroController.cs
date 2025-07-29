using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.DataAccess;
using EcommerceAPI.Models;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarroController : ControllerBase
    {
        private readonly CarroDAO _dao;
        private readonly string _uploadFolderPath;

        public CarroController()
        {
            _dao = new CarroDAO();
            _uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(_uploadFolderPath);
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
        public async Task<ActionResult> Inserir([FromForm] CarroFormModel carroForm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (carroForm.FotoArquivo == null || carroForm.FotoArquivo.Length == 0)
            {
                ModelState.AddModelError("FotoArquivo", "A foto é obrigatória.");
                return BadRequest(ModelState);
            }

            string filePath = "";
            string relativeUrl = "";

            try
            {
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(carroForm.FotoArquivo.FileName);
                filePath = Path.Combine(_uploadFolderPath, uniqueFileName);
                relativeUrl = $"/uploads/{uniqueFileName}";

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await carroForm.FotoArquivo.CopyToAsync(stream);
                }

                var carro = new Carro
                {
                    Modelo = carroForm.Modelo,
                    Marca = carroForm.Marca,
                    Preco = carroForm.Preco,
                    Quantidade = carroForm.Quantidade,
                    Foto = relativeUrl,
                    Categoria = new Categoria { Id = carroForm.CategoriaId }
                };

                if (carro.Categoria == null || carro.Categoria.Id <= 0)
                {
                    ModelState.AddModelError("Categoria.Id", "O ID da Categoria é obrigatório e deve ser válido.");
                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                    return BadRequest(ModelState);
                }

                if (_dao.Inserir(carro))
                    return Created("", carro);
                else
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    return BadRequest("Erro ao inserir carro.");
                }
            }
            catch (Exception ex)
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                Console.Error.WriteLine($"Erro ao processar upload: {ex.Message}");
                return StatusCode(500, "Erro interno ao processar a requisição.");
            }
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

    public class CarroFormModel
    {
        public string Modelo { get; set; }
        public string Marca { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
        public IFormFile FotoArquivo { get; set; }
        public int CategoriaId { get; set; }
    }
}