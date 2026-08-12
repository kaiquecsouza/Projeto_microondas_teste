using MicroondasDigital.Domain.Entities;
using MicroondasDigital.Domain.Exceptions;
using MicroondasDigital.Domain.Interfaces;
using System.Linq;

namespace MicroondasDigital.Domain.Services;

public class ProgramaCustomizadoService
{
    private readonly IProgramaRepository _repository;

    public ProgramaCustomizadoService(IProgramaRepository repository)
    {
        _repository = repository;
    }

    public void Adicionar(string nome, string alimento, int tempo, int potencia, char caractere, string instrucoes)
    {
        Validar(nome, alimento, tempo, potencia, caractere, null);

        var proximoId = _repository.ObterTodos().Any() ? _repository.ObterTodos().Max(p => p.Id) + 1 : 100;

        var programa = new ProgramaAquecimento(proximoId, nome, alimento, tempo, potencia, caractere, instrucoes, false);

        _repository.Adicionar(programa);
    }

    public IEnumerable<ProgramaAquecimento> ListarTodos()
    {
        return _repository.ObterTodos();
    }

    private void Validar(string nome, string alimento, int tempo, int potencia, char caractere, int? id)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new RegraNegocioException("Informe o nome do programa.");
        if (string.IsNullOrWhiteSpace(alimento))
            throw new RegraNegocioException("Informe o alimento.");
        if (tempo <= 0)
            throw new RegraNegocioException("O tempo deve ser maior que zero.");
        if (potencia < 1 || potencia > 10)
            throw new RegraNegocioException("A potência deve estar entre 1 e 10.");
        if (caractere == '.')
            throw new RegraNegocioException("O caractere '.' é reservado.");
        if (_repository.ExisteCaractere(caractere, id))
            throw new RegraNegocioException("O caractere já está sendo utilizado.");
    }
}