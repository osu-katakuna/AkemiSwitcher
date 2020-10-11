using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Security.Principal;

namespace AkemiSwitcher
{
    class Utils
    {
        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static string GetSwitcherHash()
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            FileStream stream = File.OpenRead(Process.GetCurrentProcess().MainModule.FileName);

            md5.ComputeHash(stream);

            stream.Close();

            string rtrn = "";
            for (int i = 0; i < md5.Hash.Length; i++) rtrn += (md5.Hash[i].ToString("x2"));

            return rtrn.ToUpper();
        }

        public static string GetVersionHash()
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(string.Format(@"AkemiSwitcher{0}{1}damedanedameyo{0}bakamitai{1}versaosaopaolo{2}haxormoment{3}", 2030 / 2 + 5 - 20 * 2, 2020, System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(), GetSwitcherHash())));

            string rtrn = "";
            for (int i = 0; i < md5.Hash.Length; i++) rtrn += (md5.Hash[i].ToString("x2"));

            return rtrn.ToUpper();
        }
    }
}
