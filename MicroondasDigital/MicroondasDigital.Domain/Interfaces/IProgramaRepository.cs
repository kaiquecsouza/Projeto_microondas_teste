using MicroondasDigital.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroondasDigital.Domain.Interfaces;

public interface IProgramaRepository
{
    IEnumerable<ProgramaAquecimento> ObterTodos(); 
    ProgramaAquecimento ObterPorId(int id); 
    void Adicionar(ProgramaAquecimento programa); 
    void Atualizar(ProgramaAquecimento programa); 
    void Excluir(int id); 
    bool ExisteCaractere(char caractere, int? ignorarId = null);
}
