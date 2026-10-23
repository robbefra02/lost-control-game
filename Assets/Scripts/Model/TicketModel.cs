using System;
using LostControl.Config;

namespace LostControl.Model
{
    /// <summary>
    /// One user's PC with one problem. Owns the minigame, the rounds and the PC's damage level.
    /// Damage only builds up while the minigame's pressure meter is full. The ticket is resolved after its
    /// last round; at Blackout the user is lost and the ticket closes without being resolved.
    /// </summary>
    public class TicketModel
    {
        readonly float damageInterval;
        readonly CountdownBelowFull countdownBelowFull;

        /// <summary>Index of this ticket's definition in the level config.</summary>
        public int Id { get; }
        public TicketDefinition Definition { get; }
        public MinigameModel Minigame { get; }
        public DamageLevel Damage { get; private set; } = DamageLevel.Fine;

        /// <summary>0-1, filled by the minigame. While full, the damage countdown runs.</summary>
        public float Pressure => Minigame.Pressure;

        public int RoundsDone { get; private set; }
        public int RoundsNeeded { get; }

        /// <summary>Level time at which this ticket arrived.</summary>
        public float ArrivedAt { get; }

        /// <summary>Seconds at a full meter counted toward the next damage level.</summary>
        public float DamageTimer { get; private set; }

        /// <summary>The user is lost. A blacked-out ticket is closed but never counts as resolved.</summary>
        public bool IsBlackout => Damage == DamageLevel.Blackout;

        /// <summary>The player finished every round before Blackout.</summary>
        public bool IsResolved => !IsBlackout && RoundsDone >= RoundsNeeded;

        /// <summary>Resolved or lost: the ticket no longer simulates and cannot be connected to.</summary>
        public bool IsClosed => IsResolved || IsBlackout;

        public event Action<TicketModel> DamageIncreased;

        public TicketModel(int id, TicketDefinition definition, LevelConfig config, MinigameSeed seed, float arrivedAt)
        {
            Id = id;
            Definition = definition;
            ArrivedAt = arrivedAt;
            damageInterval = config.damageInterval;
            countdownBelowFull = config.countdownBelowFull;
            RoundsNeeded = config.GetRounds(definition.minigame);

            Minigame = CreateMinigame(definition.minigame, config, seed, id);
            Minigame.RoundCompleted += OnRoundCompleted;
            Minigame.StartRound(0);
        }

        /// <summary>Advances the minigame and the damage countdown. Connected = the player is in this ticket's session.</summary>
        public void Tick(float deltaTime, bool connected)
        {
            if (IsClosed) return;
            Minigame.Tick(deltaTime, connected);
            TickDamage(deltaTime);
        }

        static MinigameModel CreateMinigame(MinigameType type, LevelConfig config, MinigameSeed seed, int ticketId)
        {
            switch (type)
            {
                case MinigameType.OffFolder: return new OffFolderModel(config.offFolder, seed, ticketId);
                default: throw new ArgumentOutOfRangeException(nameof(type), type, "No model for this minigame.");
            }
        }

        // The next round starts right away; after the last one the ticket is resolved.
        void OnRoundCompleted(MinigameModel minigame)
        {
            RoundsDone++;
            if (RoundsDone < RoundsNeeded) minigame.StartRound(RoundsDone);
        }

        void TickDamage(float deltaTime)
        {
            if (Pressure < 1f)
            {
                if (countdownBelowFull == CountdownBelowFull.Reset) DamageTimer = 0f;
                return;
            }

            DamageTimer += deltaTime;
            while (DamageTimer >= damageInterval && !IsBlackout)
            {
                DamageTimer -= damageInterval;
                Damage++;
                if (IsBlackout)
                {
                    DamageTimer = 0f;
                    Minigame.Stop();
                }
                DamageIncreased?.Invoke(this);
            }
        }
    }
}
