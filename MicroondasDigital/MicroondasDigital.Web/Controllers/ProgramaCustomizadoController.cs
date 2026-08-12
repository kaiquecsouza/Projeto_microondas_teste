using MicroondasDigital.Domain.Exceptions;
using MicroondasDigital.Domain.Services;
using MicroondasDigital.Infrastruture.Repositories;
using MicroondasDigital.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace MicroondasDigital.Web.Controllers;

public class ProgramaCustomizadoController : Controller
{
    private readonly IWebHostEnvironment _env;

    public ProgramaCustomizadoController(IWebHostEnvironment env)
    {
        _env = env;
    }

    private ProgramaCustomizadoService MontarService()
    {
        var caminho = Path.Combine(_env.ContentRootPath, "App_Data", "programas-customizados.json");
        var repository = new JsonProgramaRepository(caminho);
        return new ProgramaCustomizadoService(repository);
    }

    public ActionResult Index()
    {
        var service = MontarService();
        var programas = service.ListarTodos(); 
        return View(programas);
    }

    [HttpGet]
    public ActionResult CadastrarPrograma()
    {
        return View(new ProgramaCadastroModel());
    }

    [HttpPost]
    public ActionResult CadastrarPrograma(ProgramaCadastroModel model)
    {
        if (!ModelState.IsValid) return View(model);
        try
        {
            var service = MontarService();
            service.Adicionar(model.Nome, model.Alimento, model.Tempo, model.Potencia, model.Caractere[0], model.Instrucoes);
            return RedirectToAction("Index");
        }
        catch (RegraNegocioException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }
}