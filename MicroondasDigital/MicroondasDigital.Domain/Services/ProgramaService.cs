using MicroondasDigital.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroondasDigital.Domain.Services;

public class ProgramaService
{
    private readonly List<ProgramaAquecimento> _preDefinidos;

    public ProgramaService()
    {
        _preDefinidos = new List<ProgramaAquecimento>
        {
            new ProgramaAquecimento(1, "Pipoca", "Pipoca de micro-ondas", 180, 7, '*', "Observar o intervalo entre os estouros.", true), 
            new ProgramaAquecimento(2, "Leite", "Leite", 300, 5, '~', "Cuidado com fervura e choque térmico.", true), 
            new ProgramaAquecimento(3, "Carnes de boi", "Carne em pedaço/fatias", 840, 4, '#', "Virar na metade.", true), 
            new ProgramaAquecimento(4, "Frango", "Qualquer corte", 480, 7, '>', "Virar na metade.", true), 
            new ProgramaAquecimento(5, "Feijão", "Feijão congelado", 480, 9, '+', "Recipiente destampado; cuidado ao retirar.", true)
        };
    }

    public IEnumerable<ProgramaAquecimento> ObterPreDefinidos()
    {
        return _preDefinidos;
    }

    public ProgramaAquecimento ObterPreDefinido(int id)
    {
        return _preDefinidos.FirstOrDefault(p => p.Id == id);
    }
}
