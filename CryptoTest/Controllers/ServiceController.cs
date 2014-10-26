using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace CryptoTest.Controllers
{
    public class ServiceController : ApiController
    {
        public string Decrypt(InputViewModel model)
        {
            var cipherText = Convert.FromBase64String(model.CipherText);
            var password = Encoding.UTF8.GetBytes(model.Password);
            var salt = Convert.FromBase64String(model.Salt);

            var crypto = new SimpleCrypto(password, salt);

            var iv = Convert.FromBase64String(model.Iv);
            if (!Enumerable.SequenceEqual(iv, crypto.IV))
            {
                throw new Exception("IVs do not match");
            }

            var key = Convert.FromBase64String(model.Key);
            if (!Enumerable.SequenceEqual(key, crypto.Key)) {
                throw new Exception("Keys do not match");
            }


            var plainText = crypto.Decrypt(cipherText);

            return Encoding.UTF8.GetString(plainText);
        }
    }

    public class InputViewModel
    {
        public string CipherText { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }

        public string Key { get; set; }
        public string Iv { get; set; }
    }

    public class SimpleCrypto
    {
        private readonly Encoding encoder = Encoding.Default;
        private readonly SymmetricAlgorithm algorithm;

        private const int ivBitLength = 128;
        private const int keyBitLength = 256;

        private const int Iterations = 1000;

        public SimpleCrypto(byte[] password, byte[] salt)
        {
            algorithm = Rijndael.Create();
            algorithm.Padding = PaddingMode.PKCS7;
            algorithm.Mode = CipherMode.CBC;
            algorithm.BlockSize = 128;
            algorithm.KeySize = 256;

            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations)) {
                algorithm.IV = rfc2898DeriveBytes.GetBytes(ivBitLength / 8);
                algorithm.Key = rfc2898DeriveBytes.GetBytes(keyBitLength / 8);
            }
        }

        public byte[] IV {
            get { return algorithm.IV; }
        }

        public byte[] Key
        {
            get { return algorithm.Key; }
        }

        private static byte[] Transform(byte[] bytes, Func<ICryptoTransform> selectCryptoTransform)
        {
            using (var memoryStream = new MemoryStream()) {
                using (var cryptoStream = new CryptoStream(memoryStream, selectCryptoTransform(), CryptoStreamMode.Write)) {
                    cryptoStream.Write(bytes, 0, bytes.Length);
                }
                return memoryStream.ToArray();
            }
        }

        public string Encrypt(string unencrypted)
        {
            return Convert.ToBase64String(Encrypt(encoder.GetBytes(unencrypted)));
        }

        public string Decrypt(string encrypted)
        {
            return encoder.GetString(Decrypt(Convert.FromBase64String(encrypted)));
        }

        public byte[] Encrypt(byte[] buffer)
        {
            return Transform(buffer, algorithm.CreateEncryptor);
        }

        public byte[] Decrypt(byte[] buffer)
        {
            return Transform(buffer, algorithm.CreateDecryptor);
        }

    }
}
