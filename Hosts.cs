using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkemiSwitcher
{
    class Hosts
    {
        internal string HostsPath = Environment.GetEnvironmentVariable("windir") + "\\System32\\Drivers\\etc\\hosts";
        internal List<HostEntry> hostEntries = new List<HostEntry>();

        public async Task Parse()
        {
            hostEntries.Clear();
            // try to do the magic!
            try
            {
                if (!File.Exists(HostsPath)) // no file? just go away.
                    return;

                using (StreamReader reader = new StreamReader(HostsPath)) // open hosts file
                {
                    string line;
                    while ((line = reader.ReadLine()) != null) // read the hosts file line by line
                    {
                        if (line.StartsWith("#") || line.Length == 0)
                        {
                            hostEntries.Add(new HostCommentEntry()
                            {
                                comment = line
                            });
                            continue;
                        }

                        // lazy safe way.
                        string l = line.Replace("\t", " ");
                        string ip = l.Substring(0, l.IndexOf(" "));
                        string host = l.Substring(l.IndexOf(" "));
                        while (host[0] == ' ') host = host.Substring(1);
                        if(host.IndexOf(" ") > 0) host = host.Substring(host.IndexOf(" "));

                        hostEntries.Add(new HostEntry()
                        {
                            ipAddress = ip,
                            targetDomain = host
                        });
                    }
                }
            }
            finally { }
        }

        public async Task Save()
        {
            if (hostEntries.Count == 0) throw new Exception("hostEntries.Count = 0");
            foreach (HostEntry h in hostEntries) Console.WriteLine(h);
        }
    }
}
