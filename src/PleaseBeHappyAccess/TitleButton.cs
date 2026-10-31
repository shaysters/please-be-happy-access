using System;
using Naninovel;
using Naninovel.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleaseBeHappyAccess
{
    /// <summary>
    /// Adds an "Accessibility" button to the title menu, after Help, that opens the mod settings.
    /// It's a copy of the Help button with the copy's own click handler and translation removed.
    /// </summary>
    internal static class TitleButton
    {
        const string Name = "PleaseBeHappyAccessButton";
        static bool added;
        static GameObject copy;
        static float nextTry;

        public static void Tick()
        {
            if (Time.realtimeSinceStartup < nextTry || !Engine.Initialized) return;
            nextTry = Time.realtimeSinceStartup + 1f;
            // The language can change in Options.
            if (copy != null) SetLabel(copy);
            if (added) return;
            try
            {
                IUIManager ui = Engine.GetService<IUIManager>();
                if (ui == null) return;
                foreach (IManagedUI managed in ui.GetManagedUIs())
                {
                    if (!(managed is MonoBehaviour mb) || mb.name != "PBHTitleUI") continue;
                    Transform help = mb.transform.Find("ButtonsPanel/HelpButton");
                    if (help == null) return;
                    Add(help);
                    added = true;
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Error("TitleButton: " + e);
                added = true; // don't keep failing every second
            }
        }

        static void Add(Transform help)
        {
            if (help.parent.Find(Name) != null) return;
            copy = UnityEngine.Object.Instantiate(help.gameObject, help.parent);
            copy.name = Name;
            copy.transform.SetSiblingIndex(help.GetSiblingIndex() + 1);
            // The Help button's own behaviour (opening Help) and anything that rewrites its label.
            foreach (MonoBehaviour c in copy.GetComponentsInChildren<MonoBehaviour>(true))
                if (c is ScriptableUIBehaviour || c.GetType().Name.Contains("Locali"))
                    UnityEngine.Object.DestroyImmediate(c);
            Button button = copy.GetComponent<Button>();
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(ModMenu.Show);
            SetLabel(copy);
            Log.Info("Added the Accessibility button to the title menu.");
        }

        /// <summary>Keeps the label in the game's language.</summary>
        static void SetLabel(GameObject button)
        {
            foreach (TMP_Text t in button.GetComponentsInChildren<TMP_Text>(true))
            {
                t.text = Strings.Get("accessibilityButton");
                // Longer than the game's labels: keep it on one line (they're right-aligned, so it grows leftwards).
                t.enableWordWrapping = false;
                t.overflowMode = TextOverflowModes.Overflow;
            }
        }
    }
}
