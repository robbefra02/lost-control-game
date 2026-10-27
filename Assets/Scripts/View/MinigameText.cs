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
                case MinigameType.PacketFilter: return "Packet filter";
                case MinigameType.Maze: return "Maze";
                case MinigameType.SsdTrim: return "SSD trimming";
                default: return type.ToString();
            }
        }
    }
}
