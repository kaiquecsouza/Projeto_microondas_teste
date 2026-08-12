namespace Microondas.Dominio.Abstracoes
{
    /// <summary>
    /// Responsável por gerar o segmento da string informativa de aquecimento correspondente a 1 segundo.
    /// Isolado em sua própria abstração para respeitar o SRP e permitir substituição/teste (Strategy).
    /// </summary>
    public interface IGeradorStringAquecimento
    {
        /// <summary>
        /// Gera o segmento referente a um segundo de aquecimento: o <paramref name="caractere"/>
        /// repetido <paramref name="potencia"/> vezes. Ex.: caractere '.', potência 3 => "...".
        /// </summary>
        string GerarSegmento(char caractere, int potencia);
    }
}
