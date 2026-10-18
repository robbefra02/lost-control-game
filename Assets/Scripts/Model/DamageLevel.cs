namespace LostControl.Model
{
    /// <summary>Damage state of one user's PC. Blackout means the user is lost.</summary>
    public enum DamageLevel
    {
        Fine = 0,
        SlightlyBroken = 1,
        MoreBroken = 2,
        Blackout = 3,
    }
}
