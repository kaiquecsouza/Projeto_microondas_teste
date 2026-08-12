using System.Collections.Generic;
using Microondas.Dominio.Modelos;

namespace Microondas.Dominio.Servicos
{
    /// <summary>
    /// Fábrica dos 5 programas de aquecimento pré-definidos (Nível 2, item 1).
    /// Cada programa usa um caractere de aquecimento único e diferente de '.'.
    /// A lista é recriada a cada chamada, garantindo que os objetos de fábrica não sejam
    /// alterados em memória (item 1.c: não podem ser alterados ou excluídos).
    /// </summary>
    public static class CatalogoProgramas
    {
        public static IReadOnlyList<ProgramaAquecimento> Predefinidos()
        {
            return new List<ProgramaAquecimento>
            {
                new ProgramaAquecimento(
                    nome: "Pipoca",
                    alimento: "Pipoca (de micro-ondas)",
                    tempoSegundos: 180,   // 3 minutos
                    potencia: 7,
                    caractere: '*',
                    instrucoes: "Observar o barulho de estouros do milho. Caso haja um intervalo de mais de " +
                                "10 segundos entre um estouro e outro, interrompa o aquecimento.",
                    customizado: false),

                new ProgramaAquecimento(
                    nome: "Leite",
                    alimento: "Leite",
                    tempoSegundos: 300,   // 5 minutos
                    potencia: 5,
                    caractere: '~',
                    instrucoes: "Cuidado com o aquecimento de líquidos. O choque térmico aliado ao movimento do " +
                                "recipiente pode causar fervura imediata e risco de queimaduras.",
                    customizado: false),

                new ProgramaAquecimento(
                    nome: "Carnes de boi",
                    alimento: "Carne em pedaço ou fatias",
                    tempoSegundos: 840,   // 14 minutos
                    potencia: 4,
                    caractere: '#',
                    instrucoes: "Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima " +
                                "para o descongelamento uniforme.",
                    customizado: false),

                new ProgramaAquecimento(
                    nome: "Frango",
                    alimento: "Frango (qualquer corte)",
                    tempoSegundos: 480,   // 8 minutos
                    potencia: 7,
                    caractere: '@',
                    instrucoes: "Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima " +
                                "para o descongelamento uniforme.",
                    customizado: false),

                new ProgramaAquecimento(
                    nome: "Feijão",
                    alimento: "Feijão congelado",
                    tempoSegundos: 480,   // 8 minutos
                    potencia: 9,
                    caractere: '%',
                    instrucoes: "Deixe o recipiente destampado. Em recipientes de plástico, cuidado ao retirá-lo, " +
                                "pois pode perder resistência em altas temperaturas.",
                    customizado: false)
            };
        }
    }
}
