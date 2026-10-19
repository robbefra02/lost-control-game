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

        public TicketDefinition Definition { get; }
        public VirusModel Virus { get; }
        public DamageLevel Damage { get; private set; } = DamageLevel.Fine;

        /// <summary>Seconds at cap counted toward the next damage level.</summary>
        public float DamageTimer { get; private set; }

        public bool IsResolved => Virus.IsCleared;
        public bool IsBlackout => Damage == DamageLevel.Blackout;

        public TicketModel(TicketDefinition definition, LevelConfig config, SeededRandom random)
        {
            Definition = definition;
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
            }
            if (IsBlackout) DamageTimer = 0f;
        }
    }
}
