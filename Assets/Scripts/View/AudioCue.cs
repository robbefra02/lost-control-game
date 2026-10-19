namespace LostControl.View
{
    /// <summary>Every sound the game can play. Real clips are assigned per cue on the AudioManager.</summary>
    public enum AudioCue
    {
        NewTicket = 0,

        /// <summary>A ticket the player cannot see gained a damage level.</summary>
        HiddenDamage = 1,
    }
}
