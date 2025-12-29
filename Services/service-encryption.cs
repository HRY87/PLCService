using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ControlplastPLCService.Services
{
    public class EncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;
        
        public EncryptionService(string masterKey)
        {
            // Generar key e IV desde master key
            using var sha256 = SHA256.Create();
            var keyHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(masterKey));
            _key = keyHash;
            
            var ivHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(masterKey + "_IV"));
            _iv = new byte[16];
            Array.Copy(ivHash, _iv, 16);
        }
        
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;
            
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }
            
            return Convert.ToBase64String(msEncrypt.ToArray());
        }
        
        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;
            
            try
            {
                var buffer = Convert.FromBase64String(cipherText);
                
                using var aes = Aes.Create();
                aes.Key = _key;
                aes.IV = _iv;
                
                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                
                using var msDecrypt = new MemoryStream(buffer);
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);
                
                return srDecrypt.ReadToEnd();
            }
            catch
            {
                return cipherText; // Si falla, devolver el texto original
            }
        }
    }
}