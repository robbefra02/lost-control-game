namespace LostControl.Model
{
    /// <summary>
    /// The one seed every minigame draws its randomness from. Each round of each ticket gets its own stream,
    /// so the same seed, ticket and round always generate the same content and a restart replays the level.
    /// </summary>
    public class MinigameSeed
    {
        readonly int seed;

        public MinigameSeed(int seed)
        {
            this.seed = seed;
        }

        /// <summary>Random stream for one round of one ticket's minigame.</summary>
        public SeededRandom For(int ticketId, int round)
        {
            unchecked
            {
                int hash = seed;
                hash = hash * 486187739 + ticketId;
                hash = hash * 486187739 + round;
                return new SeededRandom(hash);
            }
        }
    }
}
