namespace MicroondasDigital.Web.Models;

public class MicroondasModel
{
    public int Tempo { get; set; }
    public int? Potencia { get; set; }
    public int TempoRestante { get; set; }
    public string TempoFormatado { get; set; }
    public int PotenciaAtual { get; set; }
    public string Estado { get; set; }
    public string StringAquecimento { get; set; }
    public string MensagemErro { get; set; }
}
