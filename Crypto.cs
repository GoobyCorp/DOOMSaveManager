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
		private const uint C1 = 0xCC9E2D51;
		private const uint C2 = 0x1B873593;
		private const uint C3 = 0xE6546B64;

		public static uint rotr(uint n, int d) => (n >> d) | (n << (32 - d));

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

		public static byte[] CalcProfileHash(byte[] data) {
			byte[] pfData = new byte[data.Length - 0xC];
			Buffer.BlockCopy(data, 0xC, pfData, 0, pfData.Length);

			return FarmHash_Hash32(pfData);
		}

		public static uint Mur(uint a, uint h) {
			// Taken from https://github.com/google/farmhash/blob/master/src/farmhash.cc

			a *= C1;
			a = rotr(a, 17);
			a *= C2;
			h ^= a;
			h = rotr(h, 19);

			return h * 5 + C3;
		}

		public static byte[] FarmHash_Hash32(byte[] data) {
			// Taken from https://github.com/google/farmhash/blob/master/src/farmhash.cc

			uint size = (uint)data.Length;

			uint h = size;
			uint g = C1 * size;
			uint f = g;

			uint a0 = rotr(BitConverter.ToUInt32(data, (int)(size - 4)) * C1, 17) * C2;
			uint a1 = rotr(BitConverter.ToUInt32(data, (int)(size - 8)) * C1, 17) * C2;
			uint a2 = rotr(BitConverter.ToUInt32(data, (int)(size - 12)) * C1, 17) * C2;
			uint a3 = rotr(BitConverter.ToUInt32(data, (int)(size - 16)) * C1, 17) * C2;
			uint a4 = rotr(BitConverter.ToUInt32(data, (int)(size - 20)) * C1, 17) * C2;

			h ^= a0;
			h = rotr(h, 19);
			h = h * 5 + C3;
			h ^= a3;
			h = rotr(h, 19);
			h = h * 5 + C3;

			g ^= a1;
			g = rotr(g, 19);
			g = g * 5 + C3;
			g ^= a2;
			g = rotr(g, 19);
			g = g * 5 + C3;

			f += a4;
			f = rotr(f, 19) + 113;

			uint iters = (size - 1) / 20;

			uint s = 0;
			do {
				uint a = BitConverter.ToUInt32(data, (int)s);
				uint b = BitConverter.ToUInt32(data, (int)s + 4);
				uint c = BitConverter.ToUInt32(data, (int)s + 8);
				uint d = BitConverter.ToUInt32(data, (int)s + 12);
				uint e = BitConverter.ToUInt32(data, (int)s + 16);
				h += a;
				g += b;
				f += c;
				h = Mur(d, h) + e;
				g = Mur(c, g) + a;
				f = Mur(b + e * C1, f) + d;
				f += g;
				g += f;
				s += 20;
			} while (--iters != 0);

			g = rotr(g, 11) * C1;
			g = rotr(g, 17) * C1;
			f = rotr(f, 11) * C1;
			f = rotr(f, 17) * C1;
			h = rotr(h + g, 19);
			h = h * 5 + C3;
			h = rotr(h, 17) * C1;
			h = rotr(h + f, 19);
			h = h * 5 + C3;
			h = rotr(h, 17) * C1;

			byte[] r = BitConverter.GetBytes(h);
			Array.Reverse(r);

			return r;
		}
	}
}