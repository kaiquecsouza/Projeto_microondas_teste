using MicroondasDigital.Domain.Entities;
using MicroondasDigital.Domain.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace MicroondasDigital.Infrastruture.Repositories;

public class JsonProgramaRepository : IProgramaRepository
{
    private readonly string _arquivo;

    public JsonProgramaRepository(string arquivo)
    {
        _arquivo = arquivo;
    }

    public IEnumerable<ProgramaAquecimento> ObterTodos() 
    { 
        return Ler(); 
    }

    public ProgramaAquecimento ObterPorId(int id)
    {
        return Ler().FirstOrDefault(p => p.Id == id);
    }
    public void Adicionar(ProgramaAquecimento programa)
    {
        var programas = Ler().ToList();
        programas.Add(programa);
        Salvar(programas);
    }

    public void Atualizar(ProgramaAquecimento programa) 
    { 
        var lista = Ler(); 
        var indice = lista.FindIndex(p => p.Id == programa.Id); 
        if (indice >= 0) 
        { 
            lista[indice] = programa; 
            Salvar(lista); 
        } 
    }

    public void Excluir(int id)
    {
        var lista = Ler();
        lista.RemoveAll(p => p.Id == id); 
        Salvar(lista);
    }

    public bool ExisteCaractere(char caractere, int? ignorarId = null) 
    { 
        return Ler().Any(p => p.CaractereAquecimento == caractere && (!ignorarId.HasValue || p.Id != ignorarId.Value)); 
    }

    private List<ProgramaAquecimento> Ler() 
    { 
        if (!File.Exists(_arquivo)) 
            return new List<ProgramaAquecimento>();

        var json = File.ReadAllText(_arquivo); 
        if (string.IsNullOrWhiteSpace(json)) 
            return new List<ProgramaAquecimento>();
        
        return JsonConvert.DeserializeObject<List<ProgramaAquecimento>>(json) ?? new List<ProgramaAquecimento>(); 
    }

    private void Salvar(List<ProgramaAquecimento> programas) 
    { 
        var pasta = Path.GetDirectoryName(_arquivo); 
        
        if (!Directory.Exists(pasta)) 
            Directory.CreateDirectory(pasta); 

        var json = JsonConvert.SerializeObject(programas, Formatting.Indented);
        File.WriteAllText(_arquivo, json); 
    }
}
