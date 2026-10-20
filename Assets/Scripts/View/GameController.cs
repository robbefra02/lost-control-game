using LostControl.Config;
using LostControl.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        /// <summary>Starts the level over by reloading the scene, so every model and view starts fresh.</summary>
        public void Restart()
        {
            SceneManager.LoadScene(gameObject.scene.name);
        }

        void OnTicketDamaged(TicketModel ticket)
        {
            if (config.hiddenDamageCue && Simulation.IsStale(ticket))
                audioManager.Play(AudioCue.HiddenDamage);
        }
    }
}
