using System;

namespace Microondas.Dominio.Abstracoes
{
    /// <summary>
    /// Abstração para registro de erros não tratados (Nível 4, item 2.c).
    /// A implementação concreta (arquivo texto, banco, etc.) fica na infraestrutura.
    /// </summary>
    public interface ILogadorErros
    {
        /// <summary>Registra a exceção com contexto, InnerException e stacktrace.</summary>
        void Registrar(Exception excecao, string contexto);
    }
}
