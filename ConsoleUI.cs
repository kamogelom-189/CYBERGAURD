namespace CyberBot;


public static class ConsoleUI
{
    // ── Colour palette ───────────────────────────────────────────────────────
    public static readonly ConsoleColor Primary   = ConsoleColor.Cyan;
    public static readonly ConsoleColor Secondary = ConsoleColor.Green;
    public static readonly ConsoleColor Accent    = ConsoleColor.Yellow;
    public static readonly ConsoleColor Error     = ConsoleColor.Red;
    public static readonly ConsoleColor Muted     = ConsoleColor.DarkGray;
    public static readonly ConsoleColor Bot       = ConsoleColor.Cyan;
    public static readonly ConsoleColor User      = ConsoleColor.White;

    private static readonly int TypingDelayMs = 18;

    // ── ASCII banner ─────────────────────────────────────────────────────────
    private static readonly string[] Banner =
    {
        @"   ██████╗██╗   ██╗██████╗ ███████╗██████╗ ",
         @"  ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗",
         @"  ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝",
         @"  ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗",
         @"  ╚██████╗   ██║   ██████╔╝███████╗██║  ██║",
         @"   ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝",
         @"",
         @"    ███████╗███████╗ ██████╗██╗   ██╗██████╗ ██╗████████╗██╗   ██╗",
         @"    ██╔════╝██╔════╝██╔════╝██║   ██║██╔══██╗██║╚══██╔══╝╚██╗ ██╔╝",
         @"    ███████╗█████╗  ██║     ██║   ██║██████╔╝██║   ██║    ╚████╔╝ ",
         @"    ╚════██║██╔══╝  ██║     ██║   ██║██╔══██╗██║   ██║     ╚██╔╝  ",
         @"    ███████║███████╗╚██████╗╚██████╔╝██║  ██║██║   ██║      ██║   ",
         @"    ╚══════╝╚══════╝ ╚═════╝ ╚═════╝ ╚═╝  ╚═╝╚═╝   ╚═╝      ╚═╝  ",           
                    "🛡  Your Personal Cybersecurity Advisor  🛡",
    };

    private static readonly string[] ShieldArt =
    {
        @"          /\    ",
        @"         /  \   ",
        @"        / /\ \  ",
        @"       / /  \ \ ",
        @"      /_/ __ \_\",
        @"        |    |  ",
        @"        | /\ |  ",
        @"        |/  \|  ",
        @"         \  /   ",
        @"          \/    ",
    };

    // ── Public rendering methods ─────────────────────────────────────────────

    public static void DrawBanner()
    {
        Console.Clear();
        DrawSeparator('═');

        // Shield on the left, banner text on the right
        int shieldWidth = ShieldArt.Max(l => l.Length);

        for (int i = 0; i < Banner.Length; i++)
        {
            // Shield art (first 10 lines)
            Console.ForegroundColor = Secondary;
            string shield = i < ShieldArt.Length ? ShieldArt[i] : new string(' ', shieldWidth);
            Console.Write(shield.PadRight(shieldWidth + 2));

            // Banner text
            Console.ForegroundColor = i == Banner.Length - 1 ? Accent : Primary;
            Console.WriteLine(Banner[i]);
        }

        Console.ResetColor();
        DrawSeparator('═');
        Console.WriteLine();
    }

    public static void DrawSeparator(char ch = '─', ConsoleColor color = ConsoleColor.DarkCyan)
    {
        int width = Math.Min(Console.WindowWidth - 1, 80);
        Console.ForegroundColor = color;
        Console.WriteLine(new string(ch, width));
        Console.ResetColor();
    }

    /// <summary>Prints text one character at a time to simulate typing.</summary>
    public static void TypeWrite(string text, ConsoleColor color = ConsoleColor.White, int delayMs = -1)
    {
        int delay = delayMs < 0 ? TypingDelayMs : delayMs;
        Console.ForegroundColor = color;
        foreach (char c in text)
        {
            Console.Write(c);
            Thread.Sleep(delay);
        }
        Console.ResetColor();
        Console.WriteLine();
    }

    /// <summary>Prints a bot message with a prefix tag and typing animation.</summary>
    public static void BotSay(string message)
    {
        Console.ForegroundColor = Muted;
        Console.Write("  [CyberBot] ");
        TypeWrite(message, Bot);
    }

    /// <summary>Prints a section header.</summary>
    public static void SectionHeader(string title)
    {
        Console.WriteLine();
        Console.ForegroundColor = Accent;
        Console.WriteLine($"  ── {title} ──");
        Console.ResetColor();
    }

    /// <summary>Prints an error message.</summary>
    public static void PrintError(string message)
    {
        Console.ForegroundColor = Error;
        Console.WriteLine($"  [!] {message}");
        Console.ResetColor();
    }

    /// <summary>Prints a success / info message.</summary>
    public static void PrintInfo(string message)
    {
        Console.ForegroundColor = Secondary;
        Console.WriteLine($"  [✓] {message}");
        Console.ResetColor();
    }

    /// <summary>Renders the chat input prompt and returns the trimmed user input.</summary>
    public static string? PromptUser(string username)
    {
        Console.WriteLine();
        Console.ForegroundColor = Accent;
        Console.Write($"  [{username}] > ");
        Console.ForegroundColor = User;
        string? input = Console.ReadLine();
        Console.ResetColor();
        return input;
    }

    /// <summary>Displays the list of recognised commands.</summary>
    public static void ShowHelp()
    {
        Console.WriteLine();
        DrawSeparator();
        SectionHeader("Available Topics");

        var topics = new (string Keyword, string Description)[]
        {
            ("how are you",  "Check in on your cyber-buddy"),
            ("purpose",      "What CyberBot is here to do"),
            ("password",     "Password best practices"),
            ("phishing",     "Spot & avoid phishing attacks"),
            ("malware",      "What malware is & how to stay safe"),
            ("vpn",          "Why a VPN matters"),
            ("2fa",          "Two-factor authentication explained"),
            ("backup",       "The 3-2-1 backup strategy"),
            ("tips",         "Quick daily security checklist"),
            ("help",         "Show this help menu"),
            ("exit / quit",  "Say goodbye"),
        };

        foreach (var (kw, desc) in topics)
        {
            Console.ForegroundColor = Accent;
            Console.Write($"    {kw,-18}");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"→  {desc}");
        }

        DrawSeparator();
        Console.ResetColor();
    }

    /// <summary>Farewell screen.</summary>
    public static void DrawFarewell(string username)
    {
        Console.WriteLine();
        DrawSeparator('═');
        Console.ForegroundColor = Secondary;
        TypeWrite($"  Stay safe out there, {username}! 🛡", Secondary, 25);
        Console.ForegroundColor = Muted;
        Console.WriteLine("  CyberBot signing off.");
        DrawSeparator('═');
        Console.ResetColor();
        Console.WriteLine();
    }
}
