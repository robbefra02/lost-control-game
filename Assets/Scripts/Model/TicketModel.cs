using System;
using LostControl.Config;

namespace LostControl.Model
{
    /// <summary>
    /// One user's PC with one problem. Owns the minigame, the rounds and the PC's damage level.
    /// The minigame's damage rule decides when the PC takes a damage level (by default: each time its meter fills).
    /// The ticket is resolved after its last round; at Blackout the user is lost and the ticket closes without being resolved.
    /// </summary>
    public class TicketModel
    {
        readonly float pressureAfterDamage;

        /// <summary>Index of this ticket's definition in the level config.</summary>
        public int Id { get; }
        public TicketDefinition Definition { get; }
        public MinigameModel Minigame { get; }
        public DamageLevel Damage { get; private set; } = DamageLevel.Fine;

        /// <summary>0-1, filled by the minigame. A full meter costs one damage level.</summary>
        public float Pressure => Minigame.Pressure;

        public int RoundsDone { get; private set; }
        public int RoundsNeeded { get; }

        /// <summary>Level time at which this ticket arrived.</summary>
        public float ArrivedAt { get; }

        /// <summary>The user is lost. A blacked-out ticket is closed but never counts as resolved.</summary>
        public bool IsBlackout => Damage == DamageLevel.Blackout;

        /// <summary>The player finished every round before Blackout.</summary>
        public bool IsResolved => !IsBlackout && RoundsDone >= RoundsNeeded;

        /// <summary>Resolved or lost: the ticket no longer simulates and cannot be connected to.</summary>
        public bool IsClosed => IsResolved || IsBlackout;

        public event Action<TicketModel> DamageIncreased;

        /// <summary>The player finished the last round.</summary>
        public event Action<TicketModel> Resolved;

        public TicketModel(int id, TicketDefinition definition, LevelConfig config, MinigameSeed seed, float arrivedAt)
        {
            Id = id;
            Definition = definition;
            ArrivedAt = arrivedAt;
            pressureAfterDamage = config.pressureAfterDamage;
            RoundsNeeded = config.GetRounds(definition.minigame);

            Minigame = CreateMinigame(definition.minigame, config, seed, id);
            Minigame.RoundCompleted += OnRoundCompleted;
            Minigame.StartRound(0);
        }

        /// <summary>Advances the minigame and applies its damage rule. Connected = the player is in this ticket's session.</summary>
        public void Tick(float deltaTime, bool connected)
        {
            if (IsClosed) return;
            Minigame.Tick(deltaTime, connected);
            // Also catches a meter that a click filled since the last tick.
            if (Minigame.CheckDamage(deltaTime, pressureAfterDamage)) TakeDamage();
        }

        static MinigameModel CreateMinigame(MinigameType type, LevelConfig config, MinigameSeed seed, int ticketId)
        {
            switch (type)
            {
                case MinigameType.OffFolder: return new OffFolderModel(config.offFolder, seed, ticketId);
                case MinigameType.MatchColors: return new MatchColorsModel(config.matchColors, seed, ticketId);
                case MinigameType.PacketFilter: return new PacketFilterModel(config.packetFilter, seed, ticketId);
                case MinigameType.Maze: return new MazeModel(config.maze, seed, ticketId);
                case MinigameType.SsdTrim: return new SsdTrimModel(config.ssdTrim, seed, ticketId);
                default: throw new ArgumentOutOfRangeException(nameof(type), type, "No model for this minigame.");
            }
        }

        // The next round starts right away; after the last one the ticket is resolved.
        void OnRoundCompleted(MinigameModel minigame)
        {
            RoundsDone++;
            if (RoundsDone < RoundsNeeded) minigame.StartRound(RoundsDone);
            else Resolved?.Invoke(this);
        }

        void TakeDamage()
        {
            Damage++;
            if (IsBlackout) Minigame.Stop();
            DamageIncreased?.Invoke(this);
        }
    }
}
