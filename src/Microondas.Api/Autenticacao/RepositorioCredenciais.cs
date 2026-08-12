using System;
using System.IO;
using System.Text.Json;
using Microondas.Infraestrutura.Seguranca;

namespace Microondas.Api.Autenticacao
{
    /// <summary>
    /// Guarda as credenciais de acesso à API (Nível 4, item 1.d/1.e). A senha nunca é armazenada
    /// em texto puro: apenas seu hash SHA-256 é persistido em arquivo JSON. Permite reconfigurar
    /// as credenciais em tempo de execução (seção específica de configuração).
    /// </summary>
    public sealed class RepositorioCredenciais
    {
        private readonly string _caminhoArquivo;
        private readonly ServicoHash _hash;
        private readonly object _trava = new object();

        public RepositorioCredenciais(string caminhoArquivo, ServicoHash hash,
            string usuarioPadrao, string senhaPadrao)
        {
            _caminhoArquivo = caminhoArquivo;
            _hash = hash;

            string diretorio = Path.GetDirectoryName(Path.GetFullPath(caminhoArquivo));
            if (!string.IsNullOrEmpty(diretorio) && !Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);

            // Semeia credenciais padrão na primeira execução para que a API seja utilizável.
            if (!File.Exists(_caminhoArquivo))
                Definir(usuarioPadrao, senhaPadrao);
        }

        /// <summary>Define/atualiza usuário e senha, persistindo o hash da senha.</summary>
        public void Definir(string usuario, string senha)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("Usuário é obrigatório.", nameof(usuario));
            if (string.IsNullOrWhiteSpace(senha))
                throw new ArgumentException("Senha é obrigatória.", nameof(senha));

            var registro = new CredencialRegistro
            {
                Usuario = usuario.Trim(),
                SenhaHash = _hash.Gerar(senha)
            };

            lock (_trava)
            {
                File.WriteAllText(_caminhoArquivo,
                    JsonSerializer.Serialize(registro, new JsonSerializerOptions { WriteIndented = true }));
            }
        }

        /// <summary>Valida um par usuário/senha contra o hash persistido.</summary>
        public bool Validar(string usuario, string senha)
        {
            var registro = Carregar();
            if (registro == null)
                return false;

            bool usuarioOk = string.Equals(registro.Usuario, usuario?.Trim(),
                StringComparison.OrdinalIgnoreCase);

            return usuarioOk && _hash.Verificar(senha ?? string.Empty, registro.SenhaHash);
        }

        private CredencialRegistro Carregar()
        {
            lock (_trava)
            {
                if (!File.Exists(_caminhoArquivo))
                    return null;

                string conteudo = File.ReadAllText(_caminhoArquivo);
                return string.IsNullOrWhiteSpace(conteudo)
                    ? null
                    : JsonSerializer.Deserialize<CredencialRegistro>(conteudo);
            }
        }

        private sealed class CredencialRegistro
        {
            public string Usuario { get; set; }
            public string SenhaHash { get; set; }
        }
    }
}
