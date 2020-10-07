using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AkemiSwitcher
{
    class Switcher
    {
        Hosts hosts = new Hosts();

        public void LoadHostsFiles()
        {
            _ = hosts.Parse();
        }
    }
}

