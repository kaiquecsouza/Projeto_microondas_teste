using System;
using Microondas.Dominio.Configuracao;
using Microondas.Dominio.Excecoes;

namespace Microondas.Dominio.Modelos
{
    /// <summary>
    /// Representa um programa de aquecimento (pré-definido ou customizado).
    /// É imutável: uma vez criado, seus dados não podem ser alterados. Isso protege o uso
    /// incorreto (requisito I) e atende à regra de que programas pré-definidos não podem ser
    /// modificados (Nível 2, item 1.c).
    /// </summary>
    public sealed class ProgramaAquecimento
    {
        public string Nome { get; }
        public string Alimento { get; }
        public int TempoSegundos { get; }
        public int Potencia { get; }
        public char Caractere { get; }
        public string Instrucoes { get; }

        /// <summary>Indica se o programa foi cadastrado pelo usuário (true) ou é de fábrica (false).</summary>
        public bool Customizado { get; }

        public ProgramaAquecimento(
            string nome,
            string alimento,
            int tempoSegundos,
            int potencia,
            char caractere,
            string instrucoes,
            bool customizado)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new RegraNegocioException("O nome do programa é obrigatório.");

            if (string.IsNullOrWhiteSpace(alimento))
                throw new RegraNegocioException("O alimento do programa é obrigatório.");

            if (tempoSegundos < RegrasAquecimento.TempoMinimoSegundos)
                throw new RegraNegocioException("O tempo do programa deve ser de ao menos 1 segundo.");

            if (potencia < RegrasAquecimento.PotenciaMinima || potencia > RegrasAquecimento.PotenciaMaxima)
                throw new RegraNegocioException("A potência do programa deve estar entre 1 e 10.");

            if (caractere == RegrasAquecimento.CaractereProgressoPadrao)
                throw new RegraNegocioException(
                    "O caractere de aquecimento não pode ser '.' pois é reservado ao aquecimento padrão.");

            if (char.IsWhiteSpace(caractere))
                throw new RegraNegocioException("O caractere de aquecimento não pode ser um espaço em branco.");

            Nome = nome.Trim();
            Alimento = alimento.Trim();
            TempoSegundos = tempoSegundos;
            Potencia = potencia;
            Caractere = caractere;
            Instrucoes = instrucoes?.Trim() ?? string.Empty;
            Customizado = customizado;
        }
    }
}
