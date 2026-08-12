using Microondas.Api.Autenticacao;
using Microondas.Api.Contratos;
using Microondas.Dominio.Excecoes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microondas.Api.Controllers
{
    /// <summary>
    /// Endpoints de autenticação (Nível 4). Permite configurar as credenciais (item 1.d),
    /// efetuar login para obter o Bearer Token (item 1.b) e consultar o status.
    /// </summary>
    [ApiController]
    [Route("api/autenticacao")]
    public sealed class AutenticacaoController : ControllerBase
    {
        private readonly RepositorioCredenciais _credenciais;
        private readonly ServicoToken _token;

        public AutenticacaoController(RepositorioCredenciais credenciais, ServicoToken token)
        {
            _credenciais = credenciais;
            _token = token;
        }

        /// <summary>Configura/atualiza usuário e senha. A senha é persistida como hash SHA-256.</summary>
        [AllowAnonymous]
        [HttpPost("configurar")]
        public IActionResult Configurar([FromBody] CredenciaisRequest req)
        {
            if (req == null)
                throw new RegraNegocioException("Dados de credenciais não informados.");

            _credenciais.Definir(req.Usuario, req.Senha);
            return Ok(new { sucesso = true, mensagem = "Credenciais configuradas com sucesso." });
        }

        /// <summary>Autentica e retorna o Bearer Token quando as credenciais são válidas.</summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] CredenciaisRequest req)
        {
            if (req == null || !_credenciais.Validar(req.Usuario, req.Senha))
                return Unauthorized(new { sucesso = false, mensagem = "Usuário ou senha inválidos." });

            string token = _token.Emitir(req.Usuario);
            return Ok(new { sucesso = true, token, tipo = "Bearer" });
        }

        /// <summary>Confirma que o token atual é válido (usado pela interface para exibir o status).</summary>
        [Authorize]
        [HttpGet("status")]
        public IActionResult Status()
        {
            return Ok(new { sucesso = true, autenticado = true, usuario = User.Identity?.Name });
        }
    }
}
