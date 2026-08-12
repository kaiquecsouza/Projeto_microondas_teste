using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microondas.Api.Autenticacao
{
    /// <summary>
    /// Esquema de autenticação Bearer Token integrado ao pipeline do ASP.NET Core.
    /// Lê o cabeçalho "Authorization: Bearer &lt;token&gt;" e valida contra o <see cref="ServicoToken"/>.
    /// Endpoints marcados com [Authorize] só executam quando o token é válido (Nível 4, item 1.c).
    /// </summary>
    public sealed class AutenticacaoBearerHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string Esquema = "Bearer";

        private readonly ServicoToken _servicoToken;

        public AutenticacaoBearerHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ServicoToken servicoToken)
            : base(options, logger, encoder)
        {
            _servicoToken = servicoToken;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var valores))
                return Task.FromResult(AuthenticateResult.NoResult());

            string cabecalho = valores.ToString();
            const string prefixo = "Bearer ";

            if (string.IsNullOrWhiteSpace(cabecalho) ||
                !cabecalho.StartsWith(prefixo, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(AuthenticateResult.Fail("Cabeçalho de autorização ausente ou inválido."));
            }

            string token = cabecalho.Substring(prefixo.Length).Trim();

            if (!_servicoToken.Validar(token, out string usuario))
                return Task.FromResult(AuthenticateResult.Fail("Token inválido ou expirado."));

            var identidade = new ClaimsIdentity(
                new List<Claim> { new Claim(ClaimTypes.Name, usuario) }, Esquema);
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identidade), Esquema);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
