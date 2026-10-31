using System;
using System.Linq;
using HarmonyLib;
using Naninovel.UI;
using TMPro;

namespace PleaseBeHappyAccess
{
    /// <summary>
    /// The game's pop-ups. Song names ("Now playing", MihoKitMusicPlayer), achievements
    /// (AkemiTrophyHandlerNotification) and other notices all go through Naninovel's ToastUI.Show;
    /// the day and time of day has its own pop-up (z04TimeDatePopup). Turned off with "Read pop-ups";
    /// song names with "Read the name of each song".
    /// </summary>
    internal static class PopupPatches
    {
        [HarmonyPatch(typeof(ToastUI), nameof(ToastUI.Show))]
        static class ToastPatch
        {
            static void Prefix(string text, string appearance)
            {
                try
                {
                    // The music player's "Now playing" toast (MihoKitMusicPlayer) has its own setting.
                    bool song = appearance == "nowPlaying";
                    if (!(song ? Settings.ReadSongs.Value : Settings.ReadPopups.Value)) return;
                    // Lines become sentences: "A Fox's Journey. Now playing."
                    string s = TextUtil.CleanLines(text, ". ");
                    if (s.Length > 0) Speech.Say(TextUtil.Sentences(new[] { s }), interrupt: false);
                }
                catch (Exception e)
                {
                    Log.Error("ToastPatch: " + e);
                }
            }
        }

        /// <summary>
        /// The place-and-time card: z04NavigationAssetManager.SetItem shows a place name as text and the
        /// time of day as a picture named time_morning, time_afternoon, time_evening, time_night or time_unknown.
        /// </summary>
        [HarmonyPatch(typeof(z04NavigationAssetManager), "SetItem")]
        internal static class NavigationPatch
        {
            /// <summary>Strings.cs key for the time of day last shown, or null.</summary>
            public static string TimeKey;

            static void Postfix(z04NavigationAssetManager.Location item)
            {
                string tex = item?.bgTex != null ? item.bgTex.name : null;
                if (tex == null || !tex.StartsWith("time_")) return;
                string t = tex.Substring(5);
                TimeKey = t.StartsWith("morning") ? "timeMorning"
                    : t.StartsWith("afternoo") ? "timeAfternoon"
                    : t.StartsWith("evening") ? "timeEvening"
                    : t.StartsWith("n") ? "timeNight"
                    : null;
            }
        }

        [HarmonyPatch(typeof(z04TimeDatePopup), nameof(z04TimeDatePopup.PlayAnim))]
        static class TimeDatePatch
        {
            static void Postfix(z04TimeDatePopup __instance)
            {
                // The pop-up fills in and shows its text a second later (ShowHide waits 1 s).
                if (Runner.Instance != null) Runner.Instance.StartCoroutine(ReadLater(__instance));
            }

            static System.Collections.IEnumerator ReadLater(z04TimeDatePopup popup)
            {
                yield return new UnityEngine.WaitForSecondsRealtime(1.5f);
                Read(popup);
            }

            static string lastText;
            static float lastAt;

            static void Read(z04TimeDatePopup __instance)
            {
                try
                {
                    // Fired several times while the title screen loads; only the story's pop-ups matter.
                    if (__instance == null || !Settings.ReadPopups.Value || ScreenFocus.HasScreen) return;
                    var parts = __instance.GetComponentsInChildren<TMP_Text>(true)
                        .Select(t => TextUtil.CleanLines(t.text))
                        .Where(TextUtil.IsReadable)
                        .Distinct()
                        .ToList();
                    // The time of day is a picture; say it in words.
                    if (NavigationPatch.TimeKey != null) parts.Add(Strings.Get(NavigationPatch.TimeKey));
                    string text = TextUtil.Sentences(parts);
                    // The game plays this pop-up on several screens at once and again when menus open;
                    // say each one once.
                    float now = UnityEngine.Time.realtimeSinceStartup;
                    if (parts.Count == 0 || (text == lastText && now - lastAt < 30f)) return;
                    lastText = text;
                    lastAt = now;
                    Speech.Say(text, interrupt: false);
                }
                catch (Exception e)
                {
                    Log.Error("TimeDatePatch: " + e);
                }
            }
        }
    }
}
