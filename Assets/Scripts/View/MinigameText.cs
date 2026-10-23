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
                default: return type.ToString();
            }
        }
    }
}
