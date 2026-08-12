using System.Collections.Generic;
using Microondas.Dominio.Modelos;

namespace Microondas.Dominio.Abstracoes
{
    /// <summary>
    /// Abstração de persistência dos programas customizados (Nível 3, item 1.e).
    /// A implementação concreta (JSON, SQL Server, etc.) reside na camada de infraestrutura,
    /// de modo que o domínio não depende de detalhes de armazenamento (DIP).
    /// </summary>
    public interface IProgramaRepositorio
    {
        /// <summary>Retorna todos os programas customizados persistidos.</summary>
        IReadOnlyList<ProgramaAquecimento> ObterCustomizados();

        /// <summary>Persiste um novo programa customizado.</summary>
        void Adicionar(ProgramaAquecimento programa);

        /// <summary>Remove um programa customizado pelo nome. Retorna true se removeu.</summary>
        bool Remover(string nome);
    }
}
