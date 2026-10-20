namespace LostControl.Model
{
    public enum GameOutcome
    {
        Playing = 0,

        /// <summary>Every ticket in the level is resolved.</summary>
        Won = 1,

        /// <summary>Enough users reached Blackout.</summary>
        Lost = 2,
    }
}
