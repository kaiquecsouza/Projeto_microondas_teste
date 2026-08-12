using MicroondasDigital.Domain.Entities;
using MicroondasDigital.Domain.Exceptions;
using MicroondasDigital.Domain.Services;
using MicroondasDigital.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MicroondasDigital.Web.Controllers;

public class MicroondasController : Controller
{
    private const string SessionKey = "MICROONDAS";
    private readonly ProgramaService _programaService = new ProgramaService();

    private Microondas ObterMicroondas()
    {
        var json = HttpContext.Session.GetString(SessionKey);
        Microondas? microondas = null;

        if (!string.IsNullOrEmpty(json))
        {
            microondas = JsonSerializer.Deserialize<Microondas>(json);
        }

        if (microondas == null)
        {
            microondas = new Microondas();
            SalvarMicroondas(microondas);
        }

        return microondas;
    }

    private void SalvarMicroondas(Microondas microondas)
    {
        var json = JsonSerializer.Serialize(microondas);
        HttpContext.Session.SetString(SessionKey, json);
    }

    public IActionResult Index()
    {
        return View(CriarViewModel(ObterMicroondas()));
    }

    [HttpPost]
    public IActionResult Iniciar(int tempo, int? potencia)
    {
        var microondas = ObterMicroondas();
        try
        {
            microondas.Iniciar(tempo, potencia);
            SalvarMicroondas(microondas);
            return Json(CriarViewModel(microondas));
        }
        catch (RegraNegocioException ex)
        {
            Response.StatusCode = 400;
            return Json(new { mensagem = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult InicioRapido()
    {
        var microondas = ObterMicroondas();
        try
        {
            microondas.InicioRapido();
            SalvarMicroondas(microondas);
            return Json(CriarViewModel(microondas));
        }
        catch (RegraNegocioException ex)
        {
            Response.StatusCode = 400;
            return Json(new { mensagem = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult ProcessarSegundo()
    {
        var microondas = ObterMicroondas();
        microondas.ProcessarSegundo();
        SalvarMicroondas(microondas);
        return Json(CriarViewModel(microondas));
    }

    [HttpPost]
    public IActionResult PausarCancelar()
    {
        var microondas = ObterMicroondas();
        microondas.PausarOuCancelar();
        SalvarMicroondas(microondas);
        return Json(CriarViewModel(microondas));
    }

    [HttpPost]
    public IActionResult IniciarPrograma(int id)
    {
        var programas = _programaService.ObterPreDefinido(id);
        if(programas == null)
        {
            return NotFound();
        }

        var microondas = ObterMicroondas();
        microondas.IniciarPrograma(programas.Tempo, programas.Potencia, programas.CaractereAquecimento);

        return Json(CriarViewModel(microondas));
    }

    private MicroondasModel CriarViewModel(Microondas m)
    {
        return new MicroondasModel
        {
            TempoRestante = m.TempoRestante,
            TempoFormatado = m.ObterTempoFormatado(),
            PotenciaAtual = m.Potencia,
            Estado = m.Estado.ToString(),
            StringAquecimento = m.StringAquecimento
        };
    }
}
