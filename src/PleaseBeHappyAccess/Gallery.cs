using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PleaseBeHappyAccess
{
    /// <summary>
    /// Extras, Gallery: opening an unlocked picture reads its title and describes it, and each page turn
    /// ([ and ], or the arrow buttons) says which page it is and describes the new picture.
    ///
    /// The gallery shows each illustration as one finished picture, named differently from the story's
    /// layered images. Most pictures are described with the story's descriptions of their layers (listed
    /// below); pictures that only appear here have their own "gallery/&lt;name&gt;" descriptions.
    /// </summary>
    internal static class Gallery
    {
        /// <summary>Gallery picture name → the story descriptions that make it up, background first.</summary>
        static readonly Dictionary<string, string[]> Layers = new Dictionary<string, string[]>
        {
            ["seonbi composite"] = new[] { "bg/cg_bg/seonbiforest", "bg/cg_fg/seonbi" },
            ["hello miho composite"] = new[] { "bg/cg_fg/hellomiho" },
            ["hello sky composite"] = new[] { "bg/cg_fg/helloskymiho" },
            ["hello sky human composite"] = new[] { "bg/cg_fg/helloskymiho2" },
            ["crowd composite"] = new[] { "bg/crowd/crowd2" },
            ["julietintro1"] = new[] { "bg/julietintro/Juliet>Notice" },
            ["julietintro2"] = new[] { "bg/julietintro/Juliet>Hand" },
            ["aspenbarista 1"] = new[] { "bg/cg_cutin/C1>aspenbarista1,other-" },
            ["aspenbarista 2"] = new[] { "bg/cg_cutin/C1>aspenbarista2,other-" },
            ["mihosky"] = new[] { "bg/cg_cutin/C1>mihosky,other-" },
            ["AspenSpillComposite1"] = new[] { "bg/aspenspill/Miho>Gape,Drink>Whoosh,Aspen>Slip" },
            ["AspenSpillComposite2"] = new[] { "bg/aspenspill/Miho>Unimpressed,Drink>Splat,Aspen>Worry" },
            ["julietexp2"] = new[] { "bg/cg_cutin/C1>julietexp2,other-" },
            ["JulietWalkComposite"] = new[] { "bg/cg_bg/JulietWalkBG", "bg/cg_fg/JulietWalkJuliet", "bg/cg_fg2/JulietWalkMiho" },
            ["goodmorning"] = new[] { "bg/cg_bg/goodmorning" },
            ["AspenWorking"] = new[] { "bg/cg_bg/AspenWorking" },
            ["GroupHug"] = new[] { "bg/cg_bg/GroupHug" },
            ["SilverIntro"] = new[] { "bg/cg_bg/SilverIntroBG", "bg/cg_fg/SilverIntroSilver" },
            ["Eavesdrop1"] = new[] { "bg/cg_bg/Eavesdrop1" },
            ["Eavesdrop2"] = new[] { "bg/cg_bg/Eavesdrop2" },
            ["SilverFarewell"] = new[] { "bg/cg_fg/SilverFarewellSilver" },
            ["LenaIntro"] = new[] { "bg/cg_bg/LenaIntroBG", "bg/cg_fg/LenaIntroLena" },
            ["LenaFinale"] = new[] { "bg/cg_fg2/LenaFinaleChars", "bg/cg_fg3/LenaFinaleFG" },
            ["MrKimIntro"] = new[] { "bg/cg_bg/MrKimIntroBG", "bg/cg_fg/MrKimIntroKim", "bg/cg_fg2/MrKimIntroFish", "bg/cg_fg3/MrKimIntroFG" },
            ["MrKimFinale"] = new[] { "bg/cg_bg/MrKimFinale" },
            ["TommyIntro"] = new[] { "bg/cg_bg/TommyIntroBG", "bg/cg_fg/TommyIntroTommy", "bg/cg_fg2/TommyIntroFG" },
            ["TommyFinale"] = new[] { "bg/cg_fg/TommyFinaleChara", "bg/cg_fg2/TommyFinaleGerbie" },
            ["AspenConfession1"] = new[] { "bg/cg_bg/AspenConfessionBG", "bg/cg_fg/AspenConfession1a" },
            ["AspenConfession2"] = new[] { "bg/cg_bg/AspenConfessionBG", "bg/cg_fg/AspenConfession1b" },
            ["AspenConfession3"] = new[] { "bg/cg_bg/AspenConfessionBG", "bg/cg_fg/AspenConfession1c" },
            ["AspenConfession4"] = new[] { "bg/cg_bg/AspenConfessionBG", "bg/cg_fg/AspenConfession2" },
            ["AspenKiss1"] = new[] { "bg/cg_bg/AspenKissBG", "bg/cg_fg/AspenKissLook" },
            ["AspenKiss2"] = new[] { "bg/cg_bg/AspenKissBG", "bg/cg_fg/AspenKissKiss" },
            ["AspenGlowworm1"] = new[] { "bg/cg_bg/AspenGlowwormBG", "bg/cg_fg/AspenGlowworm1", "bg/cg_fg2/AspenGlowwormFG" },
            ["AspenGlowworm2"] = new[] { "bg/cg_bg/AspenGlowwormBG", "bg/cg_fg/AspenGlowworm2", "bg/cg_fg2/AspenGlowwormFG" },
            ["AspenGlowworm3"] = new[] { "bg/cg_bg/AspenGlowwormBG", "bg/cg_fg/AspenGlowworm3", "bg/cg_fg2/AspenGlowwormFG" },
            ["AspenGlowworm4"] = new[] { "bg/cg_bg/AspenGlowwormBG", "bg/cg_fg/AspenGlowworm4", "bg/cg_fg2/AspenGlowwormFG" },
            ["AspenSleepover1"] = new[] { "bg/cg_bg/AspenSleepover1" },
            ["AspenSleepover2"] = new[] { "bg/cg_bg/AspenSleepover2" },
            ["AspenSleepover3"] = new[] { "bg/cg_bg/AspenSleepover3" },
            ["AspenScatter"] = new[] { "bg/cg_bg/AspenScatterBG", "bg/cg_fg/AspenScatterAspen", "bg/cg_fg2/AspenScatterFG" },
            ["AspenComfy1"] = new[] { "bg/cg_bg/AspenComfyBG", "bg/cg_fg/AspenComfy1" },
            ["AspenComfy2"] = new[] { "bg/cg_bg/AspenComfyBG", "bg/cg_fg/AspenComfy2" },
            ["AspenWild1"] = new[] { "bg/cg_bg/AspenWildBG", "bg/cg_fg/AspenWild1" },
            ["AspenWild2"] = new[] { "bg/cg_bg/AspenWildBG", "bg/cg_fg/AspenWild2" },
            ["AspenWild3"] = new[] { "bg/cg_bg/AspenWildBG", "bg/cg_fg/AspenWild3" },
            ["AspenWild4"] = new[] { "bg/cg_bg/AspenWildBG", "bg/cg_fg/AspenWild4" },
            ["JulietGrave"] = new[] { "bg/cg_bg/JulietGrave" },
            ["JulietRunAnimal"] = new[] { "bg/cg_bg/JulietRunAnimal" },
            ["JulietDance1"] = new[] { "bg/cg_bg/JulietDanceBG", "bg/cg_fg/JulietDanceDance" },
            ["JulietDance2"] = new[] { "bg/cg_bg/JulietDanceBG", "bg/cg_fg/JulietDanceKiss" },
            ["JulietBlanket"] = new[] { "bg/cg_fg/JulietBlanketFG" },
            ["JulietKiss1"] = new[] { "bg/cg_bg/JulietKissBG", "bg/cg_fg/JulietKissLook" },
            ["JulietKiss2"] = new[] { "bg/cg_bg/JulietKissBG", "bg/cg_fg/JulietKissKiss" },
            ["JulietGift"] = new[] { "bg/cg_bg/JulietGiftBG", "bg/cg_fg/JulietGiftChara" },
            ["JulietComfy1"] = new[] { "bg/cg_bg/JulietComfyBG", "bg/cg_fg/JulietComfy1" },
            ["JulietComfy2"] = new[] { "bg/cg_bg/JulietComfyBG", "bg/cg_fg/JulietComfy2" },
            ["JulietComfy3"] = new[] { "bg/cg_bg/JulietComfyBG", "bg/cg_fg/JulietComfy3" },
            ["JulietWild1"] = new[] { "bg/cg_bg/JulietWildBG", "bg/cg_fg/JulietWild1" },
            ["JulietWild2"] = new[] { "bg/cg_bg/JulietWildBG", "bg/cg_fg/JulietWild2" },
            ["JulietWild3"] = new[] { "bg/cg_bg/JulietWildBG", "bg/cg_fg/JulietWild3" },
        };

        static z04CGManager manager;
        static Texture shown;
        static GameObject opener;

        /// <summary>True while a gallery picture is open.</summary>
        public static bool Showing => manager != null && manager.isShowingCG;

        public static void Tick()
        {
            if (manager == null)
            {
                // Created with the Extras screen; look again only every couple of seconds until it exists.
                if (Time.frameCount % 120 != 0) return;
                manager = Resources.FindObjectsOfTypeAll<z04CGManager>().FirstOrDefault(m => m.gameObject.scene.IsValid());
                if (manager == null) return;
            }
            EventSystem es = EventSystem.current;
            if (!manager.isShowingCG)
            {
                // Closed: back to the gallery entry that opened the picture, and read it.
                if (shown != null)
                {
                    if (opener != null && opener.activeInHierarchy && es != null) es.SetSelectedGameObject(opener);
                    FocusReader.Reannounce();
                    opener = null;
                }
                shown = null;
                return;
            }
            if (shown == null && opener == null && es != null) opener = es.currentSelectedGameObject;
            Texture texture = Preview()?.texture;
            if (texture == null || texture == shown) return;
            // The game puts the picture in place at the end of the frame; until then the last one opened shows.
            if (!Pages().Contains(texture)) return;
            // Opening a picture with several pages first shows the last one added for a frame; wait until it settles.
            bool opening = shown == null;
            shown = texture;
            Speech.Say(Describe(includeTitle: opening), interrupt: true);
        }

        /// <summary>d and r while a picture is open: the title, page and description again.</summary>
        public static void Repeat() => Speech.Say(Describe(includeTitle: true), interrupt: true);

        /// <summary>
        /// Escape while a picture is open should close just the picture. The game means to do that
        /// (MihoKitMainMenuSidebarReturnButton checks for an open picture) but looks for the gallery by a
        /// path that doesn't exist, so Escape also left Extras for the title screen.
        /// </summary>
        [HarmonyPatch(typeof(MihoKitMainMenuSidebarReturnButton), "invokeButtonPress")]
        internal static class ReturnPatch
        {
            static bool Prefix() => !Showing;
        }

        static List<Texture> Pages() => Traverse.Create(manager).Field("previewList").GetValue<List<Texture>>() ?? new List<Texture>();

        static RawImage Preview() => Traverse.Create(manager).Field("preview").GetValue<RawImage>();

        static string Describe(bool includeTitle)
        {
            var parts = new List<string>();
            List<Texture> pages = Pages();
            Texture texture = Preview()?.texture;
            if (includeTitle)
            {
                TMP_Text title = Traverse.Create(manager).Field("text").GetValue<TextMeshProUGUI>();
                string s = title != null ? TextUtil.Clean(title.text) : "";
                if (TextUtil.IsReadable(s)) parts.Add(s);
            }
            if (pages.Count > 1 && texture != null)
                parts.Add(string.Format(Strings.Get("pageOf"), pages.IndexOf(texture) + 1, pages.Count));
            string description = texture != null ? DescriptionOf(texture.name) : null;
            parts.Add(description ?? Strings.Get("noImage"));
            return TextUtil.Sentences(parts);
        }

        static string DescriptionOf(string name)
        {
            // The logos come in one picture per language: galleryGameLogo-en, galleryGameLogo-ja...
            Match language = Regex.Match(name, "-(en|ja|ko|zh-CN|zh-TW)$");
            string own = Descriptions.Get("gallery/" + (language.Success ? name.Substring(0, language.Index) : name));
            if (own != null && language.Success)
                return TextUtil.Sentences(new[] { own, string.Format(Strings.Get("logoLanguage"), Strings.Get("language." + language.Groups[1].Value)) });
            if (own != null) return own;
            if (!Layers.TryGetValue(name, out string[] keys)) return null;
            var found = keys.Select(Descriptions.Get).Where(s => s != null).ToList();
            return found.Count > 0 ? TextUtil.Sentences(found) : null;
        }
    }
}
