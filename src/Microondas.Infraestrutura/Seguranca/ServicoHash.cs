using System;
using System.Security.Cryptography;
using System.Text;

namespace Microondas.Infraestrutura.Seguranca
{
    /// <summary>
    /// Gera o hash da senha para persistência (Nível 4, item 1.e).
    ///
    /// Observação sobre o requisito: ele pede "SHA1 (256 bits)". SHA-1 produz 160 bits, enquanto
    /// 256 bits corresponde ao SHA-256. Como o número de bits é explícito, adotou-se SHA-256
    /// (também a opção segura e recomendada atualmente). Ver observações no README.
    /// </summary>
    public sealed class ServicoHash
    {
        public string Gerar(string valor)
        {
            if (valor == null)
                throw new ArgumentNullException(nameof(valor));

            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(valor));
                var sb = new StringBuilder(bytes.Length * 2);
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        /// <summary>Compara um valor em texto puro com um hash previamente gerado.</summary>
        public bool Verificar(string valor, string hashEsperado)
        {
            if (string.IsNullOrEmpty(hashEsperado))
                return false;

            string hashCalculado = Gerar(valor);
            // Comparação em tempo fixo para reduzir superfície a ataques de temporização.
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(hashCalculado),
                Encoding.UTF8.GetBytes(hashEsperado));
        }
    }
}
