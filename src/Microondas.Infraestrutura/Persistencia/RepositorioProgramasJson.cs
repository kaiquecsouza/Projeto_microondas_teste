using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microondas.Dominio.Abstracoes;
using Microondas.Dominio.Modelos;

namespace Microondas.Infraestrutura.Persistencia
{
    /// <summary>
    /// Persistência dos programas customizados em arquivo JSON (Nível 3, item 1.e).
    /// Thread-safe por meio de bloqueio; recarrega o arquivo a cada operação para simplicidade
    /// e consistência em um cenário de baixo volume.
    /// </summary>
    public sealed class RepositorioProgramasJson : IProgramaRepositorio
    {
        private readonly string _caminhoArquivo;
        private readonly object _trava = new object();

        private static readonly JsonSerializerOptions OpcoesJson = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public RepositorioProgramasJson(string caminhoArquivo)
        {
            if (string.IsNullOrWhiteSpace(caminhoArquivo))
                throw new ArgumentException("Caminho do arquivo de persistência é obrigatório.", nameof(caminhoArquivo));

            _caminhoArquivo = caminhoArquivo;
            GarantirDiretorio();
        }

        public IReadOnlyList<ProgramaAquecimento> ObterCustomizados()
        {
            lock (_trava)
            {
                return Carregar().Select(ParaDominio).ToList();
            }
        }

        public void Adicionar(ProgramaAquecimento programa)
        {
            if (programa == null)
                throw new ArgumentNullException(nameof(programa));

            lock (_trava)
            {
                var registros = Carregar();
                registros.Add(ProgramaRegistro.De(programa));
                Salvar(registros);
            }
        }

        public bool Remover(string nome)
        {
            lock (_trava)
            {
                var registros = Carregar();
                int removidos = registros.RemoveAll(r =>
                    string.Equals(r.Nome, nome, StringComparison.OrdinalIgnoreCase));

                if (removidos > 0)
                    Salvar(registros);

                return removidos > 0;
            }
        }

        private List<ProgramaRegistro> Carregar()
        {
            if (!File.Exists(_caminhoArquivo))
                return new List<ProgramaRegistro>();

            string conteudo = File.ReadAllText(_caminhoArquivo);
            if (string.IsNullOrWhiteSpace(conteudo))
                return new List<ProgramaRegistro>();

            return JsonSerializer.Deserialize<List<ProgramaRegistro>>(conteudo, OpcoesJson)
                   ?? new List<ProgramaRegistro>();
        }

        private void Salvar(List<ProgramaRegistro> registros)
        {
            string conteudo = JsonSerializer.Serialize(registros, OpcoesJson);
            File.WriteAllText(_caminhoArquivo, conteudo);
        }

        private void GarantirDiretorio()
        {
            string diretorio = Path.GetDirectoryName(Path.GetFullPath(_caminhoArquivo));
            if (!string.IsNullOrEmpty(diretorio) && !Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);
        }

        private static ProgramaAquecimento ParaDominio(ProgramaRegistro r)
        {
            return new ProgramaAquecimento(
                r.Nome, r.Alimento, r.TempoSegundos, r.Potencia,
                string.IsNullOrEmpty(r.Caractere) ? '?' : r.Caractere[0],
                r.Instrucoes, customizado: true);
        }

        /// <summary>DTO de persistência. Char é serializado como string para robustez no JSON.</summary>
        public sealed class ProgramaRegistro
        {
            public string Nome { get; set; }
            public string Alimento { get; set; }
            public int TempoSegundos { get; set; }
            public int Potencia { get; set; }
            public string Caractere { get; set; }
            public string Instrucoes { get; set; }

            public static ProgramaRegistro De(ProgramaAquecimento p)
            {
                return new ProgramaRegistro
                {
                    Nome = p.Nome,
                    Alimento = p.Alimento,
                    TempoSegundos = p.TempoSegundos,
                    Potencia = p.Potencia,
                    Caractere = p.Caractere.ToString(),
                    Instrucoes = p.Instrucoes
                };
            }
        }
    }
}
