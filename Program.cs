using CyberBot;

// ── CyberBot entry point ──────────────────────────────────────────────────────
// Program.cs is intentionally lean: all logic lives in dedicated classes.

try
{
    new ChatBot().Run();
}
catch (Exception ex)
{
    ConsoleUI.PrintError($"Unexpected error: {ex.Message}");
    Environment.Exit(1);
}
