using System.IO;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PleaseBeHappyAccess
{
    [BepInPlugin(Guid, "Please Be Happy Access", VersionString)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Guid = "shaysters.pleasebehappy.access";
        public const string VersionString = "1.0.1";

        void Awake()
        {
            Log.Init(Logger);
            Speech.Initialize(Path.GetDirectoryName(Info.Location));
            Descriptions.Load(Path.GetDirectoryName(Info.Location));
#if DEV_TOOLS
            // Scripted in-game tests; only in development builds (see PleaseBeHappyAccess.csproj).
            TestDriver.Load(Paths.ConfigPath);
#endif
            // Patch each class on its own, so a game update that breaks one hook doesn't disable the rest.
            var harmony = new Harmony(Guid);
            foreach (System.Type type in AccessTools.GetTypesFromAssembly(typeof(Plugin).Assembly))
            {
                try
                {
                    harmony.CreateClassProcessor(type).Patch();
                }
                catch (System.Exception e)
                {
                    Log.Error($"Could not apply {type.Name}: {e.Message}");
                }
            }

            // Our own hidden object for the per-frame tick, so it doesn't depend on the plugin object's lifetime.
            var runner = new GameObject("PleaseBeHappyAccess.Runner");
            runner.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(runner);
            UpdateCheck.Start(runner.AddComponent<Runner>());

            Log.Info("Please Be Happy Access loaded.");
            Speech.Say(Strings.Get("loaded"));
        }
    }

    internal class Runner : MonoBehaviour
    {
        public static Runner Instance { get; private set; }

        void Awake() => Instance = this;

        void Update()
        {
            // Each part runs on its own: if one fails (say, a different version of the game from another
            // store), the rest keep working.
            Run("TitleButton", TitleButton.Tick);
            Run("ModMenu", ModMenu.Tick);
            if (!ModMenu.Open)
            {
                Run("Gallery", Gallery.Tick);
                Run("ScreenFocus", ScreenFocus.Tick);
                Run("SceneWatcher", SceneWatcher.Tick);
                Run("FocusReader", FocusReader.Tick);
                Run("Hotkeys", Hotkeys);
            }
#if DEV_TOOLS
            Run("TestDriver", TestDriver.Tick);
#endif
        }

        static readonly System.Collections.Generic.Dictionary<string, string> lastError =
            new System.Collections.Generic.Dictionary<string, string>();

        static void Run(string name, System.Action tick)
        {
            try
            {
                tick();
            }
            catch (System.Exception e)
            {
                // Log each different error once, not every frame.
                string text = e.ToString();
                if (lastError.TryGetValue(name, out string before) && before == text) return;
                lastError[name] = text;
                Log.Error(name + ": " + text);
            }
        }

        static void Hotkeys()
        {
            Keyboard kb = Keyboard.current;
            if (kb == null) return;
            // Letter keys the game doesn't use (it has K, H and L). Not with Ctrl or Alt held.
            if (kb.ctrlKey.isPressed || kb.altKey.isPressed) return;
            // V: shortcut for the most-used setting; the same switch is in the mod settings.
            if (kb.vKey.wasPressedThisFrame)
            {
                Settings.ReadVoicedLines.Value = !Settings.ReadVoicedLines.Value;
                Speech.Say(Strings.Get(Settings.ReadVoicedLines.Value ? "voicedOn" : "voicedOff"), interrupt: true);
            }
            // D: describe the scene — backgrounds, CGs, and who is on screen with their expression — or the open gallery picture.
            if (kb.dKey.wasPressedThisFrame)
            {
                if (Gallery.Showing) Gallery.Repeat();
                else SceneWatcher.DescribeScene();
            }
            // R: read the open screen's text, or repeat the last line during the story.
            if (kb.rKey.wasPressedThisFrame)
            {
                if (Gallery.Showing) Gallery.Repeat();
                else if (ScreenFocus.HasScreen) ScreenFocus.ReadScreen();
                else Speech.Say(PrintTextPatch.LastLine ?? Strings.Get("noText"), interrupt: true);
            }
        }
    }
}
