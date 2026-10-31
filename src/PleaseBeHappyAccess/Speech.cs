using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PleaseBeHappyAccess
{
    /// <summary>
    /// Speech and braille through Prism (prism.dll, 64-bit, v0.18.3).
    /// prism_backend_output sends to both speech and braille when the screen reader supports it.
    /// </summary>
    internal static class Speech
    {
        const int PRISM_OK = 0;
        const int PRISM_ERROR_ALREADY_INITIALIZED = 15;

        [StructLayout(LayoutKind.Sequential)]
        struct PrismConfig
        {
            public byte version;
            public IntPtr registry;
            public IntPtr availabilityCallback;
            public IntPtr availabilityUserdata;
            public uint availabilityPollIntervalMs;
            public uint availabilityDebounceSamples;
            public uint availabilityBackoffMaxMs;
            public byte availabilityAutoPowerManage;
            public IntPtr availabilityBaselineCallback;
        }

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern IntPtr LoadLibraryW(string path);

        [DllImport("prism", CallingConvention = CallingConvention.Cdecl)]
        static extern PrismConfig prism_config_init();

        [DllImport("prism", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr prism_init(ref PrismConfig cfg);

        [DllImport("prism", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr prism_registry_acquire_best(IntPtr ctx);

        [DllImport("prism", CallingConvention = CallingConvention.Cdecl)]
        static extern int prism_backend_initialize(IntPtr backend);

        [DllImport("prism", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr prism_backend_name(IntPtr backend);

        [DllImport("prism", CallingConvention = CallingConvention.Cdecl)]
        static extern int prism_backend_output(IntPtr backend, byte[] text, [MarshalAs(UnmanagedType.U1)] bool interrupt);

        [DllImport("prism", CallingConvention = CallingConvention.Cdecl)]
        static extern int prism_backend_speak(IntPtr backend, byte[] text, [MarshalAs(UnmanagedType.U1)] bool interrupt);

        [DllImport("prism", CallingConvention = CallingConvention.Cdecl)]
        static extern int prism_backend_braille(IntPtr backend, byte[] text);

        [DllImport("prism", CallingConvention = CallingConvention.Cdecl)]
        static extern int prism_backend_stop(IntPtr backend);

        static IntPtr context;
        static IntPtr backend;
        static PrismConfig config;

        public static bool Ready => backend != IntPtr.Zero;

        /// <summary>Set by tests: speech is logged but not sent to the screen reader.</summary>
        public static bool Muted;

        /// <summary>Set after a scene description, so the next dialogue line queues after it instead of cutting it off.</summary>
        public static bool KeepNextLine;
        public static string BackendName { get; private set; } = "none";

        public static void Initialize(string pluginDir)
        {
            try
            {
                // Load our copy explicitly so DllImport("prism") resolves to it.
                if (LoadLibraryW(Path.Combine(pluginDir, "prism.dll")) == IntPtr.Zero)
                {
                    Log.Error($"Could not load prism.dll from {pluginDir} (Win32 error {Marshal.GetLastWin32Error()}).");
                    return;
                }
                config = prism_config_init();
                context = prism_init(ref config);
                if (context == IntPtr.Zero)
                {
                    Log.Error("prism_init failed.");
                    return;
                }
                IntPtr best = prism_registry_acquire_best(context);
                if (best == IntPtr.Zero)
                {
                    Log.Error("Prism found no speech backend.");
                    return;
                }
                int err = prism_backend_initialize(best);
                if (err != PRISM_OK && err != PRISM_ERROR_ALREADY_INITIALIZED)
                {
                    Log.Error($"prism_backend_initialize returned {err}.");
                    return;
                }
                backend = best;
                BackendName = Marshal.PtrToStringAnsi(prism_backend_name(backend)) ?? "Prism";
                Log.Info($"Prism ready, backend: {BackendName}");
            }
            catch (Exception e)
            {
                Log.Error("Prism setup failed: " + e);
            }
        }

        /// <summary>Speak and braille the text. interrupt cuts off whatever is being spoken.</summary>
        // A line that is only "..." (or "……", "・・・"), on its own or after "Miho: ". Screen readers say nothing for it.
        static readonly System.Text.RegularExpressions.Regex OnlyEllipsis =
            new System.Text.RegularExpressions.Regex(@"(^|: |：)(?:(?:\.{2,}|…+|・{2,})\s*)+(?=[?!？！]*\s*$)");

        /// <summary>Says "Ellipsis" for a silent line ("Miho: ..." becomes "Miho: Ellipsis").</summary>
        internal static string SayEllipsis(string text) =>
            OnlyEllipsis.Replace(text, m =>
            {
                // "… …" is a longer pause than "…": one word per ellipsis.
                int count = System.Text.RegularExpressions.Regex.Matches(m.Value, @"\.{2,}|…+|・{2,}").Count;
                return m.Groups[1].Value + string.Join(" ", Enumerable.Repeat(Strings.Get("ellipsis"), Math.Max(1, count)));
            });

        public static void Say(string text, bool interrupt = false)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            text = SayEllipsis(text);
            Log.Speech(text);
            if (!Ready || Muted) return;
            byte[] data = Utf8(text);
            int err = prism_backend_output(backend, data, interrupt);
            if (err != PRISM_OK)
            {
                prism_backend_speak(backend, data, interrupt);
                prism_backend_braille(backend, data);
            }
        }

        public static void Stop()
        {
            if (Ready) prism_backend_stop(backend);
        }

        static byte[] Utf8(string s)
        {
            byte[] raw = Encoding.UTF8.GetBytes(s);
            byte[] z = new byte[raw.Length + 1];
            Buffer.BlockCopy(raw, 0, z, 0, raw.Length);
            return z;
        }
    }
}
