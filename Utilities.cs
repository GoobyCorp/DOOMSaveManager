using System;

using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Win32;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace DOOMSaveManager
{
    public static class Utilities
    {
        public static string GetSteamPath() {
            using (var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Valve\Steam")) {
                return (string)reg.GetValue("InstallPath", null);
            }
        }

        public static byte[] RandomBytes(int size) {
            byte[] output = new byte[size];
            new Random().NextBytes(output);
            return output;
        }

        public static bool ByteArraysEqual(byte[] b1, byte[] b2) {
            return b1.SequenceEqual(b2);
        }

        public static ulong Id3ToId64(uint sid3) {
            return sid3 + 76561197960265728UL;
        }

        public static string Id3ToId64(string sid3) {
            return (ulong.Parse(sid3) + 76561197960265728UL).ToString();
        }

        public static ulong Id64ToId3(uint sid3) {
            return sid3 - 76561197960265728UL;
        }

        public static string Id64ToId3(string sid64) {
            return (ulong.Parse(sid64) - 76561197960265728UL).ToString();
        }

        public static string GetSavePathForId64(ulong sid64) {
            return Path.Combine(DoomEternal.SteamSavePath, (sid64 - 76561197960265728).ToString(), DoomEternal.SteamGameID.ToString(), "remote");
        }

        public static bool CheckUUID(string s) {
            Regex re = new Regex("^[0-9a-f]{8}-?[0-9a-f]{4}-?[0-9a-f]{4}-?[0-9a-f]{4}-?[0-9a-f]{12}$", RegexOptions.IgnoreCase);
            return re.IsMatch(s);
        }

        public static string Hexlify(byte[] data) {
            return BitConverter.ToString(data).Replace("-", "").ToUpper();
        }

        public static void PrintHex(byte[] data) {
            Console.WriteLine(Hexlify(data));
        }
    }
}