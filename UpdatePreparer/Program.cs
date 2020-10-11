using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace UpdatePreparer
{
    class Program
    {
        public static string GetSwitcherHash(string fname)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            FileStream stream = File.OpenRead(fname);

            md5.ComputeHash(stream);

            stream.Close();

            string rtrn = "";
            for (int i = 0; i < md5.Hash.Length; i++) rtrn += (md5.Hash[i].ToString("x2"));

            return rtrn.ToUpper();
        }

        public static string GetVersionHash(string fname, string version)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(string.Format(@"AkemiSwitcher{0}{1}damedanedameyo{0}bakamitai{1}versaosaopaolo{2}haxormoment{3}", 2030 / 2 + 5 - 20 * 2, 2020, version, GetSwitcherHash(fname))));

            string rtrn = "";
            for (int i = 0; i < md5.Hash.Length; i++) rtrn += (md5.Hash[i].ToString("x2"));

            return rtrn.ToUpper();
        }

        static void Main(string[] args)
        {
            Console.Write("Full path to AkemiSwitcher Release EXE: ");
            string f = Console.ReadLine();
            if (f.Contains("\"")) f = f.Replace("\"", "");

            string version = AssemblyName.GetAssemblyName(f).Version.ToString();
            string Hash = GetSwitcherHash(f);
            string VersionHash = GetVersionHash(f, version);

            Console.WriteLine("Add this to AkemiSwitcher.Mirai.json:");
            Console.WriteLine("{" + string.Format("\"versionCode\": \"{0}\",\"hash\": \"{1}\",\"versionData\": \"{2}\"", version, Hash, VersionHash) + "}");
            Console.ReadLine();
        }
    }
}
