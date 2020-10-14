using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AkemiSwitcher
{
    class CertificateManager
    {
        public X509Certificate2 ServerCertificate;
        X509Store store;

        private bool hasServerCertificate
        {
            get
            {
                return GetServerCert().Count > 0;
            }
        }

        private X509Certificate2Collection GetServerCert()
        {
            return store.Certificates.Find(X509FindType.FindBySerialNumber, ServerCertificate.GetSerialNumberString(), true);
        }

        public bool checkForCertificate()
        {
            X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            try
            {
                store.Open(OpenFlags.ReadWrite);
                return store.Certificates.Find(X509FindType.FindBySerialNumber, ServerCertificate.GetSerialNumberString(), true).Count > 0;
            }
            finally
            {
                store.Close();
            }
        }

        private void InstallCertificateStore()
        {
            store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            try
            {
                store.Open(OpenFlags.ReadWrite);
                if (!hasServerCertificate)
                    return;
                store.Add(ServerCertificate);
            }
            finally
            {
                store.Close();
            }
        }

        private void InstallCertificateCertutil(bool user = true)
        {
            var p = Path.GetTempPath() + Guid.NewGuid().ToString() + ".crt";
            try
            {
                File.WriteAllBytes(p, ServerCertificate.RawData);
            }
            catch (IOException)
            {
                throw new Exception("Couldn't install the certificate through the certutil method: couldn't create the temporary certificate in order to install it; maybe you don't have rights to do so?");
            }

            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();

                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.CreateNoWindow = true;
                startInfo.FileName = "certutil";
                startInfo.Arguments = $"{(user ? "-user " : "")}-f -AddStore \"Root\" {p}";

                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();
                string err = process.StandardError.ReadToEnd();

                if (process.ExitCode != 0 || !checkForCertificate())
                {
                    throw new Exception($"Couldn't install the certificate through the certutil method\nExited with code {process.ExitCode}, output:\n\n{output}");
                }
            }
            finally
            {
                System.IO.File.Delete(p);
            }
        }

        public void installCertificate()
        {
            string errors = "";

            try {
                InstallCertificateStore();
            } catch(Exception e)
            {
                errors += e.Message + "\n";
            }

            if (checkForCertificate())
            {
                errors = "";
                return;
            }

            try
            {
                InstallCertificateCertutil(true);
            }
            catch (Exception e)
            {
                errors += e.Message + "\n";
            }

            if (checkForCertificate())
            {
                errors = "";
                return;
            }

            try
            {
                InstallCertificateCertutil(false);
            }
            catch (Exception e)
            {
                errors += e.Message + "\n";
            }

            if (checkForCertificate())
            {
                errors = "";
                return;
            }

            if (errors.Length > 0)
            {
                ((App)App.Current).CertSetupError(errors);
            }
        }
    }
}
