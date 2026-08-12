using System.Linq;
using Microondas.Api.Contratos;
using Microondas.Dominio.Excecoes;
using Microondas.Dominio.Servicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microondas.Api.Controllers
{
    /// <summary>
    /// Endpoints de manutenção de programas de aquecimento (Níveis 2, 3 e 4).
    /// </summary>
    [ApiController]
    [Route("api/programas")]
    [Authorize]
    public sealed class ProgramasController : ControllerBase
    {
        private readonly ProgramaServico _programas;

        public ProgramasController(ProgramaServico programas)
        {
            _programas = programas;
        }

        /// <summary>Lista todos os programas (pré-definidos + customizados).</summary>
        [HttpGet]
        public IActionResult Listar()
        {
            var resultado = _programas.ObterTodos().Select(p => new
            {
                p.Nome,
                p.Alimento,
                p.TempoSegundos,
                p.Potencia,
                Caractere = p.Caractere.ToString(),
                p.Instrucoes,
                p.Customizado
            });

            return Ok(resultado);
        }

        /// <summary>Cadastra um programa customizado (Nível 3).</summary>
        [HttpPost]
        public IActionResult Cadastrar([FromBody] CadastroProgramaRequest req)
        {
            if (req == null)
                throw new RegraNegocioException("Dados do programa não informados.");

            if (string.IsNullOrEmpty(req.Caractere))
                throw new RegraNegocioException("O caractere de aquecimento é obrigatório.");

            var criado = _programas.Cadastrar(
                req.Nome, req.Alimento, req.TempoSegundos, req.Potencia,
                req.Caractere[0], req.Instrucoes);

            return Ok(new
            {
                sucesso = true,
                mensagem = "Programa cadastrado com sucesso.",
                programa = new
                {
                    criado.Nome,
                    criado.Alimento,
                    criado.TempoSegundos,
                    criado.Potencia,
                    Caractere = criado.Caractere.ToString(),
                    criado.Customizado
                }
            });
        }

        /// <summary>Remove um programa customizado (pré-definidos não podem ser removidos).</summary>
        [HttpDelete("{nome}")]
        public IActionResult Remover(string nome)
        {
            _programas.Remover(nome);
            return Ok(new { sucesso = true, mensagem = "Programa removido com sucesso." });
        }
    }
}
