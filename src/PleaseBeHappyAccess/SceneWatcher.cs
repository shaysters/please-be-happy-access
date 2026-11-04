using System.Collections.Generic;
using System.Linq;
using Naninovel;
using UnityEngine;

namespace PleaseBeHappyAccess
{
    /// <summary>
    /// Watches which backgrounds/CGs and characters are showing and announces changes:
    /// a new background or CG is described; a character arriving is announced by name (with a full
    /// description the first time you ever meet them) and leaving is announced too.
    /// Watching actor state, rather than hooking script commands, also covers loading saves and rollback.
    /// </summary>
    internal static class SceneWatcher
    {
        static readonly Dictionary<string, string> backgrounds = new Dictionary<string, string>();

        /// <summary>
        /// The appearance names the story script asked for ("crowd1", "Miho.smile,up"). Layered images
        /// and characters report every internal layer as their appearance instead, so we remember these.
        /// </summary>
        internal static readonly Dictionary<string, string> ScriptBackground = new Dictionary<string, string>();
        internal static readonly Dictionary<string, string> ScriptCharacter = new Dictionary<string, string>();
        static readonly HashSet<string> characters = new HashSet<string>();
        // Keyed by background layer: if a layer changes again before we speak, only its newest image is described.
        static readonly Dictionary<string, string> pendingScene = new Dictionary<string, string>();
        static readonly List<string> pendingPeople = new List<string>();

        /// <summary>
        /// Set when the last announcement included a scene description, so the next line is
        /// introduced as story text and the two can be told apart.
        /// </summary>
        public static bool LabelNextLine;

        static IScriptPlayer listeningTo;
        static IStateManager stateListeningTo;

        /// <summary>Set by loading a save: describe the loaded scene afresh once the menus close.</summary>
        static bool reannounce;

        /// <summary>
        /// The script names we remember belong to the story position we left. After loading a save,
        /// rolling back or going to the title, forget them so stale names aren't described.
        /// </summary>
        static void ForgetScriptNames()
        {
            ScriptBackground.Clear();
            ScriptCharacter.Clear();
        }

        public static void Tick()
        {
            if (!Engine.Initialized) return;
            // Listen to each script command as it starts (ScriptPlayer.OnCommandExecutionStart,
            // Naninovel/ScriptPlayer.cs:679). Patching ModifyBackground directly isn't safe: its
            // ExecuteAsync is a shared generic method, and patching it crashed the game.
            IScriptPlayer player = Engine.GetService<IScriptPlayer>();
            if (player != null && player != listeningTo)
            {
                player.OnCommandExecutionStart += RememberScriptAppearance;
                listeningTo = player;
            }
            IStateManager state = Engine.GetService<IStateManager>();
            if (state != null && state != stateListeningTo)
            {
                state.OnGameLoadFinished += _ => { ForgetScriptNames(); reannounce = true; };
                state.OnRollbackFinished += ForgetScriptNames;
                state.OnResetFinished += ForgetScriptNames;
                stateListeningTo = state;
            }
            // Menus (the title screen, pause menu) can change actors behind them; take note silently.
            bool quiet = ScreenFocus.HasScreen;
            if (reannounce && !quiet)
            {
                // Back in the story after loading: forget what was showing so the loaded scene and
                // everyone in it are announced.
                reannounce = false;
                backgrounds.Clear();
                characters.Clear();
                pendingScene.Clear();
                pendingPeople.Clear();
            }
            if (holdForVoice)
            {
                // A voiced line is on screen: release the hold once its clip has played and stopped.
                bool playing = VoicePlaying;
                if (playing) sawVoice = true;
                else if (sawVoice || Time.realtimeSinceStartup - holdSince > 20f)
                {
                    holdForVoice = false;
                    releasedForVoice = true;
                }
            }
            Poll(quiet);
            // While the story is moving, hold new descriptions for the next line (BeforeLine): its voice clip
            // starts just before it prints, and the description then waits for the voice to finish.
            // Speaking straight away would start a moment before the voice and overlap it. If no line comes
            // within a few seconds (a long transition), or a voice clip just ended, speak now.
            bool pendingAny = pendingScene.Count > 0 || pendingPeople.Count > 0;
            if (!pendingAny) pendingSince = -1f;
            else if (pendingSince < 0f) pendingSince = Time.realtimeSinceStartup;
            bool idle = player == null || !player.Playing;
            if (idle || releasedForVoice || (pendingAny && Time.realtimeSinceStartup - pendingSince > 3f)) Flush();
            releasedForVoice = false;
        }

        static bool holdForVoice, sawVoice, releasedForVoice;
        static float pendingSince = -1f;
        static float holdSince;

        /// <summary>
        /// Called right before a line is printed, so a scene change is described before the line —
        /// or, when the line is voiced, after the voice actor has finished it.
        /// </summary>
        public static void BeforeLine(bool voiced)
        {
            if (!Engine.Initialized) return;
            holdForVoice = voiced;
            sawVoice = false;
            holdSince = Time.realtimeSinceStartup;
            Poll(quiet: false);
            Flush();
        }

        /// <summary>
        /// True while a voice clip plays. Descriptions wait until it ends so they never talk over
        /// the voice actors (the clip starts before its line prints, Naninovel.Commands/PrintText.cs:104).
        /// </summary>
        static bool VoicePlaying
        {
            get
            {
                IAudioManager audio = Engine.GetService<IAudioManager>();
                if (audio == null) return false;
                if (audio.GetPlayedVoicePath() != null) return true;
                // @voice commands don't always register as "the played voice"; ask about the clip itself.
                string last = VoicePatch.LastPath;
                return last != null && audio.GetVoiceTrack(last) is IAudioTrack track && track.Playing;
            }
        }

        /// <summary>Notes the appearance names script lines set, for layered images and characters.</summary>
        static void RememberScriptAppearance(Command command)
        {
            try
            {
                if (command is Naninovel.Commands.ModifyBackground back)
                {
                    var app = back.AppearanceAndTransition;
                    if (app == null || !app.HasValue || string.IsNullOrEmpty(app.Value?.Name)) return;
                    string id = back.Id != null && back.Id.HasValue ? back.Id.Value : BackgroundsConfiguration.MainActorId;
                    ScriptBackground[id] = app.Value.Name;
                }
                else if (command is Naninovel.Commands.ModifyCharacter chr)
                {
                    var named = chr.IdAndAppearance;
                    if (named == null || !named.HasValue || named.Value == null) return;
                    string appearance = named.Value.Value != null && named.Value.Value.HasValue ? named.Value.Value.Value : null;
                    if (!string.IsNullOrEmpty(named.Value.Name) && !string.IsNullOrEmpty(appearance))
                        ScriptCharacter[named.Value.Name] = appearance;
                }
            }
            catch (System.Exception e)
            {
                Log.Error("RememberScriptAppearance: " + e.Message);
            }
        }

        static void Poll(bool quiet)
        {
            var bgManager = Engine.GetService<IBackgroundManager>();
            if (bgManager != null)
            {
                foreach (IBackgroundActor bg in bgManager.GetAllActors())
                {
                    string now = bg.Visible ? (ScriptBackground.TryGetValue(bg.Id, out string named) ? named : bg.Appearance) : null;
                    backgrounds.TryGetValue(bg.Id, out string before);
                    if (now == before) continue;
                    backgrounds[bg.Id] = now;
                    if (quiet || !Settings.DescribeScenes.Value) continue;
                    string desc = now == null ? null : DescribeBackground(bg.Id, now);
                    if (desc != null) pendingScene[bg.Id] = desc;
                    else pendingScene.Remove(bg.Id); // hidden or not described: drop anything stale
                }
            }

            var charManager = Engine.GetService<ICharacterManager>();
            if (charManager != null)
            {
                foreach (ICharacterActor ch in charManager.GetAllActors())
                {
                    bool visible = ch.Visible;
                    bool was = characters.Contains(ch.Id);
                    if (visible == was) continue;
                    if (visible) characters.Add(ch.Id); else characters.Remove(ch.Id);
                    if (quiet || !visible || !Settings.DescribeCharacters.Value) continue;
                    // Comings and goings aren't announced (D says who's on screen); only the first meeting
                    // is described. The description starts with their name, so it waits until the story
                    // has told us who they are.
                    if (!NameHidden(ch.Id) && Descriptions.Get("char/" + ch.Id) is string full && FirstMeeting(ch.Id))
                        pendingPeople.Add(full);
                }
            }
        }

        static void Flush()
        {
            if (pendingScene.Count == 0 && pendingPeople.Count == 0) return;
            // Said once the voice line ends (Tick keeps calling Flush). A voiced line's clip can take a
            // moment to load, so the hold covers that gap too (released in Tick, at most 20 seconds).
            if (VoicePlaying || holdForVoice) return;
            // Scene descriptions switched off since they were noted: drop them, so nothing is said
            // and the next line isn't labelled "Story text".
            if (!Settings.DescribeScenes.Value) pendingScene.Clear();
            if (!Settings.DescribeCharacters.Value) pendingPeople.Clear();
            if (pendingScene.Count == 0 && pendingPeople.Count == 0) return;
            var parts = new List<string>();
            if (pendingScene.Count > 0) parts.Add(Strings.Get("sceneLabel") + " " + TextUtil.Sentences(pendingScene.Values));
            parts.AddRange(pendingPeople);
            Speech.Say(TextUtil.Sentences(parts), interrupt: false);
            LabelNextLine = pendingScene.Count > 0;
            pendingScene.Clear();
            pendingPeople.Clear();
            // The line that follows queues after this instead of cutting it off.
            Speech.KeepNextLine = true;
        }

        public static string DescribeBackground(string id, string appearance)
        {
            if (string.IsNullOrEmpty(appearance) || Descriptions.Has("skip/" + appearance)) return null;
            // Layered images (the dog, cut-ins) can combine parts in ways not listed, so fall back
            // to a description of the image as a whole.
            return Descriptions.Get($"bg/{id}/{appearance}") ?? Descriptions.Get("bg/" + appearance) ?? Descriptions.Get("bgid/" + id);
        }

        static bool FirstMeeting(string id)
        {
            string key = "PleaseBeHappyAccess.Met." + id;
            if (PlayerPrefs.GetInt(key, 0) != 0) return false;
            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.Save();
            return true;
        }

        /// <summary>
        /// The name the game shows. Until the story introduces someone the game shows "???", so we say
        /// "Someone" rather than give their name away. Actors with no display name at all use their id.
        /// </summary>
        internal static string Name(string id)
        {
            string name = TextUtil.Clean(Engine.GetService<ICharacterManager>()?.GetDisplayName(id));
            if (TextUtil.IsReadable(name)) return name;
            return NameHidden(id) || !string.IsNullOrEmpty(name) ? Strings.Get("someone") : id;
        }

        /// <summary>
        /// True until the story reveals this character's name. The game keeps that in a variable such as
        /// "mrKimName" and shows "???" or a label like "Mother" or "Barista" until then
        /// (MihoKitCharacterNameHandler.getDynamicCharacterDisplayNameViaAkemiLocaliserForCharacterNameID).
        /// </summary>
        static bool NameHidden(string id)
        {
            var vars = Engine.GetService<ICustomVariableManager>();
            if (vars == null || string.IsNullOrEmpty(id)) return false;
            string key = char.ToLowerInvariant(id[0]) + id.Substring(1) + "Name";
            return vars.VariableExists(key) && !IsTrue(vars.GetVariableValue(key));
        }

        static bool IsTrue(string value) => bool.TryParse(value, out bool b) && b;

        /// <summary>D: the current backgrounds/CGs, then everyone on screen with their expression and pose.</summary>
        public static void DescribeScene()
        {
            var parts = new List<string>();
            var bgManager = Engine.GetService<IBackgroundManager>();
            if (bgManager != null)
                foreach (IBackgroundActor bg in bgManager.GetAllActors().Where(b => b.Visible))
                    if (DescribeBackground(bg.Id, ScriptBackground.TryGetValue(bg.Id, out string named) ? named : bg.Appearance) is string d) parts.Add(d);
            var charManager = Engine.GetService<ICharacterManager>();
            if (charManager != null)
                foreach (ICharacterActor ch in charManager.GetAllActors().Where(c => c.Visible))
                    parts.Add(DescribeCharacter(ch));
            Speech.Say(parts.Count > 0 ? TextUtil.Sentences(parts) : Strings.Get("noImage"), interrupt: true);
        }

        static string DescribeCharacter(ICharacterActor ch)
        {
            // The parts the script last set, e.g. "closed,uneven,small,down,stretch".
            ScriptCharacter.TryGetValue(ch.Id, out string parts);
            var words = (parts ?? "").Split(',')
                // Some characters' parts are named with their layer, e.g. Minseo's "Eyes>uclosed".
                .Select(p => p.Trim().Substring(p.Trim().IndexOf('>') + 1))
                // A part can mean different things per character ("up": Miho's ears, Mr. Kim's arm).
                .Select(p => Descriptions.Get($"part/{ch.Id}/{p}") ?? Descriptions.Get("part/" + p))
                .Where(w => !string.IsNullOrEmpty(w))
                .Distinct()
                .ToList();
            return words.Count > 0 ? Name(ch.Id) + ": " + string.Join(", ", words) : Name(ch.Id);
        }
    }
}
