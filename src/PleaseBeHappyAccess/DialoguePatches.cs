using System;
using HarmonyLib;
using Naninovel;

namespace PleaseBeHappyAccess
{
    /// <summary>
    /// Reads each line of dialogue and narration as Naninovel starts printing it.
    /// Hook point: TextPrinterManager.PrintTextAsync (Naninovel/TextPrinterManager.cs:94) —
    /// every @print and generic text line in the scripts goes through it.
    /// </summary>
    [HarmonyPatch(typeof(TextPrinterManager), nameof(TextPrinterManager.PrintTextAsync))]
    internal static class PrintTextPatch
    {
        /// <summary>The most recent line, for R (repeat) during the story.</summary>
        public static string LastLine { get; private set; }

        static void Prefix(string text, string authorId)
        {
            try
            {
                bool voiced = VoicePatch.TakeVoiceFlag();
                SceneWatcher.BeforeLine(voiced);
                string line = TextUtil.Clean(text);
                if (line.Length == 0) return;
                string speaker = SpeakerName(authorId);
                string spoken = string.IsNullOrEmpty(speaker) ? line : speaker + ": " + line;
                LastLine = spoken;
                if (voiced && !Settings.ReadVoicedLines.Value)
                {
                    Log.Info("Voiced line not read (setting off): " + spoken);
                    // The "Story text:" label belonged to this line; don't carry it to the next one.
                    SceneWatcher.LabelNextLine = false;
                    Speech.KeepNextLine = false;
                    return;
                }
                // After a scene description, say where the story picks up again.
                if (SceneWatcher.LabelNextLine && Settings.DescribeScenes.Value) spoken = Strings.Get("storyLabel") + " " + spoken;
                SceneWatcher.LabelNextLine = false;
                Speech.Say(spoken, interrupt: !Speech.KeepNextLine);
                Speech.KeepNextLine = false;
            }
            catch (Exception e)
            {
                Log.Error("PrintTextPatch: " + e);
            }
        }

        internal static string SpeakerName(string authorId)
        {
            if (string.IsNullOrEmpty(authorId)) return null;
            var characters = Engine.GetService<ICharacterManager>();
            string name = characters?.GetDisplayName(authorId);
            return TextUtil.Clean(string.IsNullOrEmpty(name) ? authorId : name);
        }
    }

    /// <summary>
    /// Notes when a voice clip starts. PrintText.ExecuteAsync plays the auto-voice before printing
    /// (Naninovel.Commands/PrintText.cs:104-120), and @voice commands run before the line they voice,
    /// so by the time PrintTextAsync runs, the flag tells us whether this line is voiced.
    /// </summary>
    [HarmonyPatch(typeof(AudioManager), nameof(AudioManager.PlayVoiceAsync))]
    internal static class VoicePatch
    {
        static bool voiceStarted;

        /// <summary>The most recent voice clip, so scene descriptions can wait for it to end.</summary>
        public static string LastPath { get; private set; }

        static void Prefix(string path)
        {
            Log.Info("Voice: " + path);
            // Title-screen greetings (menulines/...) aren't story lines; they mustn't hold descriptions back.
            if (path != null && path.StartsWith("menulines/")) return;
            voiceStarted = true;
            startedAt = UnityEngine.Time.realtimeSinceStartup;
            LastPath = path;
        }

        static float startedAt;

        public static bool TakeVoiceFlag()
        {
            // Only a clip started just before this line counts: the title screen's voice
            // ("Please be happy") must not mark the first story line as voiced.
            bool v = voiceStarted && UnityEngine.Time.realtimeSinceStartup - startedAt < 5f;
            voiceStarted = false;
            return v;
        }
    }
}
