using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Microondas.Infraestrutura.Seguranca
{
    /// <summary>
    /// Criptografia simétrica (AES-256) reversível, utilizada para proteger valores sensíveis
    /// de configuração, como a connection string (Nível 4, item 1.f). Diferente do hash, é
    /// reversível, pois a connection string precisa ser recuperada em texto claro em tempo de uso.
    ///
    /// A chave deriva de um segredo de configuração via PBKDF2. O IV aleatório é prefixado ao
    /// texto cifrado (formato: Base64(IV + cifra)).
    /// </summary>
    public sealed class ServicoCriptografia
    {
        private static readonly byte[] Salt =
            Encoding.UTF8.GetBytes("Microondas.Salt.v1");

        private readonly byte[] _chave;

        public ServicoCriptografia(string segredo)
        {
            if (string.IsNullOrWhiteSpace(segredo))
                throw new ArgumentException("Segredo de criptografia é obrigatório.", nameof(segredo));

            using (var derivador = new Rfc2898DeriveBytes(segredo, Salt, 100_000, HashAlgorithmName.SHA256))
            {
                _chave = derivador.GetBytes(32); // 256 bits
            }
        }

        public string Criptografar(string textoClaro)
        {
            if (textoClaro == null)
                throw new ArgumentNullException(nameof(textoClaro));

            using (var aes = Aes.Create())
            {
                aes.Key = _chave;
                aes.GenerateIV();

                using (var encriptador = aes.CreateEncryptor())
                using (var memoria = new MemoryStream())
                {
                    memoria.Write(aes.IV, 0, aes.IV.Length);
                    using (var cripto = new CryptoStream(memoria, encriptador, CryptoStreamMode.Write))
                    using (var escritor = new StreamWriter(cripto, Encoding.UTF8))
                    {
                        escritor.Write(textoClaro);
                    }
                    return Convert.ToBase64String(memoria.ToArray());
                }
            }
        }

        public string Descriptografar(string textoCifrado)
        {
            if (string.IsNullOrWhiteSpace(textoCifrado))
                throw new ArgumentException("Texto cifrado é obrigatório.", nameof(textoCifrado));

            byte[] dados = Convert.FromBase64String(textoCifrado);

            using (var aes = Aes.Create())
            {
                aes.Key = _chave;

                int tamanhoIv = aes.BlockSize / 8;
                byte[] iv = new byte[tamanhoIv];
                Array.Copy(dados, 0, iv, 0, tamanhoIv);
                aes.IV = iv;

                using (var decriptador = aes.CreateDecryptor())
                using (var memoria = new MemoryStream(dados, tamanhoIv, dados.Length - tamanhoIv))
                using (var cripto = new CryptoStream(memoria, decriptador, CryptoStreamMode.Read))
                using (var leitor = new StreamReader(cripto, Encoding.UTF8))
                {
                    return leitor.ReadToEnd();
                }
            }
        }
    }
}
