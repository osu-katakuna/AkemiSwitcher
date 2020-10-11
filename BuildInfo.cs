using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace AkemiSwitcher
{
    class BuildInfo
    {
        public static string ServerName          = "Katakuna";
#if FALLBACK
        public static string StaticServerIP      = "51.83.241.127";
#endif
#if ONLINE_SERVERS
        public static string SwitcherServerList  = "https://raw.githubusercontent.com/osu-katakuna/common/master/AkemiSwitcher.Servers.json?version=latest";
#endif
#if UPDATABLE
        public static string UpdateVersionList   = "https://raw.githubusercontent.com/osu-katakuna/common/master/AkemiSwitcher.Mirai.json?version=latest";
#endif
    }
}
