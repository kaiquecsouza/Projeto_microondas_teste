using System;
using System.Collections.Generic;
using System.Linq;
using Microondas.Dominio.Abstracoes;
using Microondas.Dominio.Modelos;

namespace Microondas.Testes
{
    /// <summary>
    /// Implementação em memória de <see cref="IProgramaRepositorio"/> para isolar os testes da
    /// camada de negócio de qualquer dependência de arquivo/banco.
    /// </summary>
    public sealed class RepositorioProgramasFake : IProgramaRepositorio
    {
        private readonly List<ProgramaAquecimento> _programas = new List<ProgramaAquecimento>();

        public IReadOnlyList<ProgramaAquecimento> ObterCustomizados() => _programas.ToList();

        public void Adicionar(ProgramaAquecimento programa) => _programas.Add(programa);

        public bool Remover(string nome)
        {
            return _programas.RemoveAll(p =>
                string.Equals(p.Nome, nome, StringComparison.OrdinalIgnoreCase)) > 0;
        }
    }
}
