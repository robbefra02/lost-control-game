using LostControl.Model;

namespace LostControl.View
{
    /// <summary>Player-facing names for minigames.</summary>
    public static class MinigameText
    {
        public static string Name(MinigameType type)
        {
            switch (type)
            {
                case MinigameType.OffFolder: return "Find the off-folder";
                case MinigameType.MatchColors: return "Match the colors";
                case MinigameType.Firewall: return "Extinguish the firewall";
                case MinigameType.Maze: return "Maze";
                default: return type.ToString();
            }
        }
    }
}
