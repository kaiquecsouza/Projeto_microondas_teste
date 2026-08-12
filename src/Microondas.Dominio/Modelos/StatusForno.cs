namespace Microondas.Dominio.Modelos
{
    /// <summary>
    /// Fotografia (snapshot) imutável do estado atual do forno, exposta para a interface/API.
    /// Não expõe os campos internos do forno, apenas o necessário para exibição.
    /// </summary>
    public sealed class StatusForno
    {
        public string Estado { get; }
        public int TempoRestanteSegundos { get; }
        public string TempoFormatado { get; }
        public int Potencia { get; }
        public string TextoProgresso { get; }
        public string ProgramaSelecionado { get; }
        public bool EhPrograma { get; }
        public bool Concluido { get; }

        public StatusForno(
            string estado,
            int tempoRestanteSegundos,
            string tempoFormatado,
            int potencia,
            string textoProgresso,
            string programaSelecionado,
            bool ehPrograma,
            bool concluido)
        {
            Estado = estado;
            TempoRestanteSegundos = tempoRestanteSegundos;
            TempoFormatado = tempoFormatado;
            Potencia = potencia;
            TextoProgresso = textoProgresso;
            ProgramaSelecionado = programaSelecionado;
            EhPrograma = ehPrograma;
            Concluido = concluido;
        }
    }
}
