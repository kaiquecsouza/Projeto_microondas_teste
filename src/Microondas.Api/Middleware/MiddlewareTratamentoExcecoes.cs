using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microondas.Api.Contratos;
using Microondas.Dominio.Abstracoes;
using Microondas.Dominio.Excecoes;
using Microsoft.AspNetCore.Http;

namespace Microondas.Api.Middleware
{
    /// <summary>
    /// Mecanismo central de tratamento de exceções (Nível 4, item 2.a).
    /// - <see cref="RegraNegocioException"/> => HTTP 400 com mensagem amigável (item 2.b), não logada
    ///   como falha por ser um erro esperado de validação.
    /// - Qualquer outra exceção não tratada => HTTP 500 com resposta padronizada e registro completo
    ///   em log (item 2.c: Exception, Inner Exception, stacktrace e contexto).
    /// </summary>
    public sealed class MiddlewareTratamentoExcecoes
    {
        private readonly RequestDelegate _proximo;
        private readonly ILogadorErros _logador;

        public MiddlewareTratamentoExcecoes(RequestDelegate proximo, ILogadorErros logador)
        {
            _proximo = proximo;
            _logador = logador;
        }

        public async Task InvokeAsync(HttpContext contexto)
        {
            try
            {
                await _proximo(contexto);
            }
            catch (RegraNegocioException ex)
            {
                await EscreverRespostaAsync(contexto, HttpStatusCode.BadRequest,
                    new RespostaErro(ex.Message, "RegraNegocio"));
            }
            catch (Exception ex)
            {
                _logador.Registrar(ex, $"{contexto.Request.Method} {contexto.Request.Path}");
                await EscreverRespostaAsync(contexto, HttpStatusCode.InternalServerError,
                    new RespostaErro("Ocorreu um erro interno. Tente novamente mais tarde.", "ErroInterno"));
            }
        }

        private static async Task EscreverRespostaAsync(HttpContext contexto, HttpStatusCode status,
            RespostaErro corpo)
        {
            contexto.Response.Clear();
            contexto.Response.StatusCode = (int)status;
            contexto.Response.ContentType = "application/json; charset=utf-8";

            string json = JsonSerializer.Serialize(corpo,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            await contexto.Response.WriteAsync(json);
        }
    }
}
