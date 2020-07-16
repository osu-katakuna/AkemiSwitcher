using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace AkemiSwitcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Switcher switcher = new Switcher();

        void App_Startup(object sender, StartupEventArgs e)
        {
            AkemiSwitcherUI window = new AkemiSwitcherUI();
            window.Show();

            switcher.LoadHostsFiles();
        }

        public void JaPierdole()
        {
            AkemiSwitcherUI ui = ((AkemiSwitcherUI)this.MainWindow);

            ui.btnSwitch.Background = this.FindResource("ButtonStateError") as Brush;
            ui.btnSwitch.IsEnabled = true;
            ui.btnSwitch.Content = "KURWA JA PIERDOLE APP.CS";

            ui.versionText.Content = "KURWA JA PIERDOLE";
        }


        public bool isSwitching = false;
        public bool isKatakuna = true;
        public bool isBancho = false;

        public string server
        {
            get
            {
                return !isSwitching ? getCurrentServer() : getTargetServer();
            }
        }

        public string SwitchTo
        {
            get
            {
                return string.Format("Switch to {0}", getTargetServer());
            }
        }

        public string getCurrentServer()
        {
            if (isKatakuna && !isBancho) return "Katakuna";
            if (!isKatakuna && isBancho) return "Bancho";

            return "Other server";
        }

        public string getTargetServer()
        {
            if (isKatakuna) return "Bancho";
            if (isBancho) return "Katakuna";

            return "Katakuna";
        }

        public Brush targetServerBrush()
        {
            if (isKatakuna) return this.FindResource("ButtonStateNormal") as Brush;
            if (isBancho) return this.FindResource("ButtonStateOK") as Brush;

            return this.FindResource("ButtonStateNormal") as Brush;
        }

        public string status
        {
            get
            {
                return isSwitching ? string.Format("switching to {0}...", server) : string.Format("playing on {0}.", server);
            }
        }

        public string versionString
        {
            get
            {
                return string.Format("{0} v{1} - {2}", System.Reflection.Assembly.GetEntryAssembly().GetName().Name, System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(), status);
            }
        }

    }
}
