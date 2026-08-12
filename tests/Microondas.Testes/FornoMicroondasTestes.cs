using Microondas.Dominio.Excecoes;
using Microondas.Dominio.Modelos;
using Microondas.Dominio.Servicos;
using Xunit;

namespace Microondas.Testes
{
    public class FornoMicroondasTestes
    {
        private static FornoMicroondas CriarForno() =>
            new FornoMicroondas(new GeradorStringAquecimento());

        [Fact]
        public void Iniciar_ComTempoEPotenciaValidos_IniciaAquecimento()
        {
            var forno = CriarForno();

            var status = forno.Iniciar(90, 5);

            Assert.Equal("EmAndamento", status.Estado);
            Assert.Equal(90, status.TempoRestanteSegundos);
            Assert.Equal(5, status.Potencia);
            Assert.Equal("1:30", status.TempoFormatado); // Nível 1, item 2.c
        }

        [Fact]
        public void Iniciar_SemInformarTempoEPotencia_ExecutaInicioRapido()
        {
            var forno = CriarForno();

            var status = forno.Iniciar(null, null);

            // Nível 1, item 4: início rápido = 30s e potência 10.
            Assert.Equal("EmAndamento", status.Estado);
            Assert.Equal(30, status.TempoRestanteSegundos);
            Assert.Equal(10, status.Potencia);
        }

        [Fact]
        public void Iniciar_SemPotencia_AssumePotenciaPadrao10()
        {
            var forno = CriarForno();

            var status = forno.Iniciar(45, null);

            // Nível 1, itens 2.b/3.c: potência não informada => 10.
            Assert.Equal(10, status.Potencia);
            Assert.Equal(45, status.TempoRestanteSegundos);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(121)]
        [InlineData(-5)]
        public void Iniciar_ComTempoForaDosLimites_LancaExcecao(int tempo)
        {
            var forno = CriarForno();

            var ex = Assert.Throws<RegraNegocioException>(() => forno.Iniciar(tempo, 5));
            Assert.Contains("Tempo inválido", ex.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(11)]
        public void Iniciar_ComPotenciaInvalida_LancaExcecao(int potencia)
        {
            var forno = CriarForno();

            var ex = Assert.Throws<RegraNegocioException>(() => forno.Iniciar(30, potencia));
            Assert.Contains("Potência inválida", ex.Message);
        }

        [Fact]
        public void Iniciar_ComPotenciaMasSemTempo_LancaExcecao()
        {
            var forno = CriarForno();

            Assert.Throws<RegraNegocioException>(() => forno.Iniciar(null, 5));
        }

        [Fact]
        public void Iniciar_DuranteAquecimentoManual_Acrescenta30Segundos()
        {
            var forno = CriarForno();
            forno.Iniciar(30, 5);

            var status = forno.Iniciar(null, null); // Nível 1, item 5

            Assert.Equal("EmAndamento", status.Estado);
            Assert.Equal(60, status.TempoRestanteSegundos);
        }

        [Fact]
        public void Iniciar_DurantePrograma_NaoPermiteAcrescimo()
        {
            var forno = CriarForno();
            var pipoca = new ProgramaAquecimento("Pipoca", "Pipoca", 180, 7, '*', "", false);
            forno.IniciarPrograma(pipoca);

            var ex = Assert.Throws<RegraNegocioException>(() => forno.Iniciar(null, null));
            Assert.Contains("Não é permitido acrescentar tempo", ex.Message); // Nível 2, item 1.e
        }

        [Fact]
        public void PausarOuCancelar_ComAquecimentoEmAndamento_Pausa()
        {
            var forno = CriarForno();
            forno.Iniciar(30, 5);

            var status = forno.PausarOuCancelar();

            Assert.Equal("Pausado", status.Estado);
            Assert.Equal(30, status.TempoRestanteSegundos);
        }

        [Fact]
        public void Iniciar_ComAquecimentoPausado_Retoma()
        {
            var forno = CriarForno();
            forno.Iniciar(30, 5);
            forno.Avancar(); // 29s
            forno.PausarOuCancelar(); // pausa

            var status = forno.Iniciar(null, null); // retoma

            Assert.Equal("EmAndamento", status.Estado);
            Assert.Equal(29, status.TempoRestanteSegundos); // Nível 1, item 7.a
        }

        [Fact]
        public void PausarOuCancelar_ComAquecimentoPausado_Cancela()
        {
            var forno = CriarForno();
            forno.Iniciar(30, 5);
            forno.PausarOuCancelar(); // pausa
            var status = forno.PausarOuCancelar(); // cancela

            // Nível 1, item 7.b: cancela e limpa todas as informações.
            Assert.Equal("Parado", status.Estado);
            Assert.Equal(0, status.TempoRestanteSegundos);
            Assert.Equal("", status.TextoProgresso);
        }

        [Fact]
        public void Avancar_AteOFim_ConcluiComFraseFinal()
        {
            var forno = CriarForno();
            forno.Iniciar(3, 2);

            StatusForno status = null;
            for (int i = 0; i < 3; i++)
                status = forno.Avancar();

            Assert.Equal("Concluido", status.Estado);
            Assert.True(status.Concluido);
            // Potência 2, 3 segundos => "".. .. ..".. + frase.
            Assert.Equal(".. .. .. Aquecimento concluído", status.TextoProgresso);
        }

        [Fact]
        public void Avancar_GeraStringConformePotencia_Exemplo1()
        {
            var forno = CriarForno();
            forno.Iniciar(10, 1);

            StatusForno status = null;
            for (int i = 0; i < 10; i++)
                status = forno.Avancar();

            // Nível 1, item 6.a: tempo 10 / potência 1 => ". . . . . . . . . ."
            Assert.Equal(". . . . . . . . . . Aquecimento concluído", status.TextoProgresso);
        }

        [Fact]
        public void Avancar_GeraStringConformePotencia_Exemplo2()
        {
            var forno = CriarForno();
            forno.Iniciar(5, 3);

            StatusForno status = null;
            for (int i = 0; i < 5; i++)
                status = forno.Avancar();

            // Nível 1, item 6.a: tempo 5 / potência 3 => "... ... ... ... ..."
            Assert.Equal("... ... ... ... ... Aquecimento concluído", status.TextoProgresso);
        }

        [Fact]
        public void IniciarPrograma_ComTempoAcimaDe2Minutos_EhPermitido()
        {
            var forno = CriarForno();
            var carne = new ProgramaAquecimento("Carnes de boi", "Carne", 840, 4, '#', "", false);

            var status = forno.IniciarPrograma(carne);

            // Programas não estão sujeitos ao limite de 2 min do aquecimento manual.
            Assert.Equal("EmAndamento", status.Estado);
            Assert.Equal(840, status.TempoRestanteSegundos);
            Assert.Equal("Carnes de boi", status.ProgramaSelecionado);
        }
    }
}
