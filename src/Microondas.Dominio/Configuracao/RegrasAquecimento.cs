namespace Microondas.Dominio.Configuracao
{
    /// <summary>
    /// Centraliza as constantes que definem as regras de aquecimento.
    /// Manter os "números mágicos" em um único ponto facilita manutenção e leitura (boas práticas).
    /// </summary>
    public static class RegrasAquecimento
    {
        /// <summary>Tempo mínimo permitido para aquecimento manual (1 segundo).</summary>
        public const int TempoMinimoSegundos = 1;

        /// <summary>Tempo máximo permitido para aquecimento manual (2 minutos).</summary>
        public const int TempoMaximoSegundos = 120;

        /// <summary>Potência mínima válida.</summary>
        public const int PotenciaMinima = 1;

        /// <summary>Potência máxima válida.</summary>
        public const int PotenciaMaxima = 10;

        /// <summary>Potência assumida quando o usuário não a informa.</summary>
        public const int PotenciaPadrao = 10;

        /// <summary>Tempo assumido no "início rápido".</summary>
        public const int TempoInicioRapidoSegundos = 30;

        /// <summary>Acréscimo aplicado a cada acionamento do botão iniciar durante o aquecimento manual.</summary>
        public const int AcrescimoSegundos = 30;

        /// <summary>Caractere padrão da string de progresso. Reservado: não pode ser usado por programas.</summary>
        public const char CaractereProgressoPadrao = '.';

        /// <summary>Frase concatenada ao final da string de progresso quando o aquecimento termina.</summary>
        public const string FraseConclusao = "Aquecimento concluído";
    }
}
