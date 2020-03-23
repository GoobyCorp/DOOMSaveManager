using System;
using System.Text;

using System.Security.Cryptography;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace DOOMSaveManager
{
    public static class Crypto
    {
        public static byte[] EncryptAndDigest(string aad, byte[] data) {
            byte[] nonce = Utilities.RandomBytes(12);
            byte[] aadBytes = Encoding.UTF8.GetBytes(aad);
            byte[] aadHash = new SHA256Managed().ComputeHash(aadBytes);
            var cipher = new GcmBlockCipher(new AesEngine());
            var cParams = new AeadParameters(new KeyParameter(aadHash, 0, 16), 128, nonce, aadBytes);
            cipher.Init(true, cParams);
            byte[] ciphertext = new byte[cipher.GetOutputSize(data.Length)];
            int retLen = cipher.ProcessBytes(data, 0, data.Length, ciphertext, 0);
            cipher.DoFinal(ciphertext, retLen);

            byte[] output = new byte[nonce.Length + ciphertext.Length];
            Buffer.BlockCopy(nonce, 0, output, 0, nonce.Length);
            Buffer.BlockCopy(ciphertext, 0, output, nonce.Length, ciphertext.Length);
            return output;
        }

        public static byte[] DecryptAndVerify(string aad, byte[] data) {
            byte[] nonce = new byte[12];
            byte[] ciphertext = new byte[data.Length - 12];
            Buffer.BlockCopy(data, 0, nonce, 0, nonce.Length);
            Buffer.BlockCopy(data, nonce.Length, ciphertext, 0, ciphertext.Length);

            byte[] aadBytes = Encoding.UTF8.GetBytes(aad);
            byte[] aadHash = new SHA256Managed().ComputeHash(aadBytes);
            var cipher = new GcmBlockCipher(new AesEngine());
            var cParams = new AeadParameters(new KeyParameter(aadHash, 0, 16), 128, nonce, aadBytes);
            cipher.Init(false, cParams);
            byte[] plaintext = new byte[cipher.GetOutputSize(ciphertext.Length)];
            int retLen = cipher.ProcessBytes(ciphertext, 0, ciphertext.Length, plaintext, 0);
            cipher.DoFinal(plaintext, retLen);

            return plaintext;
        }
    }
}