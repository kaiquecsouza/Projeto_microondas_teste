using MicroondasDigital.Domain.Entities;
Console.WriteLine("===== TESTES MANUAIS DO MICRO-ONDAS ====="); 

// TESTE 1
Console.WriteLine("\n--- TESTE 1: Iniciar(90, 5) ---"); 
var microondas = new Microondas(); 
microondas.Iniciar(90, 5);
Console.WriteLine($"Estado: {microondas.Estado}");
Console.WriteLine($"Tempo: {microondas.TempoRestante}");
Console.WriteLine($"Formatado: {microondas.ObterTempoFormatado()}");
Console.WriteLine($"Potência: {microondas.Potencia}"); 

// TESTE 2
Console.WriteLine("\n--- TESTE 2: InicioRapido() ---"); 
microondas = new Microondas(); 
microondas.InicioRapido(); 
Console.WriteLine($"Estado: {microondas.Estado}");
Console.WriteLine($"Tempo: {microondas.TempoRestante}");
Console.WriteLine($"Potência: {microondas.Potencia}"); 

// TESTE 3
Console.WriteLine("\n--- TESTE 3: ProcessarSegundo() ---"); 
microondas = new Microondas();
microondas.InicioRapido(); 
Console.WriteLine($"Antes: {microondas.TempoRestante}"); 
microondas.ProcessarSegundo(); 
Console.WriteLine($"Depois: {microondas.TempoRestante}");
Console.WriteLine($"Indicador: {microondas.StringAquecimento}"); 

// TESTE 4
Console.WriteLine("\n--- TESTE 4: Pausar/Cancelar ---"); 
microondas = new Microondas();
microondas.InicioRapido(); 
microondas.PausarOuCancelar(); 
Console.WriteLine($"Após pausar: {microondas.Estado}");
Console.WriteLine($"Tempo: {microondas.TempoRestante}"); 
microondas.PausarOuCancelar();
Console.WriteLine($"Após cancelar: {microondas.Estado}");
Console.WriteLine($"Tempo: {microondas.TempoRestante}");

// TESTE 5
Console.WriteLine("\n--- TESTE 5: Acrescentar 30 segundos ---"); 
microondas = new Microondas(); 
microondas.Iniciar(60, 5); 
Console.WriteLine($"Antes: {microondas.TempoRestante}"); 
microondas.Iniciar(20, 3); 
Console.WriteLine($"Depois: {microondas.TempoRestante}"); 
Console.WriteLine($"Potência: {microondas.Potencia}"); 
Console.WriteLine("\n===== FIM DOS TESTES ====="); 

Console.ReadKey();