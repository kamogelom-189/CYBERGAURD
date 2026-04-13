using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CyberBot;

/// <summary>
/// Handles the audio greeting on startup.
/// Plays assets/greetings.wav when available; gracefully falls back on any platform
/// where audio is unavailable (CI runners, headless servers, macOS/Linux without
/// a compatible audio back-end). Accepts an optional absolute path override.
/// </summary>
public static class VoiceGreeting
{
    private const string WavRelativePath = "assets/greetings.wav";

    /// <summary>
    /// Attempts to play the greeting WAV file.
    /// If <paramref name="overridePath"/> is provided and exists it will be used.
    /// Returns true if playback was attempted, false if skipped or failed.
    /// </summary>
    public static bool Play(string? overridePath = null)
    {
        // 1) If caller provided an explicit path, try that first.
        if (!string.IsNullOrWhiteSpace(overridePath))
        {
            try
            {
                var candidate = overridePath.Trim().Trim('"');
                if (File.Exists(candidate))
                {
                    PlayWav(candidate);
                    return true;
                }
            }
            catch
            {
                // fall through to other attempts
            }
        }

        // 2) Try the shipped assets path relative to the app base directory.
        string wavPath = Path.Combine(AppContext.BaseDirectory, WavRelativePath);

        if (!File.Exists(wavPath))
        {
            // Wav not shipped – skip silently (CI / first run)
            return false;
        }

        try
        {
            PlayWav(wavPath);
            return true;
        }
        catch
        {
            // Audio subsystem unavailable – not fatal
            return false;
        }
    }

    // ── Platform dispatch ─────────────────────────────────────────────────────

    private static void PlayWav(string path)
    {
        if (OperatingSystem.IsWindows())
        {
            PlayWindows(path);
        }
        else if (OperatingSystem.IsMacOS())
        {
            RunProcess("afplay", $"\"{path}\"");
        }
        else
        {
            // Linux: try aplay, then paplay
            if (!TryRunProcess("aplay", $"\"{path}\""))
                TryRunProcess("paplay", $"\"{path}\"");
        }
    }

    // Windows: attempt managed SoundPlayer via reflection, fallback to native winmm PlaySound.
    private static void PlayWindows(string path)
    {
        // Try reflection first (no compile-time dependency on System.Windows.Extensions)
        try
        {
            var assembly = System.Reflection.Assembly.Load("System.Windows.Extensions");
            var type = assembly.GetType("System.Media.SoundPlayer");
            if (type is not null)
            {
                using var player = (IDisposable)Activator.CreateInstance(type, path)!;
                type.GetMethod("PlaySync")!.Invoke(player, null);
                return;
            }
        }
        catch
        {
            // ignored - try native fallback
        }

        // Native fallback using winmm.dll PlaySound
        try
        {
            NativeMethods.PlaySound(path, IntPtr.Zero, NativeMethods.SND_FILENAME | NativeMethods.SND_SYNC);
        }
        catch
        {
            // swallow exceptions — audio is best-effort only
        }
    }

    private static void RunProcess(string exe, string args)
    {
        var psi = new System.Diagnostics.ProcessStartInfo(exe, args)
        {
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            UseShellExecute        = false,
        };
        using var p = System.Diagnostics.Process.Start(psi)!;
        p.WaitForExit();
    }

    private static bool TryRunProcess(string exe, string args)
    {
        try { RunProcess(exe, args); return true; }
        catch { return false; }
    }

    private static class NativeMethods
    {
        public const uint SND_SYNC = 0x0000;
        public const uint SND_ASYNC = 0x0001;
        public const uint SND_FILENAME = 0x00020000;
        public const uint SND_NODEFAULT = 0x00000002;

        [DllImport("winmm.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool PlaySound(string pszSound, IntPtr hmod, uint fdwSound);
    }
}
