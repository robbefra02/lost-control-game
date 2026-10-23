namespace LostControl.Config
{
    /// <summary>What the damage countdown does when a ticket's pressure meter drops below full.</summary>
    public enum CountdownBelowFull
    {
        /// <summary>Keep the elapsed time and continue from there when the meter is full again.</summary>
        Pause = 0,

        /// <summary>Start from zero again.</summary>
        Reset = 1,
    }
}
