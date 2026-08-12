using MicroondasDigital.Domain.Enums;
using MicroondasDigital.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroondasDigital.Domain.Entities;

public class Microondas
{
    private const int TempoMinimo = 1;

    private const int TempoMaximoManual = 120;

    private const int PotenciaPadrao = 10;

    public int TempoRestante { get; private set; }

    public int Potencia { get; private set; }

    public EstadoMicroondas Estado { get; private set; }

    public string StringAquecimento { get; private set; }

    public char CaractereAquecimento { get; private set; }

    public bool PermiteAcrescimo { get; private set; }


    public Microondas()

    {

        Limpar();

    }


    public void Iniciar(int tempo, int? potencia = null)

    {

        if (Estado == EstadoMicroondas.Aquecendo)

        {

            Acrescentar30Segundos();

            return;

        }


        if (Estado == EstadoMicroondas.Pausado)

        {

            Estado = EstadoMicroondas.Aquecendo;

            return;

        }


        int potenciaUtilizada = potencia ?? PotenciaPadrao;

        ValidarTempoManual(tempo);

        ValidarPotencia(potenciaUtilizada);

        TempoRestante = tempo;

        Potencia = potenciaUtilizada;

        CaractereAquecimento = '.';

        PermiteAcrescimo = true;

        StringAquecimento = string.Empty;

        Estado = EstadoMicroondas.Aquecendo;

    }


    public void InicioRapido()

    {

        Iniciar(30, 10);

    }


    public void ProcessarSegundo()

    {

        if (Estado != EstadoMicroondas.Aquecendo)

        {

            return;

        }

        if (TempoRestante <= 0)

        {

            return;

        }

        TempoRestante--;

        StringAquecimento += GerarIndicador() + " ";


        if (TempoRestante == 0)

        {

            Estado = EstadoMicroondas.Concluido;

            StringAquecimento += "Aquecimento concluído!";

        }

    }


    public void PausarOuCancelar()

    {

        if (Estado == EstadoMicroondas.Aquecendo)

        {

            Estado = EstadoMicroondas.Pausado;

            return;

        }

        if (Estado == EstadoMicroondas.Pausado)

        {

            Cancelar();

            return;

        }


        Limpar();

    }


    private void Cancelar()

    {

        Limpar();

    }


    private string GerarIndicador()

    {

        return new string(CaractereAquecimento, Potencia);

    }


    public void ValidarPotencia(int potencia)

    {

        if (potencia < 1 || potencia > 10)

            throw new RegraNegocioException($"A potência informada é inválida. Informe uma potência entre 1 e 10.");

    }


    private void ValidarTempoManual(int tempo)

    {

        if (tempo < TempoMinimo || tempo > TempoMaximoManual)

        {

            throw new RegraNegocioException($"O tempo informado é inválido. Informe um tempo entre 1 e 120 segundos.");

        }

    }


    public void Acrescentar30Segundos()

    {

        if (Estado != EstadoMicroondas.Aquecendo)

        {

            return;

        }


        if (!PermiteAcrescimo)

            throw new RegraNegocioException("Não é permitido acrescentar tempo ao microondas.");


        TempoRestante += 30;

    }


    public string ObterTempoFormatado()

    {

        int minutos = TempoRestante / 60;

        int segundos = TempoRestante % 60;

        return string.Format("{0}:{1:00}", minutos, segundos);

    }


    private void Limpar()

    {

        TempoRestante = 0;

        Potencia = PotenciaPadrao;

        Estado = EstadoMicroondas.Parado;

        StringAquecimento = string.Empty;

        PermiteAcrescimo = true;

        CaractereAquecimento = '.';

    }


    //Essa etapa será utilizada no Nivel 2
    public void IniciarPrograma(int tempo, int potencia, char caractere)

    {

        if(tempo <= 0)
            throw new RegraNegocioException("O tempo do programa deve ser maior que zero.");


        ValidarPotencia(potencia);


        TempoRestante = tempo;

        Potencia = potencia;

        CaractereAquecimento = caractere;

        PermiteAcrescimo = false;

        StringAquecimento = string.Empty;

        Estado = EstadoMicroondas.Aquecendo;

    }

}
