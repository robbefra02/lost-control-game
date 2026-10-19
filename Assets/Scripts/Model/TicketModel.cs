using System;
using LostControl.Config;

namespace LostControl.Model
{
    /// <summary>
    /// One user's PC with one problem. Owns the virus and the PC's damage level.
    /// Damage only builds up while the virus is at its cap.
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

        public bool IsResolved => Virus.IsCleared;
        public bool IsBlackout => Damage == DamageLevel.Blackout;

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
            if (IsResolved) return;
            Virus.Tick(deltaTime);
            TickDamage(deltaTime);
        }

        void TickDamage(float deltaTime)
        {
            if (IsBlackout) return;

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
                DamageIncreased?.Invoke(this);
            }
            if (IsBlackout) DamageTimer = 0f;
        }
    }
}
