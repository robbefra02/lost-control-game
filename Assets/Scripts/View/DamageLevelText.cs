using LostControl.Model;

namespace LostControl.View
{
    /// <summary>Player-facing names for damage levels.</summary>
    public static class DamageLevelText
    {
        public static string Name(DamageLevel level)
        {
            switch (level)
            {
                case DamageLevel.Fine: return "Fine";
                case DamageLevel.SlightlyBroken: return "Slightly broken";
                case DamageLevel.MoreBroken: return "More broken";
                default: return "Blackout";
            }
        }
    }
}
