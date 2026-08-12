namespace Microondas.Api.Contratos
{
    /// <summary>Requisição de início de aquecimento manual. Campos nulos acionam início rápido/padrão.</summary>
    public sealed class IniciarAquecimentoRequest
    {
        public int? TempoSegundos { get; set; }
        public int? Potencia { get; set; }
    }

    /// <summary>Requisição de cadastro de programa customizado (Nível 3).</summary>
    public sealed class CadastroProgramaRequest
    {
        public string Nome { get; set; }
        public string Alimento { get; set; }
        public int TempoSegundos { get; set; }
        public int Potencia { get; set; }
        public string Caractere { get; set; }
        public string Instrucoes { get; set; }
    }

    /// <summary>Credenciais para configuração (Nível 4, item 1.d) e login.</summary>
    public sealed class CredenciaisRequest
    {
        public string Usuario { get; set; }
        public string Senha { get; set; }
    }

    /// <summary>Envelope padronizado de resposta de erro (Nível 4, item 2.a).</summary>
    public sealed class RespostaErro
    {
        public bool Sucesso { get; }
        public string Mensagem { get; }
        public string Tipo { get; }

        public RespostaErro(string mensagem, string tipo)
        {
            Sucesso = false;
            Mensagem = mensagem;
            Tipo = tipo;
        }
    }
}
