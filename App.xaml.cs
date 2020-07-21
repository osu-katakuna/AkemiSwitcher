using AkemiSwitcher.Properties;
using KaedeCore.Objects;
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

            Translation = KaedeEngine.KaedeEngine.LoadLocale(Settings.Default.PreferredLocale);

            AkemiSwitcherUI window = new AkemiSwitcherUI();
            uiRef = window;

            int sIndex = 0;

            foreach (LocalePreview l in Translation.AllLocales)
            {
                window.languageSelectionBox.Items.Add(l.LocalisedName);
                if(l.Id == Translation.Locale.Id)
                {
                    window.languageSelectionBox.SelectedIndex = sIndex;
                }
                sIndex++;
            }

            if (Utils.IsAdministrator())
            {
                switcher.LoadHostsFiles();
            } else
            {
                window.btnSwitch.IsEnabled = false;
                window.btnSwitch.Content = Translation.GetString("error_NoAdmin");
            }

            window.Show();
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

        public void UpdateLanguageByIndex(int Index)
        {
            if(Translation != null && Translation.AllLocales[Index] != null)
            {
                Settings.Default.PreferredLocale = Translation.AllLocales[Index].Code;
                Settings.Default.Save();

                Translation = KaedeEngine.KaedeEngine.LoadLocale(Translation.AllLocales[Index].Code);

                uiRef.switchOnLoad();
            }
        }
    }
}
