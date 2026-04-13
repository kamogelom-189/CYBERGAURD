namespace CyberBot;

/// <summary>
/// Matches user input to cybersecurity topics and returns appropriate responses.
/// </summary>
public static class ResponseEngine
{
    // ── Response table ────────────────────────────────────────────────────────
    // Each entry: (keyword(s), response, optional follow-up tip)
    private static readonly (string[] Keywords, string Response, string? Tip)[] Responses =
    {
        (
            new[] { "how are you", "how r u", "you ok", "you good" },
            "I'm just code, but I'm running at full strength and ready to keep you safe! 💪",
            null
        ),
        (
            new[] { "purpose", "what do you do", "what can you do", "who are you", "your job" },
            "I'm CyberBot — your personal cybersecurity advisor. I help you stay safe online by explaining threats, best practices, and how to protect your digital life.",
            null
        ),
        (
            new[] { "password", "passphrase", "login credentials" },
            "Use strong, unique passwords for every account. Aim for 16+ characters mixing letters, numbers, and symbols.",
            "💡 Tip: Use a reputable password manager (e.g. Bitwarden or 1Password) so you only need to remember one master password."
        ),
        (
            new[] { "phishing", "suspicious email", "fake link", "scam email", "spam" },
            "Be careful of suspicious emails and links. Attackers impersonate trusted brands to steal your credentials.",
            "💡 Tip: Always check the sender's actual email address, hover over links before clicking, and never enter credentials on a page you navigated to via email."
        ),
        (
            new[] { "malware", "virus", "ransomware", "trojan", "spyware" },
            "Malware is malicious software designed to damage, disrupt, or gain unauthorised access to your system. Keep your OS and antivirus up to date and avoid downloading software from unverified sources.",
            "💡 Tip: Windows Defender is solid, but adding Malwarebytes as a second-opinion scanner can catch things that slip through."
        ),
        (
            new[] { "vpn", "virtual private network", "proxy" },
            "A VPN encrypts your internet traffic and masks your IP address — crucial on public Wi-Fi where attackers can sniff unencrypted data.",
            "💡 Tip: Choose a no-log VPN from a reputable provider (Mullvad, ProtonVPN). Free VPNs often monetise your data."
        ),
        (
            new[] { "2fa", "two factor", "two-factor", "mfa", "authenticator", "otp" },
            "Two-factor authentication (2FA) requires a second proof of identity — usually a time-based code — in addition to your password. Even if your password is stolen, an attacker can't log in without the second factor.",
            "💡 Tip: Use an authenticator app (Authy, Google Authenticator) instead of SMS codes — SIM-swap attacks can intercept SMS."
        ),
        (
            new[] { "backup", "back up", "data loss", "restore" },
            "Follow the 3-2-1 backup rule: keep 3 copies of your data, on 2 different media types, with 1 copy stored offsite (e.g. cloud).",
            "💡 Tip: Test your backups regularly! A backup you've never restored from is a backup you can't trust."
        ),
        (
            new[] { "tip", "tips", "advice", "checklist", "best practice" },
            "Here's your daily security checklist:",
            null // handled specially in ChatBot.cs
        ),
        (
            new[] { "help", "?" },
            "__HELP__",
            null
        ),
        (
            new[] { "exit", "quit", "bye", "goodbye", "ciao", "later" },
            "__EXIT__",
            null
        ),
    };

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Finds a matching response for the given input.
    /// Returns (response, tip, isExit, isHelp).
    /// </summary>
    public static (string Response, string? Tip, bool IsExit, bool IsHelp) GetResponse(string input)
    {
        string normalised = input.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(normalised))
            return ("I didn't catch that. Type 'help' to see what I can discuss.", null, false, false);

        foreach (var (keywords, response, tip) in Responses)
        {
            if (keywords.Any(k => normalised.Contains(k)))
            {
                if (response == "__EXIT__") return ("__EXIT__", null, true, false);
                if (response == "__HELP__") return ("__HELP__", null, false, true);
                return (response, tip, false, false);
            }
        }

        // No match
        return (
            $"I'm not sure about \"{SanitiseForDisplay(input)}\". Try asking about: passwords, phishing, malware, VPNs, 2FA, backups, or type 'help'.",
            null, false, false
        );
    }

    /// <summary>Daily tips list, exposed separately so the UI can render them specially.</summary>
    public static string[] DailyTips() => new[]
    {
        "Update your software and OS regularly.",
        "Use a password manager.",
        "Enable 2FA on all important accounts.",
        "Think before you click any link or attachment.",
        "Use a VPN on public Wi-Fi.",
        "Back up your data weekly.",
        "Review app permissions on your phone.",
        "Lock your screen when stepping away.",
    };

    // ── Helpers ───────────────────────────────────────────────────────────────
    private static string SanitiseForDisplay(string input) =>
        input.Length > 40 ? input[..40] + "…" : input;
}
