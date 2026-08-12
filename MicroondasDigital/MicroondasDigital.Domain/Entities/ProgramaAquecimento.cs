using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroondasDigital.Domain.Entities;

public class ProgramaAquecimento
{
    public int Id { get; private set; } 
    public string Nome { get; private set; } 
    public string Alimento { get; private set; } 
    public int Tempo { get; private set; } 
    public int Potencia { get; private set; } 
    public char CaractereAquecimento { get; private set; } 
    public string Instrucoes { get; private set; } 
    public bool PreDefinido { get; private set; } 
    public ProgramaAquecimento(int id, string nome, string alimento, int tempo, int potencia, char caractere, string instrucoes, bool preDefinido) 
    { 
        Id = id; Nome = nome; 
        Alimento = alimento; 
        Tempo = tempo; 
        Potencia = potencia; 
        CaractereAquecimento = caractere; 
        Instrucoes = instrucoes; 
        PreDefinido = preDefinido; 
    }
}
