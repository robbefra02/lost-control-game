namespace LostControl.Config
{
    /// <summary>What the damage countdown does when a virus drops below its dot cap.</summary>
    public enum CountdownBelowCap
    {
        /// <summary>Keep the elapsed time and continue from there when the virus is back at cap.</summary>
        Pause = 0,

        /// <summary>Start from zero again. Allows clicking one dot every interval to block all damage.</summary>
        Reset = 1,
    }
}
