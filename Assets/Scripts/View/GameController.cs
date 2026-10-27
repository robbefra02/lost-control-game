using LostControl.Config;
using LostControl.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LostControl.View
{
    /// <summary>
    /// Creates the simulation from the level config, advances it every frame once the level has started and turns
    /// model events into audio cues. With the introduction on, the level waits for StartLevel.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        [SerializeField] LevelConfig config;
        [SerializeField] AudioManager audioManager;

        public LevelConfig Config => config;
        public GameSimulation Simulation { get; private set; }

        /// <summary>False while the introduction is shown: the level clock and every ticket wait.</summary>
        public bool Started { get; private set; }

        void Awake()
        {
            Simulation = new GameSimulation(config);
            Simulation.TicketArrived += _ => audioManager.Play(AudioCue.NewTicket);
            Simulation.TicketDamaged += OnTicketDamaged;
            Started = !config.showIntroduction;
        }

        void Update()
        {
            if (Started) Simulation.Tick(Time.deltaTime);
        }

        /// <summary>Starts the level clock (the player closed the introduction).</summary>
        public void StartLevel()
        {
            Started = true;
        }

        /// <summary>Starts the level over by reloading the scene, so every model and view starts fresh.</summary>
        public void Restart()
        {
            SceneManager.LoadScene(gameObject.scene.name);
        }

        void OnTicketDamaged(TicketModel ticket)
        {
            if (config.hiddenDamageCue && Simulation.IsHidden(ticket))
                audioManager.Play(AudioCue.HiddenDamage);
        }
    }
}
