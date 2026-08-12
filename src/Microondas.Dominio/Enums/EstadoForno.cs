namespace Microondas.Dominio.Enums
{
    /// <summary>
    /// Representa os estados possíveis do forno de micro-ondas.
    /// Modelado como uma máquina de estados para tornar as transições explícitas e testáveis.
    /// </summary>
    public enum EstadoForno
    {
        /// <summary>Ocioso: nenhum aquecimento em andamento.</summary>
        Parado = 0,

        /// <summary>Aquecimento em execução.</summary>
        EmAndamento = 1,

        /// <summary>Aquecimento pausado; pode ser retomado ou cancelado.</summary>
        Pausado = 2,

        /// <summary>Aquecimento finalizado com sucesso.</summary>
        Concluido = 3
    }
}
