using System;
using System.Text;

using System.Security.Cryptography;

using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security.Policy;

namespace DOOMSaveManager
{
	public class Pair<T1, T2> {
		public T1 First { get; set; }
		public T2 Second { get; set; }

		public Pair() { }

		public Pair(T1 v1, T2 v2) {
			First = v1;
			Second = v2;
		}
	}

	public static class Crypto
    {
		private const uint C1 = 0xCC9E2D51;
		private const uint C2 = 0x1B873593;
		private const uint C3 = 0xE6546B64;

		private const ulong K0 = 0xC3A5C85C97CB3127;
		private const ulong K1 = 0xB492B66FBE98F273;
		private const ulong K2 = 0x9AE16A3B2F90404F;

		private struct uint128_t
		{
			public ulong first, second;
			public uint128_t(ulong first, ulong second) {
				this.first = first;
				this.second = second;
			}
		}

		public static uint rotr32(uint n, int d) => (n >> d) | (n << (32 - d));
		public static ulong rotr64(ulong n, int d) => (n >> d) | (n << (64 - d));
		public static uint Rotate32(uint n, int d) => rotr32(n, d);
		public static ulong Rotate64(ulong n, int d) => rotr64(n, d);

		public static unsafe uint Fetch32(byte* p) => *(uint*)p;
		public static unsafe ulong Fetch64(byte* p) => *(ulong*)p;

		private static uint128_t Uint128(ulong lo, ulong hi) => new uint128_t(lo, hi);

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

		public static unsafe byte[] CalcProfileHash(byte[] data) {
			byte[] pfData = new byte[data.Length - 0xC];
			Buffer.BlockCopy(data, 0xC, pfData, 0, pfData.Length);

			fixed(byte* s = pfData) {
				return Hash32(s, (uint)pfData.Length);
			}
		}

		private static ulong ShiftMix(ulong val) => val ^ (val >> 47);
		private static byte[] GetBytes32(uint val) {
			byte[] data = BitConverter.GetBytes(val);
			Array.Reverse(data);

			return data;
		}
		private static byte[] GetBytes64(ulong val) {
			byte[] data = BitConverter.GetBytes(val);
			Array.Reverse(data);

			return data;
		}

		private static ulong Uint128Low64(uint128_t x) => x.first;
		private static ulong Uint128High64(uint128_t x) => x.second;

		private static ulong Hash128to64(uint128_t x) {
			// Taken from https://github.com/google/farmhash/blob/master/src/farmhash.h

			const ulong kMul = 0x9DDFEA08EB382D69;
			ulong a = (Uint128Low64(x) ^ Uint128High64(x)) * kMul;
			a ^= (a >> 47);
			ulong b = (Uint128High64(x) ^ a) * kMul;
			b ^= (b >> 47);
			b *= kMul;
			return b;
		}

		private static uint fmix(uint h) {
			// Taken from https://github.com/google/farmhash/blob/master/src/farmhash.cc

			h ^= h >> 16;
			h *= 0x85EBCA6B;
			h ^= h >> 13;
			h *= 0xC2B2AE35;
			h ^= h >> 16;

			return h;
		}

		private static uint Mur(uint a, uint h) {
			// Taken from https://github.com/google/farmhash/blob/master/src/farmhash.cc

			a *= C1;
			a = rotr32(a, 17);
			a *= C2;
			h ^= a;
			h = rotr32(h, 19);

			return h * 5 + C3;
		}

		private static unsafe uint Hash32Len0to4(byte* s, uint len, uint seed = 0) {
			// Taken from https://github.com/google/farmhash/blob/master/src/farmhash.cc

			uint b = seed;
			uint c = 9;
			for (uint i = 0; i < len; i++) {
				byte v = s[i];
				b = b * C1 + v;
				c ^= b;
			}
			return fmix(Mur(b, Mur(len, c)));
		}

		private static unsafe uint Hash32Len5to12(byte* s, uint len, uint seed = 0) {
			// Taken from https://github.com/google/farmhash/blob/master/src/farmhash.cc

			uint a = len, b = len * 5, c = 9, d = b + seed;
			a += Fetch32(s);
			b += Fetch32(s + len - 4);
			c += Fetch32(s + ((len >> 1) & 4));
			return fmix(seed ^ Mur(c, Mur(b, Mur(a, d))));
		}

		private static unsafe uint Hash32Len13to24(byte* s, uint len, uint seed = 0) {
			// Taken from https://github.com/google/farmhash/blob/master/src/farmhash.cc

			uint a = Fetch32(s - 4 + (len >> 1));
			uint b = Fetch32(s + 4);
			uint c = Fetch32(s + len - 8);
			uint d = Fetch32(s + (len >> 1));
			uint e = Fetch32(s);
			uint f = Fetch32(s + len - 4);
			uint h = d * C1 + len + seed;
			a = Rotate32(a, 12) + f;
			h = Mur(c, h) + a;
			a = Rotate32(a, 3) + c;
			h = Mur(e, h) + a;
			a = Rotate32(a + f, 12) + d;
			h = Mur(b ^ seed, h) + a;
			return fmix(h);
		}

		public static unsafe byte[] Hash32(byte* s, uint len) {
			// Taken from https://github.com/google/farmhash/blob/master/src/farmhash.cc

			if(0 <= len && len <= 4)
				return GetBytes32(Hash32Len0to4(s, len));
			else if(5 <= len && len <= 12)
				return GetBytes32(Hash32Len5to12(s, len));
			else if(13 <= len && len <= 24)
				return GetBytes32(Hash32Len13to24(s, len));

			uint h = len;
			uint g = C1 * len;
			uint f = g;

			uint a0 = Rotate32(Fetch32(s + (len - 4)) * C1, 17) * C2;
			uint a1 = Rotate32(Fetch32(s + (len - 8)) * C1, 17) * C2;
			uint a2 = Rotate32(Fetch32(s + (len - 12)) * C1, 17) * C2;
			uint a3 = Rotate32(Fetch32(s + (len - 16)) * C1, 17) * C2;
			uint a4 = Rotate32(Fetch32(s + (len - 20)) * C1, 17) * C2;

			h ^= a0;
			h = rotr32(h, 19);
			h = h * 5 + C3;
			h ^= a3;
			h = rotr32(h, 19);
			h = h * 5 + C3;

			g ^= a1;
			g = rotr32(g, 19);
			g = g * 5 + C3;
			g ^= a2;
			g = rotr32(g, 19);
			g = g * 5 + C3;

			f += a4;
			f = rotr32(f, 19) + 113;

			uint iters = (len - 1) / 20;

			do {
				uint a = Fetch32(s);
				uint b = Fetch32(s + 4);
				uint c = Fetch32(s + 8);
				uint d = Fetch32(s + 12);
				uint e = Fetch32(s + 16);
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

			g = Rotate32(g, 11) * C1;
			g = Rotate32(g, 17) * C1;
			f = Rotate32(f, 11) * C1;
			f = Rotate32(f, 17) * C1;
			h = Rotate32(h + g, 19);
			h = h * 5 + C3;
			h = Rotate32(h, 17) * C1;
			h = Rotate32(h + f, 19);
			h = h * 5 + C3;
			h = Rotate32(h, 17) * C1;

			return GetBytes32(h);
		}

		private static uint128_t WeakHashLen32WithSeeds(ulong w, ulong x, ulong y, ulong z, ulong a, ulong b) {
			// Taken from https://github.com/google/farmhash/blob/master/src/farmhash.cc

			a += w;
			b = Rotate64(b + a + z, 21);
			ulong c = a;
			a += x;
			a += y;
			b += Rotate64(a, 44);

			return new uint128_t(a + z, b + c);
		}

		private static unsafe uint128_t WeakHashLen32WithSeeds(byte* s, ulong a, ulong b) {
			// Taken from https://github.com/google/farmhash/blob/master/src/farmhash.cc

			return WeakHashLen32WithSeeds(Fetch64(s), Fetch64(s + 8), Fetch64(s + 16), Fetch64(s + 24), a, b);
		}

		private static ulong HashLen16(ulong u, ulong v, ulong mul) {
			// Taken from https://github.com/google/farmhash/blob/master/src/farmhash.cc

			ulong a = (u ^ v) * mul;
			a ^= (a >> 47);
			ulong b = (v ^ a) * mul;
			b ^= (b >> 47);
			b *= mul;

			return b;
		}

		private static ulong HashLen16(ulong u, ulong v) {
			// Taken from https://github.com/google/farmhash/blob/master/src/farmhash.cc

			return Hash128to64(Uint128(u, v));
		}

		private static unsafe ulong HashLen0to16(byte* s, uint len) {
			// Taken from https://github.com/google/farmhash/blob/master/src/farmhash.cc

			if(len >= 8) {
				ulong mul = K2 + (ulong)len * 2;
				ulong a = Fetch64(s) + K2;
				ulong b = Fetch64(s + (len - 8));
				ulong c = Rotate64(b, 37) * mul + a;
				ulong d = (Rotate64(a, 25) + b) * mul;

				return HashLen16(c, d, mul);
			}
			if(len >= 4) {
				ulong mul = K2 + (ulong)len * 2;
				ulong a = Fetch32(s);

				return HashLen16(len + (a << 3), Fetch32(s + (len - 4)), mul);
			}
			if(len > 0) {
				byte a = s[0];
				byte b = s[len >> 1];
				byte c = s[len - 1];
				uint y = a + ((uint)b << 8);
				uint z = len + ((uint)c << 2);

				return ShiftMix(y * K2 ^ z * K0) * K2;
			}
			return K2;
		}

		private static unsafe ulong HashLen17to32(byte* s, uint len) {
			// Taken from https://github.com/google/farmhash/blob/master/src/farmhash.cc

			ulong mul = K2 + (ulong)len * 2;
			ulong a = Fetch64(s) * K1;
			ulong b = Fetch64(s + 8);
			ulong c = Fetch64(s + (len - 8)) * mul;
			ulong d = Fetch64(s + (len - 16)) * mul;

			return HashLen16(Rotate64(a + b, 43) + Rotate64(c, 30) + d, a + Rotate64(b + K2, 18) + c, mul);
		}

		private static unsafe ulong HashLen33to64(byte* s, uint len) {
			// Taken from https://github.com/google/farmhash/blob/master/src/farmhash.cc

			ulong mul = K2 + len * 2;
			ulong a = Fetch64(s) * K2;
			ulong b = Fetch64(s + 8);
			ulong c = Fetch64(s + len - 8) * mul;
			ulong d = Fetch64(s + len - 16) * K2;
			ulong y = Rotate64(a + b, 43) + Rotate64(c, 30) + d;
			ulong z = HashLen16(y, a + Rotate64(b + K2, 18) + c, mul);
			ulong e = Fetch64(s + 16) * mul;
			ulong f = Fetch64(s + 24);
			ulong g = (y + Fetch64(s + len - 32)) * mul;
			ulong h = (z + Fetch64(s + len - 24)) * mul;

			return HashLen16(Rotate64(e + f, 43) + Rotate64(g, 30) + h, e + Rotate64(f + a, 18) + g, mul);
		}

		public static unsafe byte[] Hash64(byte* s, uint len) {
			// Taken from https://github.com/google/farmhash/blob/master/src/farmhash.cc

			const ulong seed = 81;

			ulong tmp_x;

			if(0 <= len && len <= 16)
				return GetBytes64(HashLen0to16(s, len));
			else if(17 <= len && len <= 32)
				return GetBytes64(HashLen17to32(s, len));
			else if(33 <= len && len <= 64)
				return GetBytes64(HashLen33to64(s, len));

			ulong x = seed;
			ulong y = unchecked(seed * K1 + 113);
			ulong z = ShiftMix(y * K2 + 113) * K2;
			uint128_t v = new uint128_t(0, 0);
			uint128_t w = new uint128_t(0, 0);
			x = x * K2 + Fetch64(s);

			byte* end = s + (len - 1) / 64 * 64;
			byte* last64 = end + ((len - 1) & 63) - 63;
			do {
				x = Rotate64(x + y + v.first + Fetch64(s + 8), 37) * K1;
				y = Rotate64(y + v.second + Fetch64(s + 48), 42) * K1;
				x ^= w.second;
				y += v.first + Fetch64(s + 40);
				z = Rotate64(z + w.first, 33) * K1;
				v = WeakHashLen32WithSeeds(s, v.second * K1, x + w.first);
				w = WeakHashLen32WithSeeds(s + 32, z + w.second, y + Fetch64(s + 16));

				// swap
				tmp_x = x;
				x = z;
				z = tmp_x;

				s += 64;
			} while (s != end);

			ulong mul = K1 + ((z & 0xff) << 1);
			// Make s point to the last 64 bytes of input.
			s = last64;
			w.first += ((len - 1) & 63);
			v.first += w.first;
			w.first += v.first;
			x = Rotate64(x + y + v.first + Fetch64(s + 8), 37) * mul;
			y = Rotate64(y + v.second + Fetch64(s + 48), 42) * mul;
			x ^= w.second * 9;
			y += v.first * 9 + Fetch64(s + 40);
			z = Rotate64(z + w.first, 33) * mul;
			v = WeakHashLen32WithSeeds(s, v.second * mul, x + w.first);
			w = WeakHashLen32WithSeeds(s + 32, z + w.second, y + Fetch64(s + 16));

			// swap
			tmp_x = x;
			x = z;
			z = tmp_x;

			return GetBytes64(HashLen16(HashLen16(v.first, w.first, mul) + ShiftMix(y) * K0 + z, HashLen16(v.second, w.second, mul) + x, mul));
		}

		private static unsafe byte[] MurmurHash64B(byte* data, uint len, uint seed) {
			// Taken from http://www.cs.cmu.edu/afs/cs/project/cmt-55/lti/Courses/731/homework/mosesdecoder/kenlm/util/murmur_hash.cc

			const uint m = 0x5bd1e995;
			const int r = 24;

			uint h1 = seed ^ len;
			uint h2 = 0;

			while(len >= 8)
			{
				uint k1 = *data++;
				k1 *= m;
				k1 ^= k1 >> r;
				k1 *= m;
				h1 *= m;
				h1 ^= k1;
				len -= 4;

				uint k2 = *data++;
				k2 *= m;
				k2 ^= k2 >> r;
				k2 *= m;
				h2 *= m;
				h2 ^= k2;
				len -= 4;
			}

			if(len >= 4)
			{
				uint k1 = *data++;
				k1 *= m; k1 ^= k1 >> r; k1 *= m;
				h1 *= m; h1 ^= k1;
				len -= 4;
			}

			switch(len)
			{
				case 3: {
					h2 ^= (uint)data[2] << 16;
					break;
				}
				case 2: {
					h2 ^= (uint)data[1] << 8;
					break;
				}
				case 1: {
					h2 ^= data[0];
					h2 *= m;
					break;
				}
			};

			h1 ^= h2 >> 18;
			h1 *= m;
			h2 ^= h1 >> 22;
			h2 *= m;
			h1 ^= h2 >> 17;
			h1 *= m;
			h2 ^= h1 >> 19;
			h2 *= m;

			ulong h = h1;

			h = (h << 32) | h2;

			return GetBytes64(h);
		}

		private static unsafe uint128_t CityMurmur(byte* s, uint len, uint128_t seed) {
			// Taken from https://github.com/google/farmhash/blob/master/src/farmhash.cc

			ulong a = Uint128Low64(seed);
			ulong b = Uint128High64(seed);
			ulong c = 0;
			ulong d = 0;
			long l = len - 16;
			if (l <= 0) {  // len <= 16
				a = ShiftMix(a* K1) * K1;
				c = b* K1 + HashLen0to16(s, len);
				d = ShiftMix(a + (len >= 8 ? Fetch64(s) : c));
			} else {  // len > 16
				c = HashLen16(Fetch64(s + len - 8) + K1, a);
				d = HashLen16(b + len, c + Fetch64(s + len - 16));
				a += d;
				do {
					a ^= ShiftMix(Fetch64(s) * K1) * K1;
					a *= K1;
					b ^= a;
					c ^= ShiftMix(Fetch64(s + 8) * K1) * K1;
					c *= K1;
					d ^= c;
					s += 16;
					l -= 16;
				} while (l > 0);
			}
			a = HashLen16(a, c);
			b = HashLen16(d, b);
			return Uint128(a ^ b, HashLen16(b, a));
		}

		private static unsafe uint128_t CityHash128WithSeed(byte* s, uint len, uint128_t seed) {
			// Taken from https://github.com/google/farmhash/blob/master/src/farmhash.cc

			if (len < 128)
				return CityMurmur(s, len, seed);

			ulong tmp_x;

			// We expect len >= 128 to be the common case.  Keep 56 bytes of state:
			// v, w, x, y, and z.
			uint128_t v = new uint128_t(0, 0);
			uint128_t w = new uint128_t(0, 0);
			ulong x = Uint128Low64(seed);
			ulong y = Uint128High64(seed);
			ulong z = len * K1;
			v.first = Rotate64(y ^ K1, 49) * K1 + Fetch64(s);
			v.second = Rotate64(v.first, 42) * K1 + Fetch64(s + 8);
			w.first = Rotate64(y + z, 35) * K1 + x;
			w.second = Rotate64(x + Fetch64(s + 88), 53) * K1;

			// This is the same inner loop as CityHash64(), manually unrolled.
			do {
				x = Rotate64(x + y + v.first + Fetch64(s + 8), 37) * K1;
				y = Rotate64(y + v.second + Fetch64(s + 48), 42) * K1;
				x ^= w.second;
				y += v.first + Fetch64(s + 40);
				z = Rotate64(z + w.first, 33) * K1;
				v = WeakHashLen32WithSeeds(s, v.second* K1, x + w.first);
				w = WeakHashLen32WithSeeds(s + 32, z + w.second, y + Fetch64(s + 16));

				tmp_x = x;
				x = z;
				z = tmp_x;

				s += 64;
				x = Rotate64(x + y + v.first + Fetch64(s + 8), 37) * K1;
				y = Rotate64(y + v.second + Fetch64(s + 48), 42) * K1;
				x ^= w.second;
				y += v.first + Fetch64(s + 40);
				z = Rotate64(z + w.first, 33) * K1;
				v = WeakHashLen32WithSeeds(s, v.second* K1, x + w.first);
				w = WeakHashLen32WithSeeds(s + 32, z + w.second, y + Fetch64(s + 16));

				tmp_x = x;
				x = z;
				z = tmp_x;

				s += 64;
				len -= 128;
			} while (len >= 128);
			x += Rotate64(v.first + z, 49) * K0;
			y = y* K0 + Rotate64(w.second, 37);
			z = z* K0 + Rotate64(w.first, 27);
			w.first *= 9;
			v.first *= K0;

			// If 0 < len < 128, hash up to 4 chunks of 32 bytes each from the end of s.
			for (uint tail_done = 0; tail_done < len;) {
				tail_done += 32;
				y = Rotate64(x + y, 42) * K0 + v.second;
				w.first += Fetch64(s + len - tail_done + 16);
				x = x* K0 + w.first;
				z += w.second + Fetch64(s + len - tail_done);
				w.second += v.first;
				v = WeakHashLen32WithSeeds(s + len - tail_done, v.first + z, v.second);
				v.first *= K0;
			}
			// At this point our 56 bytes of state should contain more than
			// enough information for a strong 128-bit hash.  We use two
			// different 56-byte-to-8-byte hashes to get a 16-byte final result.
			x = HashLen16(x, v.first);
			y = HashLen16(y + z, w.first);
			return Uint128(HashLen16(x + v.second, w.second) + y, HashLen16(x + w.second, y + v.second));
		}

		public static unsafe byte[] CityHash128(byte* s, uint len) {
			// Taken from https://github.com/google/farmhash/blob/master/src/farmhash.cc

			uint128_t res;
			if (len >= 16) {
				res = CityHash128WithSeed(s + 16, len - 16, Uint128(Fetch64(s), Fetch64(s + 8) + K0));
			} else {
				res = CityHash128WithSeed(s, len, Uint128(K0, K1));
			}
			byte[] resBytes = new byte[16];
			byte[] firstBytes = GetBytes64(res.first);
			byte[] secondBytes = GetBytes64(res.second);
			Buffer.BlockCopy(firstBytes, 0, resBytes, 0, firstBytes.Length);
			Buffer.BlockCopy(secondBytes, 0, resBytes, firstBytes.Length, secondBytes.Length);
			return resBytes;
		}

		public static unsafe byte[] MurmurHash64B(byte[] data, uint seed) {
			fixed(byte* s = data) {
				return MurmurHash64B(s, (uint)data.Length, seed);
			}
		}

		public static unsafe byte[] Fingerprint32(byte[] data) {
			fixed(byte* s = data) {
				return Hash32(s, (uint)data.Length);
			}
		}

		public static unsafe byte[] Fingerprint64(byte[] data) {
			fixed(byte* s = data) {
				return Hash64(s, (uint)data.Length);
			}
		}

		public static unsafe byte[] Fingerprint128(byte[] data) {
			fixed (byte* s = data) {
				return CityHash128(s, (uint)data.Length);
			}
		}
	}
}