using System.Globalization;
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
        AkemiSwitcherUI uiRef;
        KaedeEngine.KaedeEngine Translation;

        void App_Startup(object sender, StartupEventArgs e)
        {
            // System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh");

            Translation = KaedeEngine.KaedeEngine.LoadLocale("pl");

            AkemiSwitcherUI window = new AkemiSwitcherUI();
            window.Show();

            uiRef = ((AkemiSwitcherUI)this.MainWindow);

            if (Utils.IsAdministrator())
            {
                switcher.LoadHostsFiles();
            } else
            {
                uiRef.btnSwitch.IsEnabled = false;
                uiRef.btnSwitch.Content = Translation.GetString("error_NoAdmin");
            }
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
                return string.Format(Translation.GetString("prompt_SwitchTo"), getTargetServer());
            }
        }

        public string getCurrentServer()
        {
            if (isKatakuna && !isBancho) return "Katakuna";
            if (!isKatakuna && isBancho) return "Bancho";

            return Translation.GetString("server_other");
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
                return isSwitching ? string.Format(Translation.GetString("status_switching"), server) : string.Format(Translation.GetString("status_playing"), server);
            }
        }

        public string versionString
        {
            get
            {
                return Utils.IsAdministrator() ?
                    string.Format("{0} v{1} - {2}",
                        System.Reflection.Assembly.GetEntryAssembly().GetName().Name,
                        System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(),
                        status
                    ) :
                    string.Format("{0} v{1}",
                        System.Reflection.Assembly.GetEntryAssembly().GetName().Name,
                        System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString()
                    );
            }
        }

    }
}
