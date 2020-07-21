using KaedeCore.Objects;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace KaedeEngine
{
    class KaedeEngine
    {
        public Locale Locale;
        public static int Version = 1;
        private Locale FallbackLocale;
        public List<LocalePreview> AllLocales = new List<LocalePreview>();

        // optimised function to not take up memory; loads only required locales
        public static KaedeEngine LoadLocale(string TargetLocale)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AkemiSwitcher.Translation.AkemiSwitcher.ktf");
            KaedeEngine instance = new KaedeEngine();

            BinaryReader b = new BinaryReader(stream);

            if (string.Compare(new string(b.ReadChars(6)), "KAEDE!") != 0) // check for the header
            {
                b.Close();
                throw new System.Exception("Invalid Kaede Translation file!");
            }

            int ver = b.ReadInt32(); // get teh version
            int fbLocaleID = b.ReadInt32(); // get the fall back locale id

            if (ver != Version) // version checks pls
            {
                b.Close();
                throw new System.Exception(string.Format("Invalid translation file; It was designed for v{0}.", ver));
            }

            int cntLocales = b.ReadInt32(); // get how many locales are there

            if (cntLocales < 0) // if negative(wtf?)
            {
                b.Close();
                throw new System.Exception("Invalid structure!");
            }

            for (int i = 0; i < cntLocales; i++)
            {
                Locale l = Locale.FromBinaryReader(b);

                if (l.Id == fbLocaleID && instance.FallbackLocale == null)
                {
                    instance.FallbackLocale = l;
                }

                if(string.Compare(l.Code, TargetLocale) == 0 && l.Id != fbLocaleID && instance.Locale == null) 
                    instance.Locale = l;

                instance.AllLocales.Add(new LocalePreview()
                {
                    LocalisedName = l.LocalisedName,
                    Id = l.Id,
                    Code = l.Code
                });
            }

            b.Close();

            if (instance.Locale == null) throw new System.Exception(string.Format("Locale {0} not found!", TargetLocale));

            return instance;
        }

        public static KaedeEngine FromCurrentUICulture()
        {
            return LoadLocale(Thread.CurrentThread.CurrentUICulture == null ? "en" : Thread.CurrentThread.CurrentUICulture.Name);
        }

        public string GetString(string Name)
        {
            if(Locale == null)
            {
                if (FallbackLocale == null) return Name;
                return FallbackLocale.GetString(Name) == null ? Name : FallbackLocale.GetString(Name);
            }

            return Locale.GetString(Name) == null ? Name : Locale.GetString(Name);
        }
    }
}
