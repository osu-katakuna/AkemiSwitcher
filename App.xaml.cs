using AkemiSwitcher.Properties;
using KaedeCore.Objects;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
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
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
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

        public string GetTranslationString(string x)
        {
            return Translation.GetString(x);
        }

        public void PerformSwitcherAction()
        {
            _ = switcher.PerformSwitch();
        }

        public void UpdateMode()
        {
            uiRef?.Hide();
            AkemiSwitcherUpdate u = new AkemiSwitcherUpdate();
            u.Show();
        }

        public void onSwitcherMessage(object sender, SwitcherMessageEvent e)
        {
            uiRef.fallbackLabel.Visibility = e.serverFailure ? Visibility.Visible : Visibility.Hidden;
            uiRef.fallbackLabel.Content = Translation.GetString("info_fallback");

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
                case SwitcherEvent.ServerSwitchInProgress:
                    if (!e.justText)
                    {
                        uiRef.btnSwitch.Background = this.FindResource("ButtonStateDisabled") as Brush;
                        uiRef.btnSwitch.IsEnabled = false;
                    }
                    uiRef.btnSwitch.Content = string.Format(Translation.GetString("info_switching"), switcher.GetSwitchToText());
                    appendVersion = string.Format(Translation.GetString("status_switching"), switcher.GetSwitchToText()).ToLower();
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
                        uiRef.btnSwitch.Background = switcher.onCurrentServer == SwitcherServer.Private ? this.FindResource("ButtonStateOK") as Brush : this.FindResource("ButtonStateNormal") as Brush;
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

        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs e)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();

            var assemblyName = new AssemblyName(e.Name);
            var dllName = assemblyName.Name + ".dll";

            var resources = thisAssembly.GetManifestResourceNames().Where(s => s.EndsWith(dllName));
            if (resources.Any())
            {
                var resourceName = resources.First();
                using (var stream = thisAssembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null) return null;
                    var block = new byte[stream.Length];

                    try
                    {
                        stream.Read(block, 0, block.Length);
                        return Assembly.Load(block);
                    }
                    catch (IOException)
                    {
                        return null;
                    }
                    catch (BadImageFormatException)
                    {
                        return null;
                    }
                }
            }

            // in the case the resource doesn't exist, return null.
            return null;
        }
    }
}
