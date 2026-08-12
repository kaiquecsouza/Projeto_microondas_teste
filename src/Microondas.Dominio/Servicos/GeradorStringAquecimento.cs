using System;
using Microondas.Dominio.Abstracoes;

namespace Microondas.Dominio.Servicos
{
    /// <summary>
    /// Gera o segmento de 1 segundo da string de progresso: o caractere repetido "potência" vezes.
    /// A junção dos segmentos por espaço é responsabilidade do forno, produzindo, por exemplo:
    /// tempo 10 / potência 1 => ". . . . . . . . . ."; tempo 5 / potência 3 => "... ... ... ... ...".
    /// </summary>
    public sealed class GeradorStringAquecimento : IGeradorStringAquecimento
    {
        public string GerarSegmento(char caractere, int potencia)
        {
            if (potencia < 1)
                throw new ArgumentOutOfRangeException(nameof(potencia), "A potência deve ser maior ou igual a 1.");

            return new string(caractere, potencia);
        }
    }
}
