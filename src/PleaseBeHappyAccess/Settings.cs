using UnityEngine;

namespace PleaseBeHappyAccess
{
    /// <summary>
    /// Mod settings. Changed in game through the mod settings menu (M, or Accessibility on the title menu), saved with Unity PlayerPrefs
    /// under "PleaseBeHappyAccess." keys — there is no file for the player to edit.
    /// </summary>
    internal static class Settings
    {
        public static readonly BoolSetting ReadVoicedLines = new BoolSetting("ReadVoicedLines", "settingReadVoiced", true);

        public static readonly BoolSetting CheckForUpdates = new BoolSetting("CheckForUpdates", "settingCheckUpdates", true);

        public static readonly BoolSetting DescribeScenes = new BoolSetting("DescribeScenes", "settingDescribeScenes", true);

        public static readonly BoolSetting DescribeCharacters = new BoolSetting("DescribeCharacters", "settingDescribeCharacters", true);

        public static readonly BoolSetting ReadPopups = new BoolSetting("ReadPopups", "settingReadPopups", true);

        public static readonly BoolSetting ReadSongs = new BoolSetting("ReadSongs", "settingReadSongs", true);

        public static readonly BoolSetting[] All = { ReadVoicedLines, DescribeScenes, DescribeCharacters, ReadSongs, ReadPopups, CheckForUpdates };
    }

    internal class BoolSetting
    {
        readonly string key;
        readonly bool fallback;

        /// <summary>Strings.cs key for the setting's spoken name.</summary>
        public readonly string NameKey;

        public BoolSetting(string key, string nameKey, bool fallback)
        {
            this.key = "PleaseBeHappyAccess." + key;
            NameKey = nameKey;
            this.fallback = fallback;
        }

        public bool Value
        {
            get => PlayerPrefs.GetInt(key, fallback ? 1 : 0) != 0;
            set
            {
                PlayerPrefs.SetInt(key, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        public string Spoken => Strings.Get(NameKey) + ", " + Strings.Get(Value ? "on" : "off");
    }
}
