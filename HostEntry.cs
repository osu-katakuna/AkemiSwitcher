namespace AkemiSwitcher
{
    internal class HostEntry
    {
        public string ipAddress;
        public string targetDomain;

        public override string ToString() => $"{targetDomain} => {ipAddress}";

        public HostEntry() {}

        public override bool Equals(object obj)
        {
            // classic stuff
            var other = obj as HostEntry; // try to cast to this class
            if (other == null) return false; // if I can't give up.
            return ipAddress.Equals(other.ipAddress) && targetDomain.Equals(other.targetDomain); // else check if the IPs and Domains match.
        }
    }

    internal class HostCommentEntry: HostEntry
    {
        public string comment;

        public override string ToString() => $"COMMENT: {comment}";

        public HostCommentEntry() { }
    }
}
