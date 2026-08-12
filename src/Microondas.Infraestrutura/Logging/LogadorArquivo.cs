using System;
using System.IO;
using System.Text;
using Microondas.Dominio.Abstracoes;

namespace Microondas.Infraestrutura.Logging
{
    /// <summary>
    /// Registra exceções não tratadas em arquivo de texto (Nível 4, item 2.c), incluindo
    /// a Exception, a Inner Exception, o stacktrace e informações de contexto.
    /// </summary>
    public sealed class LogadorArquivo : ILogadorErros
    {
        private readonly string _caminhoArquivo;
        private readonly object _trava = new object();

        public LogadorArquivo(string caminhoArquivo)
        {
            if (string.IsNullOrWhiteSpace(caminhoArquivo))
                throw new ArgumentException("Caminho do arquivo de log é obrigatório.", nameof(caminhoArquivo));

            _caminhoArquivo = caminhoArquivo;

            string diretorio = Path.GetDirectoryName(Path.GetFullPath(_caminhoArquivo));
            if (!string.IsNullOrEmpty(diretorio) && !Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);
        }

        public void Registrar(Exception excecao, string contexto)
        {
            if (excecao == null)
                return;

            var sb = new StringBuilder();
            sb.AppendLine("==================================================");
            sb.AppendLine("Data/Hora......: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            sb.AppendLine("Contexto.......: " + (contexto ?? "(não informado)"));
            sb.AppendLine("Exception......: " + excecao.GetType().FullName);
            sb.AppendLine("Mensagem.......: " + excecao.Message);
            sb.AppendLine("Inner Exception: " + (excecao.InnerException?.ToString() ?? "(nenhuma)"));
            sb.AppendLine("StackTrace.....:");
            sb.AppendLine(excecao.StackTrace ?? "(indisponível)");
            sb.AppendLine();

            lock (_trava)
            {
                File.AppendAllText(_caminhoArquivo, sb.ToString(), Encoding.UTF8);
            }
        }
    }
}
