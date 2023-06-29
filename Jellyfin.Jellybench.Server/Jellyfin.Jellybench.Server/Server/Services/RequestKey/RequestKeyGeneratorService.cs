using Jellyfin.Jellybench.Server.Server.Options;
using Microsoft.Extensions.Options;
using ServiceLocator.Attributes;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jellyfin.Jellybench.Server.Server.Services.RequestKey
{
    public interface IRequestKeyGeneratorService
    {
        Task<bool> ValidateKey(string key);
        Task<string> GenerateKey();
    }

    [TransientService(typeof(IRequestKeyGeneratorService))]
    public class RequestKeyGeneratorService : IRequestKeyGeneratorService
    {
        private readonly IOptions<RequestKeyOptions> _requestKeyOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestKeyGeneratorService(IOptions<RequestKeyOptions> requestKeyOptions,
            IHttpContextAccessor httpContextAccessor)
        {
            _requestKeyOptions = requestKeyOptions;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> ValidateKey(string key)
        {
            return true;
        }

        public async Task<string> GenerateKey()
        {
            var builder = new RequestKey()
            {
                CallerIp = _httpContextAccessor.HttpContext.Connection.LocalIpAddress.ToString(),
                CreatedAt = DateTimeOffset.UtcNow,
                Seed = Random.Shared.NextDouble().ToString()
            };
            using var outputStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(outputStream, builder).ConfigureAwait(false);
            return string.Join("",
                CryptoHelper.Encrypt(_requestKeyOptions.Value.Secret, outputStream.ToArray())
                    .Select(e => e.ToString("X2")));
        }

        private record RequestKey
        {
            public string Seed { get; set; }
            public DateTimeOffset CreatedAt { get; set; }
            public string CallerIp { get; set; }
        }

        public static class CryptoHelper
        {
            private static SymmetricAlgorithm GetCryproProvider(string key)
            {
                var symmetricAlgorithm = Aes.Create();
                byte[] keyValue = key.Select(e => (byte)e).ToArray();
                symmetricAlgorithm.Key = keyValue;
                return symmetricAlgorithm;
            }

            public static byte[] Encrypt(string key, byte[] plainString)
            {
                using var cryptoProvider = GetCryproProvider(key);
                using var resultStream = new MemoryStream();
                resultStream.Write(cryptoProvider.IV, 0, cryptoProvider.IV.Length);
                using (CryptoStream cryptoStream = new(
                           resultStream,
                           cryptoProvider.CreateEncryptor(),
                           CryptoStreamMode.Write))
                {
                    // By default, the StreamWriter uses UTF-8 encoding.
                    // To change the text encoding, pass the desired encoding as the second parameter.
                    // For example, new StreamWriter(cryptoStream, Encoding.Unicode).
                    cryptoStream.Write(plainString);
                }
                return resultStream.ToArray();
            }

            public static async Task<string> Decrypt(string key, byte[] encryptedString)
            {
                byte[] keyValue = key.Select(e => (byte)e).ToArray();
                using var cryptoProvider = GetCryproProvider(key);
                using var resultStream = new MemoryStream();
                using var sourceStream = new MemoryStream(encryptedString);

                byte[] iv = new byte[cryptoProvider.IV.Length];
                int numBytesToRead = cryptoProvider.IV.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    int n = sourceStream.Read(iv, numBytesRead, numBytesToRead);
                    if (n == 0) break;

                    numBytesRead += n;
                    numBytesToRead -= n;
                }

                await using CryptoStream cryptoStream = new(
                    sourceStream,
                    cryptoProvider.CreateDecryptor(keyValue, iv),
                    CryptoStreamMode.Read);
                // By default, the StreamReader uses UTF-8 encoding.
                // To change the text encoding, pass the desired encoding as the second parameter.
                // For example, new StreamReader(cryptoStream, Encoding.Unicode).
                using StreamReader decryptReader = new(cryptoStream);
                string decryptedMessage = await decryptReader.ReadToEndAsync().ConfigureAwait(false);
                return decryptedMessage;
            }
        }
    }
}
