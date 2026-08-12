using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace Microondas.Api.Autenticacao
{
    /// <summary>
    /// Emite e valida tokens Bearer (Nível 4, item 1.b). Implementação opaca em memória, com
    /// expiração, para manter a solução autocontida (sem dependências externas de JWT). Cada token
    /// é um valor aleatório criptograficamente seguro associado a um usuário e a um vencimento.
    /// </summary>
    public sealed class ServicoToken
    {
        private readonly ConcurrentDictionary<string, TokenInfo> _tokens =
            new ConcurrentDictionary<string, TokenInfo>();

        private readonly TimeSpan _validade = TimeSpan.FromHours(1);

        public string Emitir(string usuario)
        {
            string token = GerarTokenAleatorio();
            _tokens[token] = new TokenInfo(usuario, DateTime.UtcNow.Add(_validade));
            return token;
        }

        public bool Validar(string token, out string usuario)
        {
            usuario = null;

            if (string.IsNullOrWhiteSpace(token) || !_tokens.TryGetValue(token, out var info))
                return false;

            if (info.Expiracao < DateTime.UtcNow)
            {
                _tokens.TryRemove(token, out _);
                return false;
            }

            usuario = info.Usuario;
            return true;
        }

        private static string GerarTokenAleatorio()
        {
            byte[] bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes)
                .Replace("+", "-").Replace("/", "_").Replace("=", string.Empty);
        }

        private sealed class TokenInfo
        {
            public string Usuario { get; }
            public DateTime Expiracao { get; }

            public TokenInfo(string usuario, DateTime expiracao)
            {
                Usuario = usuario;
                Expiracao = expiracao;
            }
        }
    }
}
