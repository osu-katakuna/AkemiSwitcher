using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AkemiSwitcher
{
    public partial class AkemiSwitcherUpdate : Form
    {
        string UpdateText = "";
        int Percentage = 0;
        string updateFilePath = Path.Combine(Application.StartupPath, "AkemiSwitcher.Update.tmp");

        public int ProgressValue
        {
            set
            {
                Percentage = value;
                Brush progressBrush = new SolidBrush(Color.FromArgb(92, 214, 51));

                Bitmap bitmap = new Bitmap(progressBar.Width, progressBar.Height);

                Graphics g = Graphics.FromImage(bitmap);
                g.Clear(Color.FromArgb(51, 51, 51));

                g.FillRectangle(progressBrush, new Rectangle(0, 0, (int)(value * (progressBar.Width / 100.0)), progressBar.Height));

                progressBar.Image = bitmap;

                if (UpdateText.Length > 0) statusText.Text = string.Format(UpdateText, value);
            }
        }
        

        public AkemiSwitcherUpdate()
        {
            InitializeComponent();
        }

        private void AkemiSwitcherUpdate_Load(object sender, EventArgs e)
        {
            if (File.Exists(updateFilePath)) File.Delete(updateFilePath);
            ProgressValue = 0;
            UpdateText = ((App)App.Current).GetTranslationString("info_updating");
            try
            {
                _ = UpdateProcess();
            } catch (WebException exc)
            {
                Console.WriteLine(exc);
                ServerError();
                return;
            }
        }

        void ServerError()
        {
            MessageBox.Show(((App)App.Current).GetTranslationString("message_updateFailed"), ((App)App.Current).GetTranslationString("title_updateFailed"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.Close();
        }

        private void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressValue = e.ProgressPercentage;

            if(Percentage == 100)
            {
                // todo: FIND A DAMN WAY TO UPDATE STUFF
                if (File.Exists(updateFilePath))
                {
                    Process process = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = string.Format("/C ping 127.0.0.1 -n 7 && move /y \"{0}\" \"{1}\" && \"{1}\"", updateFilePath, Process.GetCurrentProcess().MainModule.FileName);
                    process.StartInfo = startInfo;
                    process.Start();
                }

                this.Close();
            }
        }

        private async Task UpdateProcess()
        {
            JToken latestVersion;

            using (WebClient webClient = new WebClient())
            {

                statusText.Text = ((App)App.Current).GetTranslationString("info_wait");

                // get the latest version.
                string serverOutput = webClient.DownloadString(BuildInfo.UpdateVersionList);

                JToken token = JObject.Parse(serverOutput);

                string target = (string)token.SelectToken("target");
                if (target == null || !target.Equals("AkemiSwitcher"))
                {
                    ServerError();
                    return;
                }

                JToken data = token.SelectToken("data");
                if (data == null)
                {
                    ServerError();
                    return;
                }

                JToken versions = data.SelectToken("versions");
                if (versions == null)
                {
                    ServerError();
                    return;
                }

                latestVersion = versions.ToList().OrderByDescending(x => int.Parse(((string)x.SelectToken("versionCode")).Replace(".", ""))).First();
                webClient.Dispose();
            }

            if (latestVersion == null)
            {
                ServerError();
                return;
            }

            // get the latest version.
            using (WebClient wc = new WebClient())
            {
                //wc.DownloadFileCompleted += SwitcherUpdateComplete;
                wc.DownloadProgressChanged += DownloadProgressCallback;

                await wc.DownloadFileTaskAsync(new Uri((string)latestVersion.SelectToken("downloadURL")), updateFilePath);
            }
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            if (File.Exists(updateFilePath) && Percentage != 100) File.Delete(updateFilePath);
            ((App)App.Current).Shutdown();
            System.Environment.Exit(0);
        }
    }
}
