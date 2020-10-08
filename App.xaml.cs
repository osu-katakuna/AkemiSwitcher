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

        SwitcherMessageEvent lastEv;

        void App_Startup(object sender, StartupEventArgs e)
        {
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

            window.btnSwitch.Background = this.FindResource("ButtonStateDisabled") as Brush;
            window.btnSwitch.IsEnabled = false;
            window.btnSwitch.Content = Translation.GetString("info_wait");
            window.versionText.Content = string.Format("{0} v{1}",
                System.Reflection.Assembly.GetEntryAssembly().GetName().Name,
                System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString()
            );

            window.Show();

            switcher.OnSwitcherMessage += onSwitcherMessage;
            switcher.Prepare();
        }

        public void onSwitcherMessage(object sender, SwitcherMessageEvent e)
        {
            lastEv = e;
            string appendVersion = "";

            switch(e.eventType)
            {
                case SwitcherEvent.ServerConnecting:
                    if (!e.justText)
                    {
                        uiRef.btnSwitch.Background = this.FindResource("ButtonStateDisabled") as Brush;
                        uiRef.btnSwitch.IsEnabled = false;
                    }
                    uiRef.btnSwitch.Content = Translation.GetString("info_server");
                    appendVersion = Translation.GetString("info_server_short").ToLower();
                    break;
                case SwitcherEvent.PleaseWait:
                    if(!e.justText)
                    {
                        uiRef.btnSwitch.Background = this.FindResource("ButtonStateDisabled") as Brush;
                        uiRef.btnSwitch.IsEnabled = false;
                    }
                    uiRef.btnSwitch.Content = Translation.GetString("info_wait");
                    appendVersion = Translation.GetString("info_wait").ToLower();
                    break;
                case SwitcherEvent.ServerError:
                    if (!e.justText)
                    {
                        uiRef.btnSwitch.IsEnabled = false;
                        uiRef.btnSwitch.Background = this.FindResource("ButtonStateError") as Brush;
                    }
                    uiRef.btnSwitch.Content = Translation.GetString("error_server");
                    break;
                case SwitcherEvent.NoAdminRights:
                    if (!e.justText)
                    {
                        uiRef.btnSwitch.Background = this.FindResource("ButtonStateDisabled") as Brush;
                        uiRef.btnSwitch.IsEnabled = false;
                    }
                    uiRef.btnSwitch.Content = Translation.GetString("error_NoAdmin");
                    break;
                case SwitcherEvent.ServerSwitch:
                    if (!e.justText)
                    {
                        uiRef.btnSwitch.Background = this.FindResource("ButtonStateNormal") as Brush;
                        uiRef.btnSwitch.IsEnabled = true;
                    }
                    string playingOn = "";

                    if (switcher.onCurrentServer == SwitcherServer.Bancho) playingOn = "Bancho";
                    else if (switcher.onCurrentServer == SwitcherServer.Other) playingOn = Translation.GetString("server_other");
                    else if (switcher.onCurrentServer == SwitcherServer.Private) playingOn = BuildInfo.ServerName;

                    uiRef.btnSwitch.Content = string.Format(Translation.GetString("prompt_SwitchTo"), switcher.GetSwitchToText());
                    appendVersion = string.Format(Translation.GetString("status_playing"), playingOn);
                    break;
            }

            uiRef.versionText.Content = string.Format(appendVersion.Length > 0 ? "{0} v{1} - {2}" : "{0} v{1}",
                System.Reflection.Assembly.GetEntryAssembly().GetName().Name,
                System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(),
                appendVersion
            );
        }

        public void UpdateLanguageByIndex(int Index)
        {
            if(Translation != null && Translation.AllLocales[Index] != null)
            {
                Settings.Default.PreferredLocale = Translation.AllLocales[Index].Code;
                Settings.Default.Save();

                Translation = KaedeEngine.KaedeEngine.LoadLocale(Translation.AllLocales[Index].Code);

                if (lastEv != null)
                {
                    SwitcherMessageEvent t = lastEv;
                    t.justText = true;
                    onSwitcherMessage(null, t);
                }
            }
        }
    }
}
