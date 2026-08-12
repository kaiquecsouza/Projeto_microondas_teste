using System.IO;
using Microondas.Api.Autenticacao;
using Microondas.Api.Middleware;
using Microondas.Dominio.Abstracoes;
using Microondas.Dominio.Servicos;
using Microondas.Infraestrutura.Logging;
using Microondas.Infraestrutura.Persistencia;
using Microondas.Infraestrutura.Seguranca;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------------------------------------
// Configuração de caminhos de dados (persistência e logs).
// ----------------------------------------------------------------------------------
string pastaDados = Path.Combine(builder.Environment.ContentRootPath, "Dados");
Directory.CreateDirectory(pastaDados);

string arquivoProgramas = Path.Combine(pastaDados, "programas-customizados.json");
string arquivoCredenciais = Path.Combine(pastaDados, "credenciais.json");
string arquivoLog = Path.Combine(pastaDados, "erros.log");

var config = builder.Configuration;
string usuarioPadrao = config["Credenciais:UsuarioPadrao"] ?? "admin";
string senhaPadrao = config["Credenciais:SenhaPadrao"] ?? "admin123";
string segredoCripto = config["Seguranca:SegredoCriptografia"] ?? "chave-de-desenvolvimento-trocar-em-producao";

// ----------------------------------------------------------------------------------
// Injeção de dependência (respeitando a Inversão de Dependência - SOLID).
// O domínio depende apenas de abstrações; as implementações concretas vêm da infraestrutura.
// ----------------------------------------------------------------------------------

// Domínio
builder.Services.AddSingleton<IGeradorStringAquecimento, GeradorStringAquecimento>();
// O forno é singleton por representar um único aparelho com estado compartilhado nesta aplicação.
builder.Services.AddSingleton<IFornoMicroondas, FornoMicroondas>();

// Infraestrutura
builder.Services.AddSingleton<IProgramaRepositorio>(_ => new RepositorioProgramasJson(arquivoProgramas));
builder.Services.AddSingleton<ILogadorErros>(_ => new LogadorArquivo(arquivoLog));
builder.Services.AddSingleton<ServicoHash>();
builder.Services.AddSingleton(_ => new ServicoCriptografia(segredoCripto));

// Serviços de aplicação
builder.Services.AddScoped<ProgramaServico>();

// Autenticação
builder.Services.AddSingleton<ServicoToken>();
builder.Services.AddSingleton(sp => new RepositorioCredenciais(
    arquivoCredenciais, sp.GetRequiredService<ServicoHash>(), usuarioPadrao, senhaPadrao));

builder.Services
    .AddAuthentication(AutenticacaoBearerHandler.Esquema)
    .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, AutenticacaoBearerHandler>(
        AutenticacaoBearerHandler.Esquema, _ => { });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

// ----------------------------------------------------------------------------------
// Pipeline HTTP.
// ----------------------------------------------------------------------------------
// Tratamento centralizado de exceções deve ser o primeiro middleware do pipeline.
app.UseMiddleware<MiddlewareTratamentoExcecoes>();

app.UseDefaultFiles();   // serve wwwroot/index.html na raiz
app.UseStaticFiles();    // serve a interface web (wwwroot)

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
