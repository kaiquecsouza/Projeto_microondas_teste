using Microondas.Dominio.Servicos;
using Xunit;

namespace Microondas.Testes
{
    public class GeradorStringAquecimentoTestes
    {
        [Theory]
        [InlineData('.', 1, ".")]
        [InlineData('.', 3, "...")]
        [InlineData('*', 7, "*******")]
        public void GerarSegmento_RepeteCaractereConformePotencia(char caractere, int potencia, string esperado)
        {
            var gerador = new GeradorStringAquecimento();

            Assert.Equal(esperado, gerador.GerarSegmento(caractere, potencia));
        }
    }

    public class FormatadorTempoTestes
    {
        [Theory]
        [InlineData(90, "1:30")]  // exemplo do Nível 1, item 2.c
        [InlineData(30, "0:30")]
        [InlineData(120, "2:00")]
        [InlineData(0, "0:00")]
        [InlineData(5, "0:05")]
        public void Formatar_ConverteSegundosEmMinutosSegundos(int segundos, string esperado)
        {
            Assert.Equal(esperado, FormatadorTempo.Formatar(segundos));
        }
    }
}
