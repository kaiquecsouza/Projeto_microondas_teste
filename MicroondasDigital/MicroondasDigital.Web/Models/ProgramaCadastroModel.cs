namespace MicroondasDigital.Web.Models;
using System.ComponentModel.DataAnnotations;

public class ProgramaCadastroModel
{
    [Required]
    public string Nome { get; set; }
    [Required]
    public string Alimento { get; set; }
    [Range(1, int.MaxValue)] 
    public int Tempo { get; set; }
    [Range(1, 10)] 
    public int Potencia { get; set; }
    [Required] 
    public string Caractere { get; set; }
    public string Instrucoes { get; set; }

}
