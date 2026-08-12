using MicroondasDigital.Domain.Entities;
using MicroondasDigital.Domain.Enums;
using MicroondasDigital.Domain.Exceptions;

namespace MicroondasDigital.Tests;

[TestClass]
public class MicroondasTests
{
    [TestMethod]
    public void InicioRapido_DeveUsar30SegundosEPotencia10()
    {
        var microondas = new Microondas();
        microondas.InicioRapido();

        Assert.AreEqual(30, microondas.TempoRestante);
        Assert.AreEqual(10, microondas.Potencia);
        Assert.AreEqual(EstadoMicroondas.Aquecendo, microondas.Estado);
    }

    [TestMethod]
    public void Iniciar_90Segundos_DeveFormatarComo1MinutoE30()
    {
        var microondas = new Microondas();
        microondas.Iniciar(90, 5);

        Assert.AreEqual("1:30", microondas.ObterTempoFormatado());
    }

    [TestMethod]
    public void Pausar_DeveAlterarEstadoParaPausado()
    {
        var microondas = new Microondas();
        microondas.InicioRapido();
        microondas.PausarOuCancelar();

        Assert.AreEqual(EstadoMicroondas.Pausado, microondas.Estado);
    }

    [TestMethod]
    public void PorcessarSegundo_DeveReduzirTempo()
    {
        var microondas = new Microondas();
        microondas.Iniciar(10,3);
        microondas.ProcessarSegundo();

        Assert.AreEqual(9, microondas.TempoRestante);
        Assert.IsTrue(microondas.StringAquecimento.StartsWith("..."));
    }

    [TestMethod]
    public void IniciarEnquantoAquecendo_DeveAcrescentar30Segundos()
    {
        var microondas = new Microondas();
        microondas.Iniciar(30, 5);
        microondas.Iniciar(30, 5);
        Assert.AreEqual(60, microondas.TempoRestante);
    }

    [TestMethod]
    [ExpectedException(typeof(RegraNegocioException))]
    public void TempoAcimaDe120_NoManual_DeveGerarErro()
    {
        var microondas = new Microondas();
        microondas.Iniciar(121, 5);
    }

    [TestMethod]
    public void PotenciaForaDoIntervalo_DeveGerarErro()
    {
        Assert.ThrowsException<RegraNegocioException>(() =>
        {
            var microondas = new Microondas();
            microondas.Iniciar(30, 11);
        });
    }
}
