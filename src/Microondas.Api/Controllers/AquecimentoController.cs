using Microondas.Api.Contratos;
using Microondas.Dominio.Abstracoes;
using Microondas.Dominio.Servicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microondas.Api.Controllers
{
    /// <summary>
    /// Exporta as regras de aquecimento como endpoints da Web API (Nível 4, item 1.a).
    /// Todos exigem autenticação Bearer (item 1.c). A lógica reside no domínio; o controller
    /// apenas orquestra e traduz para HTTP.
    /// </summary>
    [ApiController]
    [Route("api/aquecimento")]
    [Authorize]
    public sealed class AquecimentoController : ControllerBase
    {
        private readonly IFornoMicroondas _forno;
        private readonly ProgramaServico _programas;

        public AquecimentoController(IFornoMicroondas forno, ProgramaServico programas)
        {
            _forno = forno;
            _programas = programas;
        }

        /// <summary>Retorna o status atual do forno.</summary>
        [HttpGet("status")]
        public IActionResult Status() => Ok(_forno.ObterStatus());

        /// <summary>
        /// Aciona o botão iniciar: novo aquecimento, início rápido (corpo vazio/nulo),
        /// acréscimo de 30s (se em andamento) ou retomada (se pausado).
        /// </summary>
        [HttpPost("iniciar")]
        public IActionResult Iniciar([FromBody] IniciarAquecimentoRequest req)
        {
            req ??= new IniciarAquecimentoRequest();
            return Ok(_forno.Iniciar(req.TempoSegundos, req.Potencia));
        }

        /// <summary>Inicia um programa pré-definido ou customizado pelo nome.</summary>
        [HttpPost("programa/{nome}")]
        public IActionResult IniciarPrograma(string nome)
        {
            var programa = _programas.ObterPorNome(nome);
            return Ok(_forno.IniciarPrograma(programa));
        }

        /// <summary>Botão único de pausa/cancelamento.</summary>
        [HttpPost("pausar-cancelar")]
        public IActionResult PausarOuCancelar() => Ok(_forno.PausarOuCancelar());

        /// <summary>Avança 1 segundo de aquecimento (acionado pelo timer da interface).</summary>
        [HttpPost("avancar")]
        public IActionResult Avancar() => Ok(_forno.Avancar());

        /// <summary>Limpa o forno completamente.</summary>
        [HttpPost("resetar")]
        public IActionResult Resetar() => Ok(_forno.Resetar());
    }
}
