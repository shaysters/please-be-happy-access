using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PleaseBeHappyAccess
{
    /// <summary>
    /// Image descriptions, loaded from descriptions\&lt;locale&gt;.tsv next to the mod DLL
    /// (one "key&lt;TAB&gt;text" per line, # for comments). English (en.tsv) is the fallback.
    ///
    /// Keys:
    ///   bg/&lt;actor id&gt;/&lt;appearance&gt;   a background or CG, e.g. bg/MainBackground/library_day, bg/cg_fg/AspenConfession1a
    ///   bgid/&lt;actor id&gt;               fallback for any appearance of a layered image, e.g. bgid/happy
    ///   char/&lt;character id&gt;          full description of a character, said the first time you meet them
    ///   part/&lt;appearance part&gt;       words for one expression/pose part, e.g. part/smile = smiling
    ///   part/&lt;character id&gt;/&lt;part&gt;   the same, for one character only (checked first)
    ///   skip/&lt;appearance&gt;            appearances that should never be announced (plain black, white, letterbox)
    /// </summary>
    internal static class Descriptions
    {
        static readonly Dictionary<string, Dictionary<string, string>> byLocale = new Dictionary<string, Dictionary<string, string>>();
        static string folder;

        public static void Load(string pluginDir)
        {
            folder = Path.Combine(pluginDir, "descriptions");
            byLocale.Clear();
        }

        public static string Get(string key)
        {
            string s = Table(Strings.Locale).TryGetValue(key, out string v) ? v : null;
            if (s == null && Strings.Locale != "en") Table("en").TryGetValue(key, out s);
            return s;
        }

        public static bool Has(string key) => Get(key) != null;

        static Dictionary<string, string> Table(string locale)
        {
            if (byLocale.TryGetValue(locale, out var t)) return t;
            t = new Dictionary<string, string>();
            string path = folder == null ? null : Path.Combine(folder, locale + ".tsv");
            if (path != null && File.Exists(path))
            {
                foreach (string raw in File.ReadAllLines(path, Encoding.UTF8))
                {
                    if (raw.Length == 0 || raw[0] == '#') continue;
                    int tab = raw.IndexOf('\t');
                    if (tab > 0) t[raw.Substring(0, tab).Trim()] = raw.Substring(tab + 1).Trim();
                }
                Log.Info($"Loaded {t.Count} descriptions for {locale}");
            }
            byLocale[locale] = t;
            return t;
        }
    }
}
