using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PleaseBeHappyAccess
{
    /// <summary>
    /// Announces whatever the game's own keyboard/controller navigation selects.
    /// Naninovel moves EventSystem selection on arrow keys (ScriptableUIBehaviour.HandleNavigationFocus,
    /// Naninovel/ScriptableUIBehaviour.cs:366) — we only read it, we don't move it.
    /// </summary>
    internal static class FocusReader
    {
        static GameObject lastSelected;
        static string lastValue;

        /// <summary>Set when a screen introduction was just spoken, so the first item queues after it.</summary>
        public static bool QueueNext;

        /// <summary>Reads the selected control again on the next tick (after the mod settings close).</summary>
        public static void Reannounce()
        {
            lastSelected = null;
            QueueNext = true;
        }

        public static void Tick()
        {
            EventSystem es = EventSystem.current;
            GameObject selected = es != null ? es.currentSelectedGameObject : null;
            if (selected != lastSelected)
            {
                lastSelected = selected;
                lastValue = ValueOf(selected);
                if (selected != null)
                {
                    Speech.Say(Describe(selected), interrupt: !QueueNext);
                    QueueNext = false;
                }
                return;
            }
            // Sliders, toggles and drop-downs change value while staying selected.
            if (selected != null)
            {
                string value = ValueOf(selected);
                if (value != lastValue)
                {
                    lastValue = value;
                    // A tab switching on is announced by ScreenFocus ("TEXT tab."), not as "checked".
                    if (selected.GetComponent<Toggle>() is Toggle tab && tab.group != null && IsTabRow(tab)) return;
                    if (!string.IsNullOrEmpty(value)) Speech.Say(value, interrupt: true);
                }
            }
        }

        public static string Describe(GameObject go)
        {
            var sb = new StringBuilder();
            // Save/load slots show their number only as a picture; the game keeps it on the slot.
            if (go.GetComponent<Naninovel.UI.GameStateSlot>() is Naninovel.UI.GameStateSlot slot)
                sb.Append(string.Format(Strings.Get("slotN"), slot.SlotNumber)).Append(", ");
            // Backlog entries are focusable only through their "go back to this line" button;
            // read the entry's speaker and text instead of the icon's name.
            Transform message = BacklogMessageOf(go.transform);
            if (message != null)
            {
                string entry = string.Join(": ", message.GetComponentsInChildren<TMP_Text>(false)
                    .Where(t => t.enabled && t.GetComponentInParent<Selectable>() == null)
                    .Select(t => TextUtil.Clean(t.text))
                    .Where(TextUtil.IsReadable));
                // Sentences, so a line ending in a full stop doesn't become "silence., Enter to go back".
                return entry.Length > 0
                    ? TextUtil.Sentences(new[] { entry, Strings.Get("rollback") })
                    : Strings.Get("rollback");
            }
            Selectable sel = go.GetComponent<Selectable>();
            // Toggles in a group are either a row of tabs (Options' BASIC/TEXT/SOUND/VOICE) or a set of
            // choices for one setting (Music Cues: Enabled/Disabled).
            if (sel is Toggle toggle && toggle.group != null) return DescribeGroupToggle(toggle);
            // An open drop-down list: each entry is a choice, the ticked one is the current value.
            if (sel is Toggle item && ScreenFocus.InDropdownList(item.transform))
                return Fallback(TextsIn(go, includeHidden: false), go) + ", " + Strings.Get(item.isOn ? "chosen" : "notChosen");
            sb.Append(LabelOf(go));
            // Voice volume rows have a button that plays a sample line.
            if (sel is Button && go.name.Contains("Test")) sb.Append(", ").Append(Strings.Get("playSample"));
            string role = sel switch
            {
                Toggle _ => null,
                Slider _ => Strings.Get("slider"),
                TMP_Dropdown _ => Strings.Get("dropdown"),
                Dropdown _ => Strings.Get("dropdown"),
                Button _ => Strings.Get("button"),
                _ => null,
            };
            if (role != null) sb.Append(", ").Append(role);
            string value = ValueOf(go);
            if (!string.IsNullOrEmpty(value)) sb.Append(", ").Append(value);
            // The open tab of a tab row is a switched-on toggle the game greys out; call it selected, not unavailable.
            if (sel != null && !sel.interactable && !(sel is Toggle)) sb.Append(", ").Append(Strings.Get("unavailable"));
            return sb.ToString();
        }

        internal static bool IsTabRow(Toggle t) => t.transform.parent != null && t.transform.parent.name.Contains("Navigation");

        static string DescribeGroupToggle(Toggle t)
        {
            string label = TextsIn(t.gameObject, includeHidden: false);
            if (label.Length == 0) label = TextsIn(t.gameObject, includeHidden: true);
            label = Fallback(label, t.gameObject);
            if (IsTabRow(t))
                return label + ", " + Strings.Get("tab") + (t.isOn ? ", " + Strings.Get("selected") : "");
            // The chosen option is greyed out (unreachable), so say it from the others too.
            string caption = RowCaption(t.transform.parent);
            var sb = new StringBuilder();
            if (caption.Length > 0) sb.Append(caption).Append(": ");
            sb.Append(label).Append(", ").Append(Strings.Get(t.isOn ? "chosen" : "notChosen"));
            Toggle current = t.group.ActiveToggles().FirstOrDefault();
            if (!t.isOn && current != null)
            {
                string currentLabel = Fallback(TextsIn(current.gameObject, includeHidden: false), current.gameObject);
                sb.Append(", ").Append(string.Format(Strings.Get("currentlyX"), currentLabel));
            }
            return sb.ToString();
        }

        /// <summary>Caption of a settings row: plain text in the row or the row above it.</summary>
        static string RowCaption(Transform t)
        {
            for (int level = 0; level < 2 && t != null; level++, t = t.parent)
            {
                string s = string.Join(", ", t.GetComponentsInChildren<TMP_Text>(false)
                    .Where(x => x.enabled && x.GetComponentInParent<Selectable>() == null)
                    .Select(x => TextUtil.Clean(x.text))
                    .Where(TextUtil.IsReadable)
                    .Distinct());
                if (s.Length > 0) return s;
            }
            return "";
        }

        internal static Transform BacklogMessageOf(Transform t)
        {
            for (; t != null; t = t.parent)
                if (t.name.Contains("BacklogMessage")) return t;
            return null;
        }

        static string ValueOf(GameObject go)
        {
            if (go == null) return null;
            switch (go.GetComponent<Selectable>())
            {
                case Toggle t when t.isOn && !t.interactable: return Strings.Get("selected");
                case Toggle t: return Strings.Get(t.isOn ? "checked" : "unchecked");
                case Slider s:
                    float range = s.maxValue - s.minValue;
                    return range > 0 ? Mathf.RoundToInt((s.value - s.minValue) / range * 100f) + "%" : s.value.ToString();
                case TMP_Dropdown d: return d.options.Count > d.value ? TextUtil.Clean(d.options[d.value].text) : null;
                case Dropdown d: return d.options.Count > d.value ? TextUtil.Clean(d.options[d.value].text) : null;
                default: return null;
            }
        }

        /// <summary>
        /// Visible text inside the control. Icon buttons like the in-game quick menu keep their
        /// label hidden until hovered, so fall back to hidden labels, then the object name.
        /// </summary>
        public static string LabelOf(GameObject go)
        {
            // A drop-down's children are its option list, so its name comes from the row caption.
            if (go.GetComponent<TMP_Dropdown>() != null || go.GetComponent<Dropdown>() != null)
                return Fallback(SiblingLabel(go), go);
            string label = TextsIn(go, includeHidden: false);
            if (label.Length == 0) label = TextsIn(go, includeHidden: true);
            if (label.Length == 0) label = SiblingLabel(go);
            return Fallback(label, go);
        }

        /// <summary>
        /// Settings rows put the caption beside the control, e.g. LocalePanel/LanguageLabel next to
        /// LocalePanel/LanguageDropdown — read text in the parent that isn't part of another control.
        /// </summary>
        static string SiblingLabel(GameObject go)
        {
            Transform parent = go.transform.parent;
            if (parent == null) return "";
            var parts = parent.GetComponentsInChildren<TMP_Text>(false)
                .Where(t => t.enabled && t.GetComponentInParent<Selectable>() == null)
                .Select(t => TextUtil.Clean(t.text))
                .Where(s => s.Length > 0)
                .Distinct();
            return string.Join(", ", parts);
        }

        // Map pins have no text; their object names are the place names ("Oriental Bay(Clone)").
        static string Fallback(string label, GameObject go) => label.Length > 0 ? label : go.name.Replace("(Clone)", "").Trim();

        static string TextsIn(GameObject go, bool includeHidden)
        {
            TMP_Text caption = go.GetComponent<TMP_Dropdown>()?.captionText;
            var parts = go.GetComponentsInChildren<TMP_Text>(includeHidden)
                .Where(t => (includeHidden || t.enabled) && t != caption)
                .Select(t => TextUtil.CleanLines(t.text))
                .Concat(go.GetComponentsInChildren<UnityEngine.UI.Text>(includeHidden)
                    .Where(t => includeHidden || t.enabled).Select(t => TextUtil.Clean(t.text)))
                .Where(s => s.Length > 0)
                .Distinct();
            return string.Join(", ", parts);
        }
    }
}
