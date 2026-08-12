using System.Linq;
using Microondas.Dominio.Excecoes;
using Microondas.Dominio.Servicos;
using Xunit;

namespace Microondas.Testes
{
    public class ProgramaServicoTestes
    {
        private static ProgramaServico CriarServico() =>
            new ProgramaServico(new RepositorioProgramasFake());

        [Fact]
        public void ObterTodos_TrazOsCincoProgramasPredefinidos()
        {
            var servico = CriarServico();

            var todos = servico.ObterTodos();

            Assert.Equal(5, todos.Count);
            Assert.Contains(todos, p => p.Nome == "Pipoca");
            Assert.Contains(todos, p => p.Nome == "Feijão");
        }

        [Fact]
        public void ProgramasPredefinidos_PossuemCaracteresUnicosEDiferentesDePonto()
        {
            var servico = CriarServico();
            var todos = servico.ObterTodos();

            // Nível 2, item 1.b: strings de aquecimento diferenciadas e nunca '.'.
            Assert.DoesNotContain(todos, p => p.Caractere == '.');
            var distintos = todos.Select(p => p.Caractere).Distinct().Count();
            Assert.Equal(todos.Count, distintos);
        }

        [Fact]
        public void Cadastrar_ProgramaValido_AdicionaComoCustomizado()
        {
            var servico = CriarServico();

            var criado = servico.Cadastrar("Lasanha", "Lasanha congelada", 600, 8, '$', "Fure o filme.");

            Assert.True(criado.Customizado);
            Assert.Contains(servico.ObterTodos(), p => p.Nome == "Lasanha" && p.Customizado);
        }

        [Fact]
        public void Cadastrar_ComCaracterePonto_LancaExcecao()
        {
            var servico = CriarServico();

            var ex = Assert.Throws<RegraNegocioException>(
                () => servico.Cadastrar("Arroz", "Arroz", 120, 5, '.', ""));
            Assert.Contains("reservado", ex.Message); // Nível 3, item 1.c
        }

        [Fact]
        public void Cadastrar_ComCaractereJaUtilizado_LancaExcecao()
        {
            var servico = CriarServico();

            // '*' já é usado pelo programa pré-definido "Pipoca".
            var ex = Assert.Throws<RegraNegocioException>(
                () => servico.Cadastrar("Doce", "Doce", 60, 5, '*', ""));
            Assert.Contains("já é utilizado", ex.Message);
        }

        [Fact]
        public void Cadastrar_SemInstrucoes_EhPermitido()
        {
            var servico = CriarServico();

            var criado = servico.Cadastrar("Sopa", "Sopa", 200, 6, '&', null);

            Assert.Equal(string.Empty, criado.Instrucoes); // Nível 3, item 1.b: instruções opcionais
        }

        [Fact]
        public void Remover_ProgramaPredefinido_LancaExcecao()
        {
            var servico = CriarServico();

            var ex = Assert.Throws<RegraNegocioException>(() => servico.Remover("Pipoca"));
            Assert.Contains("não podem ser removidos", ex.Message); // Nível 2, item 1.c
        }

        [Fact]
        public void Remover_ProgramaCustomizado_RemoveComSucesso()
        {
            var servico = CriarServico();
            servico.Cadastrar("Chá", "Água", 90, 4, '+', "");

            servico.Remover("Chá");

            Assert.DoesNotContain(servico.ObterTodos(), p => p.Nome == "Chá");
        }
    }
}
