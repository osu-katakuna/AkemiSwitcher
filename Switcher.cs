using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace AkemiSwitcher
{
    #region Errors
#if !ONLINE_SERVERS && !FALLBACK
#error AkemiSwitcher can't compile: You need to define a proper way to get the server IP: either you use the FALLBACK method, or you use ONLINE_SERVERS(from JSON, you can back it up with FALLBACK).
#endif
    #endregion

    class Switcher
    {
        Hosts hosts = new Hosts();
        public event EventHandler<SwitcherMessageEvent> OnSwitcherMessage;
        internal List<HostEntry> serverEntries = new List<HostEntry>();
#if ONLINE_SERVERS
        internal bool serverSuccess = false;
#endif
        public SwitcherServer onCurrentServer = SwitcherServer.Bancho;

#if FALLBACK
        internal string[] targetFallover = {
            "c.ppy.sh",
            "c4.ppy.sh",
            "c5.ppy.sh",
            "c6.ppy.sh",
            "ce.ppy.sh",
            "i.ppy.sh",
            "delta.ppy.sh",
            "a.ppy.sh",
            "s.ppy.sh"
        };
#endif

        public string GetSwitchToText()
        {
            if (onCurrentServer == SwitcherServer.Private) return "Bancho";
            return BuildInfo.ServerName;
        }

        internal async Task PerformSwitch()
        {
            OnSwitcherMessage?.Invoke(null, new SwitcherMessageEvent()
            {
                eventType = SwitcherEvent.ServerSwitchInProgress,
                server = onCurrentServer,
#if ONLINE_SERVERS
                serverFailure = !serverSuccess
#else
#if FALLBACK
                serverFailure = true
#else
                serverFailure = false
#endif
#endif
            });

            if(onCurrentServer == SwitcherServer.Private)
            {
                foreach (HostEntry a in serverEntries)
                    hosts.hostEntries.RemoveAll(x => x.targetDomain == a.targetDomain);
            } else {
                foreach (HostEntry a in serverEntries)
                {
                    if (hosts.hostEntries.Exists(x => x.targetDomain == a.targetDomain))
                        hosts.hostEntries.Find(x => x.targetDomain == a.targetDomain).ipAddress = a.ipAddress;
                    else
                        hosts.hostEntries.Add(a);
                }
            }

            hosts.Save();
            HostsCheck();
        }

#if ONLINE_SERVERS
        internal async Task PerformServerConnection()
        {
            var webClient = new WebClient();
            string serverOutput = webClient.DownloadString(BuildInfo.SwitcherServerList);

            JToken token = JObject.Parse(serverOutput);

            string target = (string) token.SelectToken("target");
            if(target == null || !target.Equals("AkemiSwitcher"))
            {
                OnSwitcherMessage?.Invoke(null, new SwitcherMessageEvent()
                {
                    eventType = SwitcherEvent.ServerError
                });
                return;
            }

            JToken data = token.SelectToken("data");
            if (data == null)
            {
                OnSwitcherMessage?.Invoke(null, new SwitcherMessageEvent()
                {
                    eventType = SwitcherEvent.ServerError
                });
                return;
            }

            JToken servers = data.SelectToken("servers");
            if(servers == null)
            {
                OnSwitcherMessage?.Invoke(null, new SwitcherMessageEvent()
                {
                    eventType = SwitcherEvent.ServerError
                });
                return;
            }

            foreach(JToken s in servers)
            {
                string host = (string) s.SelectToken("host");
                string targetIP = (string) s.SelectToken("newTarget");

                serverEntries.Add(new HostEntry()
                {
                    ipAddress = targetIP,
                    targetDomain = host
                });
            }

            serverSuccess = true;
        }
#endif

        internal async Task HostsCheck()
        {
            OnSwitcherMessage?.Invoke(null, new SwitcherMessageEvent()
            {
                eventType = SwitcherEvent.PleaseWait,
#if ONLINE_SERVERS
                serverFailure = !serverSuccess
#else
#if FALLBACK
                serverFailure = true
#else
                serverFailure = false
#endif
#endif
            });

            await hosts.Parse();

            // check if we are switched to a server. search for known DNS-es
            List<HostEntry> knownHosts = hosts.hostEntries.FindAll(host => serverEntries.Exists(s => s.targetDomain == host.targetDomain));
            List<HostEntry> ourServer = knownHosts.FindAll(known => serverEntries.Exists(s => s.ipAddress == known.ipAddress));

            if (knownHosts.Count == 0) onCurrentServer = SwitcherServer.Bancho;
            else if (ourServer.Count > 0) onCurrentServer = SwitcherServer.Private;
            else onCurrentServer = SwitcherServer.Other;

            OnSwitcherMessage?.Invoke(null, new SwitcherMessageEvent()
            {
                eventType = SwitcherEvent.ServerSwitch,
                server = onCurrentServer,
#if ONLINE_SERVERS
                serverFailure = !serverSuccess
#else
#if FALLBACK
                serverFailure = true
#else
                serverFailure = false
#endif
#endif
            });
        }

        public async void Prepare()
        {
            OnSwitcherMessage?.Invoke(null, new SwitcherMessageEvent()
            {
                eventType = SwitcherEvent.PleaseWait
            });

#if UPDATABLE || TAMPER_CHECK
            if (File.Exists(Path.Combine(System.Windows.Forms.Application.StartupPath, "AkemiSwitcher.Update.tmp"))) File.Delete(Path.Combine(System.Windows.Forms.Application.StartupPath, "AkemiSwitcher.Update.tmp"));

            var webClient = new WebClient();
            string serverOutput = webClient.DownloadString(BuildInfo.UpdateVersionList);

            JToken token = JObject.Parse(serverOutput);

            string target = (string) token.SelectToken("target");
            if(target == null || !target.Equals("AkemiSwitcher"))
            {
                goto SERVER_FAILURE;
            }

            JToken data = token.SelectToken("data");
            if (data == null)
            {
                goto SERVER_FAILURE;
            }

            JToken versions = data.SelectToken("versions");
            if(versions == null)
            {
                goto SERVER_FAILURE;
            }

            string latestVersionString = (string) versions.ToList().OrderByDescending(x => int.Parse(((string)x.SelectToken("versionCode")).Replace(".", ""))).First().SelectToken("versionCode");
            int latestVersion = int.Parse(latestVersionString.Replace(".", ""));
            int currentVersion = int.Parse(System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString().Replace(".", ""));

#if TAMPER_CHECK
            foreach (JToken s in versions)
            {
                string version = (string) s.SelectToken("versionCode");
                if(version.Equals(System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString()))
                {
                    string hash = (string)s.SelectToken("hash");
                    string versionData = (string)s.SelectToken("versionData");

                    if(!hash.Equals(Utils.GetSwitcherHash()) || !versionData.Equals(Utils.GetVersionHash()))
                    {
                        MessageBox.Show(((App)App.Current).GetTranslationString("warn_modified"), ((App)App.Current).GetTranslationString("title_warn_modified"), MessageBoxButton.OK, MessageBoxImage.Error);
                        ((App)App.Current).UpdateMode();
                        return;
                    }
                }
            }
#endif

            if (latestVersion > currentVersion)
            {
                MessageBoxResult x = MessageBox.Show(string.Format(((App)App.Current).GetTranslationString("warn_newVersion"), System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(), latestVersionString), ((App)App.Current).GetTranslationString("title_warn_newVersion"), MessageBoxButton.YesNo, MessageBoxImage.Information);
                if(x == MessageBoxResult.Yes)
                {
                    ((App)App.Current).UpdateMode();
                }
            }
#endif

            if (!Utils.IsAdministrator())
            {
                OnSwitcherMessage?.Invoke(null, new SwitcherMessageEvent()
                {
                    eventType = SwitcherEvent.NoAdminRights
                });
                return;
            }

            OnSwitcherMessage?.Invoke(null, new SwitcherMessageEvent()
            {
                eventType = SwitcherEvent.ServerConnecting
            });

#if ONLINE_SERVERS
            try
            {
                await PerformServerConnection();
            } catch(WebException e)
            {
                serverSuccess = false;
            }

            if(!serverSuccess)
            {
#if FALLBACK
                foreach(string host in targetFallover)
                {
                    serverEntries.Add(new HostEntry()
                    {
                        ipAddress = BuildInfo.StaticServerIP,
                        targetDomain = host
                    });
                }
#else
                OnSwitcherMessage?.Invoke(null, new SwitcherMessageEvent()
                {
                    eventType = SwitcherEvent.ServerError
                });
                return;
#endif
#endif
            }

            await HostsCheck();
#if UPDATABLE
        SERVER_FAILURE:
            OnSwitcherMessage?.Invoke(null, new SwitcherMessageEvent()
            {
                eventType = SwitcherEvent.ServerError
            });
            return;
#endif
        }
    }
}

