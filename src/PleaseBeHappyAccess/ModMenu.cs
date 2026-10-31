using System.Collections.Generic;
using Naninovel;
using UnityEngine.InputSystem;

namespace PleaseBeHappyAccess
{
    /// <summary>
    /// The mod's own settings list, opened with M or the title menu's Accessibility button. Up/Down move,
    /// Enter/Space/Left/Right change, Escape or M close. While it is open the game ignores the keyboard.
    /// </summary>
    internal static class ModMenu
    {
        public const Key OpenKey = Key.M;

        static bool open;
        static int index;
        static bool restorePending;
        static bool naninovelInputWas;
        static IInputManager pausedInput;
        static readonly List<InputAction> pausedActions = new List<InputAction>();

        public static bool Open => open;

        // The Enter that pressed the Accessibility button mustn't also switch the first setting.
        static int openedFrame = -1;

        public static void Tick()
        {
            Keyboard kb = Keyboard.current;
            if (kb == null) return;

            if (restorePending)
            {
                // Hand the keyboard back only once every key is up, so the key that closed
                // the menu (Escape) doesn't also reach the game.
                if (!kb.anyKey.isPressed) RestoreGameInput();
                return;
            }

            if (!open)
            {
                if (kb[OpenKey].wasPressedThisFrame) Show();
                return;
            }

            if (UnityEngine.Time.frameCount == openedFrame) return;
            BoolSetting[] items = Settings.All;
            if (kb[OpenKey].wasPressedThisFrame || kb.escapeKey.wasPressedThisFrame)
            {
                Close();
            }
            else if (kb.downArrowKey.wasPressedThisFrame)
            {
                if (index < items.Length - 1) index++;
                SpeakItem();
            }
            else if (kb.upArrowKey.wasPressedThisFrame)
            {
                if (index > 0) index--;
                SpeakItem();
            }
            else if (kb.homeKey.wasPressedThisFrame)
            {
                index = 0;
                SpeakItem();
            }
            else if (kb.endKey.wasPressedThisFrame)
            {
                index = items.Length - 1;
                SpeakItem();
            }
            else if (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame
                     || kb.leftArrowKey.wasPressedThisFrame || kb.rightArrowKey.wasPressedThisFrame)
            {
                items[index].Value = !items[index].Value;
                Speech.Say(items[index].Spoken, interrupt: true);
            }
        }

        /// <summary>Opens the list (M, or the title menu's Accessibility button).</summary>
        public static void Show()
        {
            if (open) return;
            open = true;
            openedFrame = UnityEngine.Time.frameCount;
            index = 0;
            BlockGameInput();
            Speech.Say(Strings.Get("modSettings") + ". " + ItemText(), interrupt: true);
        }

        static void Close()
        {
            open = false;
            restorePending = true;
            Speech.Say(Strings.Get("modSettingsClosed"), interrupt: true);
            // Back on the title menu: say where focus is.
            if (ScreenFocus.HasScreen) FocusReader.Reannounce();
        }

        static void SpeakItem() => Speech.Say(ItemText(), interrupt: true);

        static string ItemText() =>
            Settings.All[index].Spoken + ", " + string.Format(Strings.Get("itemOf"), index + 1, Settings.All.Length);

        static void BlockGameInput()
        {
            // The game's own key actions (menus, Escape to go back) and Unity UI navigation.
            pausedActions.Clear();
            foreach (InputAction action in InputSystem.ListEnabledActions())
            {
                pausedActions.Add(action);
                action.Disable();
            }
            // Naninovel's story keys (Enter to continue, Ctrl to skip...).
            IInputManager input = Engine.Initialized ? Engine.GetService<IInputManager>() : null;
            pausedInput = input;
            if (input != null)
            {
                naninovelInputWas = input.ProcessInput;
                input.ProcessInput = false;
            }
        }

        static void RestoreGameInput()
        {
            restorePending = false;
            foreach (InputAction action in pausedActions) action.Enable();
            pausedActions.Clear();
            // Only put back what we changed: if the engine started while the menu was open, leave it be.
            if (pausedInput != null) pausedInput.ProcessInput = naninovelInputWas;
            pausedInput = null;
        }
    }
}
