using System.Collections.Generic;
using HarmonyLib;
using Naninovel;
using Naninovel.UI;
using UnityEngine;

namespace PleaseBeHappyAccess
{
    /// <summary>
    /// The icons beside each place on the location map: who is there, whether it's the main story or a
    /// character's story starting or continuing, and whether going there uses up time.
    ///
    /// The story script gives each place a variable listing its icons, such as mbHataitai =
    /// "true,lena,book,clock" (map script), and the pin shows the matching pictures (z04MapIconManager).
    /// Pins hold the variable's name; the sidebar's list of places copies the same list from its pin
    /// (z04MapButtonFinder.variableInput). The game has no text for the icons, so they're named here.
    /// </summary>
    internal static class MapIcons
    {
        /// <summary>Icon name → character id; the "who" icons are for someone whose name you don't know yet.</summary>
        static readonly Dictionary<string, string> People = new Dictionary<string, string>
        {
            ["miho"] = "Miho", ["juliet"] = "Juliet", ["aspen"] = "Aspen", ["tommy"] = "Tommy",
            ["gwen"] = "Gwen", ["lena"] = "Lena", ["mrkim"] = "MrKim",
        };

        static readonly Dictionary<string, string> Markers = new Dictionary<string, string>
        {
            ["main"] = "mapMain", ["book"] = "mapNewStory", ["!"] = "mapStoryContinues",
        };

        /// <summary>"Lena, a new story, takes time", or null for a control that isn't a map place.</summary>
        public static string Describe(GameObject go)
        {
            string input = null;
            if (go.GetComponent<z04MapButtonFinder>() is z04MapButtonFinder finder)
                input = Traverse.Create(finder).Field("variableInput").GetValue<string>();
            else if (go.GetComponent<z04MapIconManager>() != null || go.GetComponentInChildren<z04MapIconManager>(true) != null)
            {
                string variable = go.GetComponent<CustomVariableTrigger>()?.CustomVariableName;
                if (!string.IsNullOrEmpty(variable)) input = Engine.GetService<ICustomVariableManager>()?.GetVariableValue(variable);
            }
            if (string.IsNullOrEmpty(input)) return null;

            var people = new List<string>();
            var markers = new List<string>();
            bool time = false;
            foreach (string raw in input.Split(','))
            {
                string icon = raw.Trim();
                if (People.TryGetValue(icon, out string id)) people.Add(SceneWatcher.Name(id));
                else if (icon.EndsWith("who")) people.Add(Strings.Get("someoneNew"));
                else if (Markers.TryGetValue(icon, out string key)) markers.Add(Strings.Get(key));
                else if (icon == "clock") time = true;
            }
            var parts = new List<string>(people);
            parts.AddRange(markers);
            if (time) parts.Add(Strings.Get("mapTakesTime"));
            return parts.Count > 0 ? string.Join(", ", parts) : null;
        }
    }
}
