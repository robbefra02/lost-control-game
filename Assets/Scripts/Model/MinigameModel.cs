using System;

namespace LostControl.Model
{
    /// <summary>
    /// The task on one ticket's PC, played in rounds. Each round's content comes from the minigame seed.
    /// While a round is open the minigame fills its pressure meter (0-1) in its own way; a full meter costs
    /// the ticket a damage level. Finishing a round empties the meter.
    /// </summary>
    public abstract class MinigameModel
    {
        readonly MinigameSeed seed;
        readonly int ticketId;

        public abstract MinigameType Type { get; }

        /// <summary>0-1. At 1 the ticket takes a damage level.</summary>
        public float Pressure { get; protected set; }

        /// <summary>Index of the current round, from 0. Stays on the last round once the ticket is resolved.</summary>
        public int Round { get; private set; }

        /// <summary>True while the current round can be played; input is ignored otherwise.</summary>
        public bool IsRoundOpen { get; private set; }

        public event Action<MinigameModel> RoundCompleted;

        protected MinigameModel(MinigameSeed seed, int ticketId)
        {
            this.seed = seed;
            this.ticketId = ticketId;
        }

        /// <summary>Opens a round with new content generated from the minigame seed.</summary>
        public void StartRound(int round)
        {
            Round = round;
            IsRoundOpen = true;
            Generate(seed.For(ticketId, round), round);
        }

        /// <summary>Closes the round without finishing it (the ticket closed). Input is ignored afterwards.</summary>
        public void Stop()
        {
            IsRoundOpen = false;
        }

        /// <summary>Drops the meter to this level if it is higher. The ticket calls it after a full meter cost a damage level.</summary>
        public void LowerPressure(float level)
        {
            Pressure = Math.Min(Pressure, level);
        }

        /// <summary>Advances the open round. Runs whether or not the player is connected to this ticket.</summary>
        public void Tick(float deltaTime, bool connected)
        {
            if (!IsRoundOpen) return;
            OnTick(deltaTime, connected);
        }

        /// <summary>Builds the content of a round. Use only this random stream, so the round can be replayed.</summary>
        protected abstract void Generate(SeededRandom random, int round);

        protected abstract void OnTick(float deltaTime, bool connected);

        protected void AddPressure(float amount)
        {
            Pressure = Math.Min(Math.Max(Pressure + amount, 0f), 1f);
        }

        /// <summary>The player finished the open round: the meter empties and the ticket decides what comes next.</summary>
        protected void CompleteRound()
        {
            if (!IsRoundOpen) return;
            IsRoundOpen = false;
            Pressure = 0f;
            RoundCompleted?.Invoke(this);
        }
    }
}
