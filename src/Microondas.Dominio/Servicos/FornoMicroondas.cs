using System;
using System.Text;
using Microondas.Dominio.Abstracoes;
using Microondas.Dominio.Configuracao;
using Microondas.Dominio.Enums;
using Microondas.Dominio.Excecoes;
using Microondas.Dominio.Modelos;

namespace Microondas.Dominio.Servicos
{
    /// <summary>
    /// Núcleo de negócio do micro-ondas. Implementado como máquina de estados protegida por
    /// bloqueio (lock), pois na Web o avanço do tempo é acionado por uma thread de timer,
    /// enquanto comandos (iniciar, pausar) podem chegar concorrentemente.
    ///
    /// O estado interno (tempo, potência, caractere, progresso) é totalmente encapsulado:
    /// o mundo externo só interage por meio dos métodos públicos e observa o resultado via
    /// <see cref="StatusForno"/>, prevenindo uso incorreto (requisito I).
    /// </summary>
    public sealed class FornoMicroondas : IFornoMicroondas
    {
        private readonly IGeradorStringAquecimento _gerador;
        private readonly object _trava = new object();
        private readonly StringBuilder _progresso = new StringBuilder();

        private EstadoForno _estado = EstadoForno.Parado;
        private int _tempoRestante;
        private int _potencia;
        private char _caractere = RegrasAquecimento.CaractereProgressoPadrao;
        private string _programaSelecionado;
        private bool _ehPrograma;

        public FornoMicroondas(IGeradorStringAquecimento gerador)
        {
            _gerador = gerador ?? throw new ArgumentNullException(nameof(gerador));
        }

        public StatusForno Iniciar(int? tempoSegundos, int? potencia)
        {
            lock (_trava)
            {
                switch (_estado)
                {
                    // Pausado + iniciar => retoma do ponto em que parou (Nível 1, item 7.a).
                    case EstadoForno.Pausado:
                        _estado = EstadoForno.EmAndamento;
                        break;

                    // Em andamento + iniciar => acrescenta 30s ao tempo restante (Nível 1, item 5).
                    case EstadoForno.EmAndamento:
                        AcrescentarTempo();
                        break;

                    // Parado/Concluído => novo aquecimento manual (ou início rápido).
                    default:
                        IniciarAquecimentoManual(tempoSegundos, potencia);
                        break;
                }

                return MontarStatus();
            }
        }

        public StatusForno IniciarPrograma(ProgramaAquecimento programa)
        {
            if (programa == null)
                throw new ArgumentNullException(nameof(programa));

            lock (_trava)
            {
                if (_estado == EstadoForno.EmAndamento || _estado == EstadoForno.Pausado)
                    throw new RegraNegocioException(
                        "Já existe um aquecimento em andamento. Cancele-o antes de iniciar um programa.");

                _tempoRestante = programa.TempoSegundos;
                _potencia = programa.Potencia;
                _caractere = programa.Caractere;
                _programaSelecionado = programa.Nome;
                _ehPrograma = true;
                _progresso.Clear();
                _estado = EstadoForno.EmAndamento;

                return MontarStatus();
            }
        }

        public StatusForno PausarOuCancelar()
        {
            lock (_trava)
            {
                switch (_estado)
                {
                    // Em andamento => pausa (Nível 1, item 7.a).
                    case EstadoForno.EmAndamento:
                        _estado = EstadoForno.Pausado;
                        break;

                    // Pausado => cancela e limpa tudo (Nível 1, item 7.b).
                    // Parado/Concluído => apenas limpa os dados de tela (Nível 1, item 7.c).
                    default:
                        LimparInterno();
                        break;
                }

                return MontarStatus();
            }
        }

        public StatusForno Avancar()
        {
            lock (_trava)
            {
                if (_estado != EstadoForno.EmAndamento)
                    return MontarStatus();

                // Acrescenta o segmento referente a este segundo, separado por espaço.
                if (_progresso.Length > 0)
                    _progresso.Append(' ');
                _progresso.Append(_gerador.GerarSegmento(_caractere, _potencia));

                _tempoRestante--;

                if (_tempoRestante <= 0)
                    Concluir();

                return MontarStatus();
            }
        }

        public StatusForno ObterStatus()
        {
            lock (_trava)
            {
                return MontarStatus();
            }
        }

        public StatusForno Resetar()
        {
            lock (_trava)
            {
                LimparInterno();
                return MontarStatus();
            }
        }

        // ----------------- Métodos privados de apoio -----------------

        private void IniciarAquecimentoManual(int? tempoSegundos, int? potencia)
        {
            int tempo;
            int pot;

            bool inicioRapido = !tempoSegundos.HasValue && !potencia.HasValue;
            if (inicioRapido)
            {
                // Início rápido: potência 10 e tempo 30s (Nível 1, item 4).
                tempo = RegrasAquecimento.TempoInicioRapidoSegundos;
                pot = RegrasAquecimento.PotenciaPadrao;
            }
            else
            {
                if (!tempoSegundos.HasValue)
                    throw new RegraNegocioException(
                        "Informe um tempo válido, entre 1 segundo e 2 minutos.");

                tempo = tempoSegundos.Value;
                // Potência não informada assume o padrão 10 (Nível 1, itens 2.b e 3.c).
                pot = potencia ?? RegrasAquecimento.PotenciaPadrao;

                ValidarTempo(tempo);
                ValidarPotencia(pot);
            }

            _tempoRestante = tempo;
            _potencia = pot;
            _caractere = RegrasAquecimento.CaractereProgressoPadrao;
            _programaSelecionado = null;
            _ehPrograma = false;
            _progresso.Clear();
            _estado = EstadoForno.EmAndamento;
        }

        private void AcrescentarTempo()
        {
            // Programas (pré-definidos/customizados) não permitem acréscimo (Nível 2, item 1.e).
            if (_ehPrograma)
                throw new RegraNegocioException(
                    "Não é permitido acrescentar tempo em programas de aquecimento.");

            _tempoRestante += RegrasAquecimento.AcrescimoSegundos;
        }

        private void Concluir()
        {
            _tempoRestante = 0;
            _estado = EstadoForno.Concluido;

            // Ao final, concatena a frase de conclusão (Nível 1, item 6.b).
            if (_progresso.Length > 0)
                _progresso.Append(' ');
            _progresso.Append(RegrasAquecimento.FraseConclusao);
        }

        private void LimparInterno()
        {
            _estado = EstadoForno.Parado;
            _tempoRestante = 0;
            _potencia = 0;
            _caractere = RegrasAquecimento.CaractereProgressoPadrao;
            _programaSelecionado = null;
            _ehPrograma = false;
            _progresso.Clear();
        }

        private static void ValidarTempo(int tempo)
        {
            if (tempo < RegrasAquecimento.TempoMinimoSegundos || tempo > RegrasAquecimento.TempoMaximoSegundos)
                throw new RegraNegocioException(
                    "Tempo inválido. Informe um valor entre 1 segundo e 2 minutos (120 segundos).");
        }

        private static void ValidarPotencia(int potencia)
        {
            if (potencia < RegrasAquecimento.PotenciaMinima || potencia > RegrasAquecimento.PotenciaMaxima)
                throw new RegraNegocioException(
                    "Potência inválida. Informe um valor entre 1 e 10.");
        }

        private StatusForno MontarStatus()
        {
            return new StatusForno(
                estado: _estado.ToString(),
                tempoRestanteSegundos: _tempoRestante,
                tempoFormatado: FormatadorTempo.Formatar(_tempoRestante),
                potencia: _potencia,
                textoProgresso: _progresso.ToString(),
                programaSelecionado: _programaSelecionado,
                ehPrograma: _ehPrograma,
                concluido: _estado == EstadoForno.Concluido);
        }
    }
}
