using BepInEx.Logging;

namespace PleaseBeHappyAccess
{
    internal static class Log
    {
        static ManualLogSource source;

        public static void Init(ManualLogSource s) => source = s;

        public static void Info(string msg) => source?.LogInfo(msg);
        public static void Error(string msg) => source?.LogError(msg);

        /// <summary>Every spoken line is logged with a fixed prefix so tests can check what was announced.</summary>
        public static void Speech(string msg) => source?.LogMessage("SAY: " + msg);
    }
}
