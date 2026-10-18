using LostControl.Config;

namespace LostControl.Model
{
    /// <summary>Owns and ticks every model in a level. Views only read its state.</summary>
    public class GameSimulation
    {
        /// <summary>Step 3: one hardcoded ticket (the first in the config). Step 5 replaces this with the ticket list.</summary>
        public VirusModel Virus { get; }

        /// <summary>Seconds since the level started.</summary>
        public float Time { get; private set; }

        public GameSimulation(LevelConfig config)
        {
            var random = new SeededRandom(config.randomSeed);
            var ticket = config.tickets[0];
            int startDots = random.RangeInclusive(config.startDotsMin, config.startDotsMax);
            Virus = new VirusModel(ticket.infectedArea, startDots, config.dotCap, config.spreadInterval, config.spreadDistance, random);
        }

        public void Tick(float deltaTime)
        {
            Time += deltaTime;
            Virus.Tick(deltaTime);
        }
    }
}
