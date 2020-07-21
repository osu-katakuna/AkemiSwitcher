using System;
using System.Collections.Generic;
using System.IO;

namespace KaedeCore.Objects
{
    public class Locale
    {
        public int Id;
        public string Code;
        public string LocalisedName;
        private List<LocaleString> Strings = new List<LocaleString>();

        public static Locale FromBinaryReader(BinaryReader b)
        {
            Locale l = new Locale();

            l.Id = b.ReadInt32();
            l.Code = b.ReadString();
            l.LocalisedName = b.ReadString();

            int strCnt = b.ReadInt32();
            if(strCnt < 0)
            {
                throw new Exception("Invalid string count!");
            }

            for(int i = 0; i < strCnt; i++)
            {
                l.Strings.Add(LocaleString.FromBinaryReader(b));
            }

            return l;
        }

        public string GetString(string Name)
        {
            foreach (LocaleString s in Strings)
            {
                if (string.Compare(s.Name, Name) == 0) return s.Value;
            }
            return null;
        }
    }
}
