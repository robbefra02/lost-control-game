using LostControl.Config;

namespace LostControl.Model
{
    /// <summary>Owns and ticks every model in a level. Views only read its state.</summary>
    public class GameSimulation
    {
        /// <summary>Steps 3-4: one hardcoded ticket (the first in the config). Step 5 replaces this with the ticket list.</summary>
        public TicketModel Ticket { get; }

        /// <summary>Seconds since the level started.</summary>
        public float Time { get; private set; }

        public GameSimulation(LevelConfig config)
        {
            var random = new SeededRandom(config.randomSeed);
            Ticket = new TicketModel(config.tickets[0], config, random);
        }

        public void Tick(float deltaTime)
        {
            Time += deltaTime;
            Ticket.Tick(deltaTime);
        }
    }
}
