using System;
using LostControl.Config;

namespace LostControl.Model
{
    /// <summary>
    /// One user's PC with one problem. Owns the virus and the PC's damage level.
    /// Damage only builds up while the virus is at its cap. At Blackout the user is lost:
    /// the virus is gone with the PC and the ticket closes without being resolved.
    /// </summary>
    public class TicketModel
    {
        readonly float damageInterval;
        readonly CountdownBelowCap countdownBelowCap;

        /// <summary>Index of this ticket's definition in the level config.</summary>
        public int Id { get; }
        public TicketDefinition Definition { get; }
        public VirusModel Virus { get; }
        public DamageLevel Damage { get; private set; } = DamageLevel.Fine;

        /// <summary>Level time at which this ticket arrived.</summary>
        public float ArrivedAt { get; }

        /// <summary>Seconds at cap counted toward the next damage level.</summary>
        public float DamageTimer { get; private set; }

        /// <summary>The user is lost. A blacked-out ticket is closed but never counts as resolved.</summary>
        public bool IsBlackout => Damage == DamageLevel.Blackout;

        /// <summary>The player removed every dot before Blackout.</summary>
        public bool IsResolved => !IsBlackout && Virus.IsCleared;

        /// <summary>Resolved or lost: the ticket no longer simulates and cannot be connected to.</summary>
        public bool IsClosed => IsResolved || IsBlackout;

        public event Action<TicketModel> DamageIncreased;

        public TicketModel(int id, TicketDefinition definition, LevelConfig config, SeededRandom random, float arrivedAt)
        {
            Id = id;
            Definition = definition;
            ArrivedAt = arrivedAt;
            damageInterval = config.damageInterval;
            countdownBelowCap = config.countdownBelowCap;

            int startDots = random.RangeInclusive(config.startDotsMin, config.startDotsMax);
            Virus = new VirusModel(definition.infectedArea, startDots, config.dotCap, config.spreadInterval, config.spreadDistance, random);
        }

        public void Tick(float deltaTime)
        {
            if (IsClosed) return;
            Virus.Tick(deltaTime);
            TickDamage(deltaTime);
        }

        void TickDamage(float deltaTime)
        {
            if (!Virus.IsAtCap)
            {
                if (countdownBelowCap == CountdownBelowCap.Reset) DamageTimer = 0f;
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
                    Virus.Clear();
                }
                DamageIncreased?.Invoke(this);
            }
        }
    }
}
