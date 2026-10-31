using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Naninovel;
using Naninovel.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PleaseBeHappyAccess
{
    /// <summary>
    /// Gives keyboard focus to menu screens and keeps it there.
    ///
    /// The game's sub-screens (Options, Load, Extras, Help...) open without taking focus, so the arrow
    /// keys kept moving through the hidden title menu behind them. We watch Naninovel's managed UIs;
    /// the screens shown most recently are the "current screen". When it changes we focus its first
    /// control, and Up/Down walk its controls top to bottom without leaving it.
    /// </summary>
    internal static class ScreenFocus
    {
        /// <summary>UIs that are part of play, not menu screens — never take focus for these.</summary>
        static readonly string[] NotScreens =
        {
            "TextPrinter", "QuickMenu", "ContinueInput", "ClickThrough", "Toast", "Loading",
            "ScriptNavigator", "ExternalScripts", "SceneTransition", "UI Skip", "UI Auto", "MovieUI",
        };

        /// <summary>The sidebar that opens beside main-menu pages; its controls go after the page's own.</summary>
        const string Sidebar = "PBHNavigation";

        static readonly Dictionary<IManagedUI, int> shownAtFrame = new Dictionary<IManagedUI, int>();
        static List<MonoBehaviour> current = new List<MonoBehaviour>();
        static int focusAtFrame = -1;

        /// <summary>True when the current selection is a control inside the open screen.</summary>
        internal static bool OwnsFocus
        {
            get
            {
                GameObject sel = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
                return sel != null && sel.activeInHierarchy && InCurrentScreen(sel.transform);
            }
        }

        /// <summary>True while a menu screen is open (false during the story itself).</summary>
        public static bool HasScreen => current.Count > 0 && !IgnoreScreens;

        /// <summary>Test hook: menus hidden by the test driver's hideui command count as closed.</summary>
        internal static bool IgnoreScreens;

        /// <summary>A drop-down list was open as of the last frame (read by the Escape patch below).</summary>
        internal static bool DropdownWasOpen;

        public static void Tick()
        {
            DropdownWasOpen = current.Any(ui => ui != null && ui.GetComponentsInChildren<TMPro.TMP_Dropdown>(false).Any(d => d.IsExpanded));
            if (!Engine.Initialized) return;
            IUIManager uiManager = Engine.GetService<IUIManager>();
            if (uiManager == null) return;

            bool changed = false;
            foreach (IManagedUI ui in uiManager.GetManagedUIs())
            {
                if (!(ui is MonoBehaviour mb) || ui is UITextPrinterPanel || IsNotScreen(mb)) continue;
                bool visible = ui.Visible && mb.isActiveAndEnabled;
                if (visible && !shownAtFrame.ContainsKey(ui))
                {
                    shownAtFrame[ui] = Time.frameCount;
                    changed = true;
                }
                else if (!visible && shownAtFrame.Remove(ui))
                {
                    changed = true;
                }
            }

            if (changed)
            {
                // The current screen is everything shown together most recently (a page plus its sidebar).
                int newest = shownAtFrame.Count > 0 ? shownAtFrame.Values.Max() : -1;
                current = shownAtFrame.Where(kv => kv.Value >= newest - 5)
                    .Select(kv => (MonoBehaviour)kv.Key)
                    .OrderBy(mb => mb.name.Contains(Sidebar) ? 1 : 0)
                    .ToList();
                // Wait a couple of frames: screens fill in their buttons right after showing.
                focusAtFrame = Time.frameCount + 2;
                Log.Info("Screen: " + string.Join(", ", current.Select(mb => mb.name)));
                AnnounceIntro();
            }

            EventSystem es = EventSystem.current;
            if (es == null) return;
            if (current.Count == 0)
            {
                // Back in the story: drop focus left on a closed menu, or the arrow keys and Enter
                // would act on hidden buttons (seen: title-screen buttons during chapter 1).
                // The game can also select one again later (seen: "Start" mid-story), so check every frame.
                pendingTab = null;
                GameObject sel = es.currentSelectedGameObject;
                if (sel != null && !IgnoreScreens && (changed || InHiddenUI(sel.transform))) es.SetSelectedGameObject(null);
                return;
            }
            GameObject selected = es.currentSelectedGameObject;
            // A tab's content fills in a moment after it opens (Patrons first shows placeholder text,
            // "Miho Plushie" over and over), so the tab is announced shortly after it changes.
            if (TabChanged(out Toggle changedTab))
            {
                pendingTab = changedTab;
                pendingTabAt = Time.realtimeSinceStartup + 0.4f;
            }
            if (pendingTab != null)
            {
                // Until then, leave focus alone (the game may drop it while switching tabs).
                if (Time.realtimeSinceStartup < pendingTabAt) return;
                Toggle tab = pendingTab;
                pendingTab = null;
                if (tab != null && tab.isOn && tab.gameObject.activeInHierarchy)
                {
                    // A new tab's content replaced the old one: say which tab it is (and its text, for
                    // tabs that are mostly text), then go to its first control.
                    Selectable firstSetting = Controls().FirstOrDefault(s => s.transform.parent != tab.transform.parent);
                    Speech.Say(TabIntro(tab), interrupt: true);
                    if (firstSetting != null)
                    {
                        FocusReader.QueueNext = true;
                        es.SetSelectedGameObject(firstSetting.gameObject);
                    }
                    return;
                }
            }
            bool lost = selected == null || !selected.activeInHierarchy;
            bool waiting = focusAtFrame >= 0 && Time.frameCount < focusAtFrame;
            bool due = focusAtFrame >= 0 && !waiting;
            if (waiting || (!due && !lost)) return;
            focusAtFrame = -1;
            if (!lost && InCurrentScreen(selected.transform)) return;
            List<Selectable> controls = Controls();
            // In the backlog, start on the newest line (the last entry), like reading back from where you were.
            Selectable first = controls.LastOrDefault(s => FocusReader.BacklogMessageOf(s.transform) != null)
                // Screens with tabs (Options) start on the open tab's first setting; Up reaches the tabs.
                ?? controls.FirstOrDefault(s => !(s is Toggle t && t.group != null && FocusReader.IsTabRow(t)))
                ?? controls.FirstOrDefault();
            if (first == null) return;
            TabChanged(out Toggle openTab);
            if (openTab != null && first.transform.parent != openTab.transform.parent)
            {
                Speech.Say(TabIntro(openTab), interrupt: false);
                FocusReader.QueueNext = true;
            }
            es.SetSelectedGameObject(first.gameObject);
            // Naninovel would otherwise re-focus the screen's default button on the first arrow press
            // (ScriptableUIBehaviour.HandleNavigationFocus, Naninovel/ScriptableUIBehaviour.cs:366),
            // undoing the player's first move.
            Traverse.Create(typeof(ScriptableUIBehaviour)).Property("FocusOnNavigation").SetValue(null);
        }

        static Toggle lastTab;
        static Toggle pendingTab;
        static float pendingTabAt;

        /// <summary>
        /// "HELP tab." plus, for tabs that are mostly text (Help's tips and controls, the Episodes and
        /// Gallery lists of locked entries), the text itself — otherwise there'd be nothing to hear.
        /// Repeated entries are counted: "??? Locked! (46 of these)".
        /// </summary>
        static string TabIntro(Toggle tab)
        {
            string intro = string.Format(Strings.Get("tabOpened"), FocusReader.LabelOf(tab.gameObject));
            MonoBehaviour ui = current.FirstOrDefault(u => u != null && tab.transform.IsChildOf(u.transform));
            if (ui == null) return intro;
            Transform tabRow = tab.transform.parent;
            int controls = ui.GetComponentsInChildren<Selectable>(false)
                .Count(s => s.IsInteractable() && !(s is Scrollbar) && !s.transform.IsChildOf(tabRow));
            if (controls > 1) return intro;
            var counts = new List<KeyValuePair<string, int>>();
            foreach (TMPro.TMP_Text t in ui.GetComponentsInChildren<TMPro.TMP_Text>(false))
            {
                if (!t.enabled || t.transform.IsChildOf(tabRow) || InPreviewPrinter(t.transform)) continue;
                Selectable owner = t.GetComponentInParent<Selectable>();
                if (owner != null && owner.IsInteractable()) continue; // read when focused instead
                string s = TextUtil.CleanLines(t.text);
                if (!TextUtil.IsReadable(s)) continue;
                int i = counts.FindIndex(kv => kv.Key == s);
                if (i < 0) counts.Add(new KeyValuePair<string, int>(s, 1));
                else counts[i] = new KeyValuePair<string, int>(s, counts[i].Value + 1);
            }
            if (counts.Count == 0) return intro;
            return TextUtil.Sentences(new[] { intro }.Concat(counts.Select(kv =>
                kv.Value > 1 ? string.Format(Strings.Get("repeatedN"), kv.Key, kv.Value) : kv.Key)));
        }

        /// <summary>The open tab of the screen's tab row changed since last frame (not counting the screen opening).</summary>
        static bool TabChanged(out Toggle tab)
        {
            tab = current.Where(ui => ui != null)
                .SelectMany(ui => ui.GetComponentsInChildren<Toggle>(false))
                .FirstOrDefault(t => t.isOn && t.group != null && FocusReader.IsTabRow(t));
            bool changed = tab != null && lastTab != null && tab != lastTab && tab.transform.parent == lastTab.transform.parent;
            lastTab = tab;
            return changed;
        }

        /// <summary>True when moving from one control to the other would leave the current screen.</summary>
        internal static bool LeavesScreen(Selectable from, Selectable to) =>
            current.Count > 0 && from != null && to != null && InCurrentScreen(from.transform) && !InCurrentScreen(to.transform);

        /// <summary>
        /// Text worth hearing as soon as a screen opens, by screen name and path inside it.
        /// The in-game pause menu shows where you are in the story.
        /// </summary>
        static readonly Dictionary<string, string[]> Intros = new Dictionary<string, string[]>
        {
            // Only shown at start-up, until the first key press.
            ["PBHTitleUI"] = new[] { "Presplash/Text (TMP)" },
            // "Welcome to the extras screen! More things will unlock as you complete more of the game."
            ["PBHExtras"] = new[] { "ExtrasMain/Desc" },
            // Yes/no questions ("Are you sure you want to quit to the title screen?...").
            ["PBHConfirmationUI"] = new[] { "Content/DialoguePanel/MessageText" },
            [Sidebar] = new[]
            {
                "NavigationSidebar/SideBar/InGame/Ch Title",
                "Pause/Text/Title",
                "Pause/Text/Desc",
            },
        };

        static void AnnounceIntro()
        {
            var parts = new List<string>();
            foreach (MonoBehaviour ui in current)
            {
                if (ui == null || !Intros.TryGetValue(ui.name, out string[] paths)) continue;
                foreach (string path in paths)
                {
                    Transform t = ui.transform.Find(path);
                    TMPro.TMP_Text text = t != null && t.gameObject.activeInHierarchy ? t.GetComponent<TMPro.TMP_Text>() : null;
                    string s = text != null ? TextUtil.Clean(text.text) : "";
                    if (TextUtil.IsReadable(s)) parts.Add(s);
                }
            }
            if (parts.Count == 0) return;
            Speech.Say(TextUtil.Sentences(parts), interrupt: true);
            FocusReader.QueueNext = true;
        }

        /// <summary>R: every piece of plain text on the current screen (tips, descriptions, headings).</summary>
        public static void ReadScreen()
        {
            var parts = new List<string>();
            foreach (MonoBehaviour ui in current)
            {
                if (ui == null) continue;
                foreach (TMPro.TMP_Text t in ui.GetComponentsInChildren<TMPro.TMP_Text>(false))
                {
                    if (!t.enabled || t.GetComponentInParent<Selectable>() != null || t.name == "VersionLabel" || InPreviewPrinter(t.transform)) continue;
                    // Line breaks kept as pauses: lists such as the Patrons names are one text, a name per line.
                    string s = TextUtil.CleanLines(t.text);
                    if (TextUtil.IsReadable(s) && !parts.Contains(s)) parts.Add(s);
                }
            }
            Speech.Say(parts.Count > 0 ? TextUtil.Sentences(parts) : Strings.Get("noText"), interrupt: true);
        }

        /// <summary>Options' text-speed preview: sample story text shown on every tab, not part of the menu.</summary>
        static bool InPreviewPrinter(Transform t)
        {
            for (; t != null; t = t.parent)
                if (t.name == "PreviewPrinter") return true;
            return false;
        }

        static bool IsNotScreen(MonoBehaviour mb) => NotScreens.Any(n => mb.name.Contains(n));

        /// <summary>True for a control on a menu that's hidden (Naninovel hides menus without deactivating them).</summary>
        static bool InHiddenUI(Transform t)
        {
            var ui = t.GetComponentInParent<IManagedUI>();
            return ui != null && !ui.Visible;
        }

        static bool InCurrentScreen(Transform t) => current.Any(ui => ui != null && t.IsChildOf(ui.transform));

        /// <summary>
        /// The current screen's usable controls in the order the game's layout lists them
        /// (screen positions are unreliable while panels slide in).
        /// </summary>
        static List<Selectable> Controls()
        {
            var result = new List<Selectable>();
            foreach (MonoBehaviour ui in current)
            {
                if (ui == null) continue;
                result.AddRange(ui.GetComponentsInChildren<Selectable>(false)
                    .Where(s => s.IsInteractable() && s.navigation.mode != Navigation.Mode.None && !InDropdownList(s.transform)
                        // Scroll bars only move the view; they aren't settings.
                        && !(s is Scrollbar)
                        // The confirmation's invisible full-screen "close" ribbon; its question is read as the intro.
                        && s.name != "CloseRibbon"));
            }
            return result;
        }

        internal static bool InDropdownList(Transform t)
        {
            for (; t != null; t = t.parent)
                if (t.name == "Dropdown List") return true;
            return false;
        }

        /// <summary>Next/previous control for Up/Down, or null to leave Unity's choice alone.</summary>
        internal static Selectable Step(Selectable from, int direction)
        {
            if (current.Count == 0 || from == null || !InCurrentScreen(from.transform) || InDropdownList(from.transform)) return null;
            List<Selectable> controls = Controls();
            int i = controls.IndexOf(from);
            if (i < 0) return controls.FirstOrDefault();
            int next = i + direction;
            // Stop at the ends rather than wrapping, so the first and last items are easy to find.
            return next >= 0 && next < controls.Count ? controls[next] : from;
        }
    }

    /// <summary>
    /// Naninovel moves focus to a screen's default button on the first arrow press
    /// (ScriptableUIBehaviour.HandleNavigationFocus, Naninovel/ScriptableUIBehaviour.cs:366). Once we've
    /// given the screen focus ourselves, that would undo the player's first move, so skip it.
    /// </summary>
    [HarmonyPatch(typeof(ScriptableUIBehaviour), "HandleNavigationFocus")]
    internal static class NavigationFocusPatch
    {
        static bool Prefix() => !ScreenFocus.OwnsFocus;
    }

    /// <summary>
    /// Escape both closes an open drop-down list (Unity) and fires the game's own "go back" action
    /// (MihoKitBindableUIActionLabeledButton.invokeButtonPress, returnToSuperview), which closed the
    /// whole Options screen. While a list is open, let Escape close just the list.
    /// </summary>
    [HarmonyPatch(typeof(MihoKitBindableUIActionLabeledButton), "invokeButtonPress")]
    internal static class BackActionPatch
    {
        static bool Prefix(MihoKitBindableUIActionLabeledButton __instance) =>
            !(ScreenFocus.DropdownWasOpen && __instance.inputActionBindingName == "returnToSuperview");
    }

    [HarmonyPatch(typeof(Selectable), nameof(Selectable.FindSelectableOnDown))]
    internal static class FindDownPatch
    {
        static void Postfix(Selectable __instance, ref Selectable __result)
        {
            Selectable s = ScreenFocus.Step(__instance, +1);
            if (s != null) __result = s;
        }
    }

    [HarmonyPatch(typeof(Selectable), nameof(Selectable.FindSelectableOnUp))]
    internal static class FindUpPatch
    {
        static void Postfix(Selectable __instance, ref Selectable __result)
        {
            Selectable s = ScreenFocus.Step(__instance, -1);
            if (s != null) __result = s;
        }
    }

    /// <summary>Left/Right keep Unity's spatial choice, but never into a hidden menu behind the screen.</summary>
    [HarmonyPatch(typeof(Selectable), nameof(Selectable.FindSelectableOnLeft))]
    internal static class FindLeftPatch
    {
        static void Postfix(Selectable __instance, ref Selectable __result)
        {
            if (ScreenFocus.LeavesScreen(__instance, __result) || __result is Scrollbar || (__result != null && __result.name == "CloseRibbon")) __result = null;
        }
    }

    [HarmonyPatch(typeof(Selectable), nameof(Selectable.FindSelectableOnRight))]
    internal static class FindRightPatch
    {
        static void Postfix(Selectable __instance, ref Selectable __result)
        {
            if (ScreenFocus.LeavesScreen(__instance, __result) || __result is Scrollbar || (__result != null && __result.name == "CloseRibbon")) __result = null;
        }
    }
}
