using Microondas.Dominio.Modelos;

namespace Microondas.Dominio.Abstracoes
{
    /// <summary>
    /// Contrato de negócio do forno de micro-ondas. Concentra toda a lógica de aquecimento,
    /// pausa, cancelamento, início rápido e acréscimo de tempo, mantendo a interface de usuário
    /// (ou a Web API) totalmente separada das regras (requisito D).
    /// </summary>
    public interface IFornoMicroondas
    {
        /// <summary>
        /// Aciona o botão iniciar. O comportamento depende do estado atual:
        /// <list type="bullet">
        /// <item>Parado/Concluído: inicia um novo aquecimento manual. Se tempo e potência forem
        /// nulos, executa o "início rápido" (30s, potência 10).</item>
        /// <item>Em andamento (manual): acrescenta 30 segundos ao tempo restante.</item>
        /// <item>Pausado: retoma o aquecimento do ponto em que parou.</item>
        /// </list>
        /// </summary>
        StatusForno Iniciar(int? tempoSegundos, int? potencia);

        /// <summary>Inicia um programa (pré-definido ou customizado). Não permite acréscimo de tempo.</summary>
        StatusForno IniciarPrograma(ProgramaAquecimento programa);

        /// <summary>
        /// Botão único de pausa/cancelamento:
        /// em andamento => pausa; pausado => cancela e limpa; parado/concluído => limpa os dados.
        /// </summary>
        StatusForno PausarOuCancelar();

        /// <summary>Avança 1 segundo de aquecimento, atualizando a string de progresso.</summary>
        StatusForno Avancar();

        /// <summary>Retorna o status atual sem alterar o estado.</summary>
        StatusForno ObterStatus();

        /// <summary>Limpa completamente o forno, voltando ao estado inicial.</summary>
        StatusForno Resetar();
    }
}
