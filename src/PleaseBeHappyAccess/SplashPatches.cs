using System;
using System.Linq;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace PleaseBeHappyAccess
{
    /// <summary>
    /// The start-up screens: the Studio Élan logo video, then the title's "press any button" screen.
    /// </summary>
    internal static class SplashPatches
    {
        /// <summary>
        /// The logo video starts playing (Assembly-CSharp/MelanieKitNamiToolsElanBootLogoHandler.cs,
        /// handleBootLogoInitialisationComplete).
        /// </summary>
        [HarmonyPatch(typeof(MelanieKitNamiToolsElanBootLogoHandler), "handleBootLogoInitialisationComplete")]
        static class BootLogoPatch
        {
            static void Postfix()
            {
                try
                {
                    Speech.Say(Descriptions.Get("screen/bootlogo") ?? "Studio Élan.", interrupt: false);
                }
                catch (Exception e)
                {
                    Log.Error("BootLogoPatch: " + e);
                }
            }
        }

        /// <summary>
        /// The title screen appears and waits for any key (Assembly-CSharp/MihoKitTitleScreenHandler.cs, main).
        /// Reads its picture and its own prompt text.
        /// </summary>
        [HarmonyPatch(typeof(MihoKitTitleScreenHandler), "main")]
        static class TitleScreenPatch
        {
            static void Postfix()
            {
                try
                {
                    // The "Press any button to start" prompt is read with the title menu (ScreenFocus intros);
                    // this object only holds a hidden joke prompt about the Meemo Plushie.
                    string picture = Descriptions.Get("screen/title");
                    if (picture != null) Speech.Say(picture, interrupt: false);
                }
                catch (Exception e)
                {
                    Log.Error("TitleScreenPatch: " + e);
                }
            }
        }
    }
}
