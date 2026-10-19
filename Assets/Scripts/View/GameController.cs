using LostControl.Config;
using LostControl.Model;
using UnityEngine;

namespace LostControl.View
{
    /// <summary>Creates the simulation from the level config, advances it every frame and turns model events into audio cues.</summary>
    public class GameController : MonoBehaviour
    {
        [SerializeField] LevelConfig config;
        [SerializeField] AudioManager audioManager;

        public LevelConfig Config => config;
        public GameSimulation Simulation { get; private set; }

        void Awake()
        {
            Simulation = new GameSimulation(config);
            Simulation.TicketArrived += _ => audioManager.Play(AudioCue.NewTicket);
            Simulation.TicketDamaged += OnTicketDamaged;
        }

        void Update()
        {
            Simulation.Tick(Time.deltaTime);
        }

        void OnTicketDamaged(TicketModel ticket)
        {
            if (config.hiddenDamageCue && Simulation.IsStale(ticket))
                audioManager.Play(AudioCue.HiddenDamage);
        }
    }
}
