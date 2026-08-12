using MicroondasDigital.Domain.Entities;
using MicroondasDigital.Domain.Exceptions;
using MicroondasDigital.Domain.Services;
using MicroondasDigital.Infrastruture.Repositories;
using MicroondasDigital.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MicroondasDigital.Web.Controllers;

public class MicroondasController : Controller
{
    private const string SessionKey = "MICROONDAS";
    private readonly ProgramaService _programaService = new ProgramaService();
    private readonly IWebHostEnvironment _env;

    public MicroondasController(IWebHostEnvironment env)
    {
        _env = env;
    }

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

    private ProgramaCustomizadoService MontarProgramaCustomizadoService()
    {
        var caminho = Path.Combine(_env.ContentRootPath, "App_Data", "programas-customizados.json");
        var repository = new JsonProgramaRepository(caminho);
        return new ProgramaCustomizadoService(repository);
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
        var programa = _programaService.ObterPreDefinido(id);
        if (programa == null)
        {
            return NotFound();
        }

        var microondas = ObterMicroondas();
        microondas.IniciarPrograma(programa.Tempo, programa.Potencia, programa.CaractereAquecimento);
        SalvarMicroondas(microondas);

        return Json(CriarViewModel(microondas));
    }

    private MicroondasModel CriarViewModel(Microondas m)
    {
        var viewModel = new MicroondasModel
        {
            TempoRestante = m.TempoRestante,
            TempoFormatado = m.ObterTempoFormatado(),
            PotenciaAtual = m.Potencia,
            Estado = m.Estado.ToString(),
            StringAquecimento = m.StringAquecimento
        };

        var service = MontarProgramaCustomizadoService();
        viewModel.ProgramasCustomizados = service.ListarTodos()
            .Select(p => new ProgramaResumoModel { Id = p.Id, Nome = p.Nome })
            .ToList();

        return viewModel;
    }
}