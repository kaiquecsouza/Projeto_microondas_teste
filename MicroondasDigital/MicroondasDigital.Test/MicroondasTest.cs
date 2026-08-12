using MicroondasDigital.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace MicroondasDigital.Test;

[TestClass]
public class MicroondasTest
{
    [TestMethod]
    public void InicioRapido_DeveUsar30SegundosEPotencia10()
    {
        var microondas = new Microondas();
        microondas.InicioRapido();

        Assert.AreEqual(30, microondas.TempoRestante);
    }
}
