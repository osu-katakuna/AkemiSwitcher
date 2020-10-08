using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AkemiSwitcher
{
    class Switcher
    {
        Hosts hosts = new Hosts();
        public event EventHandler<SwitcherMessageEvent> OnSwitcherMessage;
        internal List<HostEntry> serverEntries = new List<HostEntry>();
        internal bool serverSuccess = false;
        public SwitcherServer onCurrentServer = SwitcherServer.Bancho;

        internal string[] targetFallover = {
            "c.ppy.sh",
            "c4.ppy.sh",
            "c5.ppy.sh",
            "c6.ppy.sh",
            "a.ppy.sh",
            "s.ppy.sh"
        };

        public string GetSwitchToText()
        {
            if (onCurrentServer == SwitcherServer.Private) return "Bancho";
            return BuildInfo.ServerName;
        }

        internal async Task PerformServerConnection()
        {
            var webClient = new WebClient();
            string serverOutput = webClient.DownloadString(BuildInfo.SwitcherServerList);

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

            JToken servers = data.SelectToken("servers");
            if(servers == null)
            {
                goto SERVER_FAILURE;
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

        SERVER_FAILURE:
            OnSwitcherMessage?.Invoke(null, new SwitcherMessageEvent()
            {
                eventType = SwitcherEvent.ServerError
            });
            return;
        }

        public async void Prepare()
        {
            OnSwitcherMessage?.Invoke(null, new SwitcherMessageEvent()
            {
                eventType = SwitcherEvent.PleaseWait
            });

            if(!Utils.IsAdministrator())
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

            await PerformServerConnection();

            if(!serverSuccess)
            {
                foreach(string host in targetFallover)
                {
                    serverEntries.Add(new HostEntry()
                    {
                        ipAddress = BuildInfo.StaticServerIP,
                        targetDomain = host
                    });
                }
            }

            OnSwitcherMessage?.Invoke(null, new SwitcherMessageEvent()
            {
                eventType = SwitcherEvent.PleaseWait
            });

            await hosts.Parse();

            // check if we are switched to a server. search for known DNS-es
            List<HostEntry> knownHosts = hosts.hostEntries.FindAll(host => serverEntries.Exists(s => s.targetDomain == host.targetDomain));
            List<HostEntry> ourServer = knownHosts.FindAll(known => serverEntries.Exists(s => s.ipAddress == known.ipAddress));

            if(knownHosts.Count == 0) onCurrentServer = SwitcherServer.Bancho;
            else if(ourServer.Count > 0) onCurrentServer = SwitcherServer.Private;
            else onCurrentServer = SwitcherServer.Other;

            OnSwitcherMessage?.Invoke(null, new SwitcherMessageEvent()
            {
                eventType = SwitcherEvent.ServerSwitch,
                server = onCurrentServer
            });
        }
    }
}

