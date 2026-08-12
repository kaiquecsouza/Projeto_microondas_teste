using System;

namespace Microondas.Dominio.Excecoes
{
    /// <summary>
    /// Exceção específica para violações de regras de negócio do sistema (Nível 4, item 2.b).
    /// Diferencia erros esperados/de validação (que geram mensagem amigável ao usuário)
    /// de erros inesperados (que são logados como falha da aplicação).
    /// </summary>
    [Serializable]
    public class RegraNegocioException : Exception
    {
        public RegraNegocioException(string mensagem) : base(mensagem)
        {
        }

        public RegraNegocioException(string mensagem, Exception inner) : base(mensagem, inner)
        {
        }
    }
}
