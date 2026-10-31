using System.Linq;
using System.Text.RegularExpressions;

namespace PleaseBeHappyAccess
{
    internal static class TextUtil
    {
        static readonly Regex Tags = new Regex("<[^>]*>", RegexOptions.Compiled);
        static readonly Regex Spaces = new Regex(@"\s+", RegexOptions.Compiled);

        /// <summary>Strips rich-text tags and collapses whitespace so screen readers get plain text.</summary>
        public static string Clean(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return Spaces.Replace(Tags.Replace(text, string.Empty), " ").Trim();
        }

        // <br> tags, the two characters \ and n (some credits contain them literally), real newlines, and
        // runs of tabs (the music room lays out "2.<tab>The Fox of Fable<tabs>Miho's theme." in columns).
        static readonly Regex LineBreaks = new Regex(@"<br\s*/?>|\\n|\r?\n|\t+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Like Clean, but keeps line breaks as pauses ("1. A Fox's Journey, The main menu theme.").
        /// Handles real newlines, &lt;br&gt; tags and the literal backslash-n some of the game's text contains.
        /// </summary>
        public static string CleanLines(string text, string separator = ", ")
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            // Some text has a literal backslash-t for spacing; treat it as a space.
            var lines = LineBreaks.Split(text.Replace("\\t", " ")).Select(Clean).Where(l => l.Length > 0).ToList();
            var sb = new System.Text.StringBuilder();
            string previous = "";
            foreach (string line in lines)
            {
                if (sb.Length > 0)
                {
                    // A line starting in lower case continues a sentence the game only wrapped for layout.
                    // In lists (", " between lines) only after a long line, though: short lines are entries
                    // ("Kira Boom", "phenoct"). Pop-ups (". ") are never lists ("A test notice. with...").
                    bool ended = ".!?。！？…".IndexOf(sb[sb.Length - 1]) >= 0;
                    bool wrapped = char.IsLower(line[0]) && !ended && (previous.Length >= 25 || separator.StartsWith("."));
                    sb.Append(wrapped ? " " : ended ? separator.TrimStart('.', ',') : separator);
                }
                sb.Append(line);
                previous = line;
            }
            return sb.ToString();
        }

        /// <summary>False for empty text and placeholders like "?" or "???" (locked entries).</summary>
        public static bool IsReadable(string s) => !string.IsNullOrEmpty(s) && s.Trim('?', ' ').Length > 0;

        /// <summary>Joins pieces into sentences, adding a full stop only where one is missing.</summary>
        public static string Sentences(System.Collections.Generic.IEnumerable<string> parts)
        {
            var sb = new System.Text.StringBuilder();
            foreach (string p in parts)
            {
                if (sb.Length > 0) sb.Append(' ');
                sb.Append(p);
                if (!".!?。！？…\"'”".Contains(p[p.Length - 1].ToString())) sb.Append('.');
            }
            return sb.ToString();
        }
    }
}
