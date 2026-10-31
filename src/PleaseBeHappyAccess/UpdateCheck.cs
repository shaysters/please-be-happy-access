using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace PleaseBeHappyAccess
{
    /// <summary>
    /// Once per game start, asks GitHub for the latest release and tells the player if it's newer
    /// than the installed mod. Reads the Loadstone release manifest the installer itself uses
    /// (releases/latest/download/loadstone-release.json — a plain redirect, no API token or rate limit).
    /// Turned off with the "Check for updates" setting. Nothing is sent except the request itself.
    /// </summary>
    internal static class UpdateCheck
    {
        const string ManifestUrl = "https://github.com/shaysters/please-be-happy-access/releases/latest/download/loadstone-release.json";
        static readonly Regex VersionField = new Regex("\"version\"\\s*:\\s*\"v?([0-9.]+)\"");

        public static void Start(MonoBehaviour host)
        {
            if (!Settings.CheckForUpdates.Value) return;
            host.StartCoroutine(Run(ManifestUrl, 15f));
        }

        /// <summary>Test hook: the same check against another release manifest, right away.</summary>
        public static void RunNow(MonoBehaviour host, string url) => host.StartCoroutine(Run(url, 0f));

        static IEnumerator Run(string url, float delay)
        {
            // Let the title screen settle so the announcement isn't lost in start-up speech.
            yield return new WaitForSecondsRealtime(delay);
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.timeout = 15;
                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    // Expected while the repository is private or offline; never bother the player.
                    Log.Info($"Update check skipped: {request.error}");
                    yield break;
                }
                Match m = VersionField.Match(request.downloadHandler.text);
                if (!m.Success) yield break;
                Version latest = Parse(m.Groups[1].Value);
                Version installed = Parse(Plugin.VersionString);
                Log.Info($"Update check: installed {installed}, latest {latest}");
                if (latest != null && installed != null && latest > installed)
                    Speech.Say(string.Format(Strings.Get("updateAvailable"), m.Groups[1].Value), interrupt: false);
            }
        }

        static Version Parse(string s)
        {
            try { return new Version(s); }
            catch { return null; }
        }
    }
}
