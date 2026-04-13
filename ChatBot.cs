using System;
using System.Linq;
using System.Threading;

namespace CyberBot;

/// <summary>
/// Orchestrates the full chatbot session:
///   1. Greeting (banner + voice)
///   2. Name acquisition
///   3. Chat loop
///   4. Farewell
/// </summary>
public class ChatBot
{
    private string _username = "User";

    // ── Entry point ───────────────────────────────────────────────────────────

    public void Run()
    {
        Greet();
        AcquireUsername();
        ChatLoop();
        ConsoleUI.DrawFarewell(_username);
    }

    // ── Phases ────────────────────────────────────────────────────────────────

    private void Greet()
    {
        ConsoleUI.DrawBanner();

        // Try audio greeting; let the user know either way.
        // Pass the absolute WAV path you provided so the program will attempt to play it.
        bool audioPlayed = VoiceGreeting.Play(@"C:\Users\Student\Downloads\files (4)\Cybersecurity.wav");
        if (audioPlayed)
            ConsoleUI.PrintInfo("Audio greeting played.");
        else
            ConsoleUI.PrintInfo("(No audio found - place assets/greetings.wav for voice greeting.)");

        Console.WriteLine();
        ConsoleUI.TypeWrite("  Welcome to CyberBot — your cybersecurity companion.", ConsoleUI.Primary, 22);
        ConsoleUI.TypeWrite("  Type 'help' at any time to see what I can discuss.", ConsoleUI.Muted, 18);
        Console.WriteLine();
    }

    private void AcquireUsername()
    {
        ConsoleUI.SectionHeader("Let's get acquainted");

        string? name = null;
        int attempts = 0;
        const int maxAttempts = 5;

        while (string.IsNullOrWhiteSpace(name) && attempts < maxAttempts)
        {
            attempts++;

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("  What's your name? > ");
            Console.ForegroundColor = ConsoleColor.White;
            var raw = Console.ReadLine();
            Console.ResetColor();

            // Handle EOF / Ctrl+Z
            if (raw is null)
            {
                ConsoleUI.PrintInfo("No input detected. Exiting.");
                Environment.Exit(0);
            }

            var trimmed = raw.Trim();

            // Recognize inline commands at the prompt
            if (string.Equals(trimmed, "help", StringComparison.OrdinalIgnoreCase))
            {
                ConsoleUI.ShowHelp();
                continue;
            }

            if (string.Equals(trimmed, "exit", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmed, "quit", StringComparison.OrdinalIgnoreCase))
            {
                if (ConfirmExit())
                    Environment.Exit(0);
                else
                    continue;
            }

            if (string.IsNullOrWhiteSpace(trimmed))
            {
                ConsoleUI.PrintError("I need a name to address you properly. Please try again.");
                continue;
            }

            name = trimmed;
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            ConsoleUI.PrintInfo("No valid name entered. Proceeding with default name 'User'.");
            _username = "User";
        }
        else
        {
            _username = SanitiseName(name);
        }

        Console.WriteLine();
        ConsoleUI.BotSay($"Great to meet you, {_username}! 🙌 I'm here to help you stay safe online.");
        ConsoleUI.BotSay("Type 'help' to see available topics, or just ask me anything!");
        ConsoleUI.DrawSeparator();
    }

    private void ChatLoop()
    {
        while (true)
        {
            string? input = ConsoleUI.PromptUser(_username);

            // Handle null / Ctrl+Z / EOF
            if (input is null)
                break;

            // Quick local commands before engine (fast-path)
            var trimmed = input.Trim();
            if (string.Equals(trimmed, "exit", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmed, "quit", StringComparison.OrdinalIgnoreCase))
            {
                if (ConfirmExit())
                    break;
                else
                {
                    ConsoleUI.DrawSeparator();
                    continue;
                }
            }

            try
            {
                var (response, tip, isExit, isHelp) = ResponseEngine.GetResponse(input);

                if (isExit)
                    break;

                if (isHelp)
                {
                    ConsoleUI.ShowHelp();
                    continue;
                }

                // Special case: daily tips list
                if (trimmed.Equals("tip", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.Equals("tips", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.Equals("advice", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.Equals("checklist", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.Equals("best practice", StringComparison.OrdinalIgnoreCase))
                {
                    ConsoleUI.BotSay(response);
                    PrintTips();
                }
                else
                {
                    Console.WriteLine();
                    ConsoleUI.BotSay(response);
                    if (tip is not null)
                    {
                        try { Thread.Sleep(300); } catch { /* ignore sleep interruptions */ }
                        ConsoleUI.BotSay(tip);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log and continue the session rather than crash the app.
                ConsoleUI.PrintError($"An error occurred while processing your message: {ex.Message}");
            }

            ConsoleUI.DrawSeparator();
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static void PrintTips()
    {
        string[] tips = ResponseEngine.DailyTips();
        Console.WriteLine();
        for (int i = 0; i < tips.Length; i++)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"    {i + 1,2}. ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(tips[i]);
            try { Thread.Sleep(60); } catch { }
        }
        Console.ResetColor();
    }

    /// <summary>Keeps only printable ASCII and caps length.</summary>
    private static string SanitiseName(string raw)
    {
        var clean = new string(raw.Where(c => c >= 32 && c < 127).ToArray()).Trim();
        return clean.Length > 30 ? clean[..30] : clean.Length == 0 ? "User" : clean;
    }

    private static bool ConfirmExit()
    {
        ConsoleUI.PrintInfo("Are you sure you want to exit? (y/N)");
        Console.ForegroundColor = ConsoleColor.White;
        var resp = Console.ReadLine();
        Console.ResetColor();
        return !string.IsNullOrWhiteSpace(resp) && resp.Trim().Equals("y", StringComparison.OrdinalIgnoreCase);
    }
}
