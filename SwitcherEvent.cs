using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkemiSwitcher
{
    public class SwitcherMessageEvent
    {
        public SwitcherEvent eventType;
        public SwitcherServer server;
        public string message;
        public bool justText = false;
    }

    public enum SwitcherEvent
    {
        NoAdminRights,
        ServerError,
        ServerSwitch,
        SwitcherError,
        PleaseWait,
        ServerConnecting
    };

    public enum SwitcherServer
    {
        Bancho,
        Private,
        Other
    }
}
