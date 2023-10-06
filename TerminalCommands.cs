using static Terminal;

namespace Achievements;

public static class TerminalCommands
{
    [HarmonyPatch(typeof(Terminal), nameof(InitTerminal))]
    [HarmonyWrapSafe]
    internal class AddChatCommands
    {
        private static void Postfix()
        {
            new ConsoleCommand("resetAchievements", "", args =>
            {
                try
                {
                    Achs.ResetAllAchievements();
                }
                catch (Exception e)
                {
                    args.Context.AddString("<color=red>Error: " + e.Message + "</color>");
                }
            });
        }
    }
}