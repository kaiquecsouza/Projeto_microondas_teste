using System;
using System.Collections.Generic;
using Microondas.Dominio.Abstracoes;
using Microondas.Dominio.Configuracao;
using Microondas.Dominio.Excecoes;
using Microondas.Dominio.Modelos;

namespace Microondas.Dominio.Servicos
{
    /// <summary>
    /// Orquestra os programas pré-definidos (fixos) e os customizados (persistidos), aplicando
    /// as regras de cadastro do Nível 3. Fica entre o repositório e os consumidores (API/UI),
    /// concentrando as validações de negócio.
    /// </summary>
    public sealed class ProgramaServico
    {
        private readonly IProgramaRepositorio _repositorio;

        public ProgramaServico(IProgramaRepositorio repositorio)
        {
            _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        }

        /// <summary>Retorna pré-definidos seguidos dos customizados.</summary>
        public IReadOnlyList<ProgramaAquecimento> ObterTodos()
        {
            var lista = new List<ProgramaAquecimento>(CatalogoProgramas.Predefinidos());
            lista.AddRange(_repositorio.ObterCustomizados());
            return lista;
        }

        /// <summary>Busca um programa pelo nome (case-insensitive). Lança exceção se não existir.</summary>
        public ProgramaAquecimento ObterPorNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new RegraNegocioException("Informe o nome do programa.");

            foreach (var programa in ObterTodos())
            {
                if (string.Equals(programa.Nome, nome.Trim(), StringComparison.OrdinalIgnoreCase))
                    return programa;
            }

            throw new RegraNegocioException("Programa de aquecimento não encontrado: " + nome + ".");
        }

        /// <summary>
        /// Cadastra um programa customizado (Nível 3, item 1). Nome, alimento, tempo, potência e
        /// caractere são obrigatórios; instruções são opcionais. O caractere não pode ser '.' nem
        /// repetir o de qualquer outro programa. O nome também deve ser único.
        /// </summary>
        public ProgramaAquecimento Cadastrar(
            string nome,
            string alimento,
            int tempoSegundos,
            int potencia,
            char caractere,
            string instrucoes)
        {
            // As validações de campo/valor (obrigatoriedade, faixas, caractere reservado) ficam
            // centralizadas no construtor de ProgramaAquecimento, evitando duplicação.
            var programa = new ProgramaAquecimento(nome, alimento, tempoSegundos, potencia, caractere,
                instrucoes, customizado: true);

            GarantirNomeUnico(programa.Nome);
            GarantirCaractereUnico(programa.Caractere);

            _repositorio.Adicionar(programa);
            return programa;
        }

        /// <summary>Remove um programa customizado. Programas pré-definidos não podem ser removidos.</summary>
        public void Remover(string nome)
        {
            var programa = ObterPorNome(nome);

            if (!programa.Customizado)
                throw new RegraNegocioException("Programas pré-definidos não podem ser removidos.");

            _repositorio.Remover(programa.Nome);
        }

        private void GarantirNomeUnico(string nome)
        {
            foreach (var existente in ObterTodos())
            {
                if (string.Equals(existente.Nome, nome, StringComparison.OrdinalIgnoreCase))
                    throw new RegraNegocioException("Já existe um programa com o nome '" + nome + "'.");
            }
        }

        private void GarantirCaractereUnico(char caractere)
        {
            if (caractere == RegrasAquecimento.CaractereProgressoPadrao)
                throw new RegraNegocioException(
                    "O caractere de aquecimento não pode ser '.' pois é reservado.");

            foreach (var existente in ObterTodos())
            {
                if (existente.Caractere == caractere)
                    throw new RegraNegocioException(
                        "O caractere '" + caractere + "' já é utilizado pelo programa '" + existente.Nome + "'.");
            }
        }
    }
}
