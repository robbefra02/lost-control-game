using LostControl.Config;
using LostControl.Model;
using UnityEngine;

namespace LostControl.View
{
    /// <summary>Creates the simulation from the level config and advances it every frame.</summary>
    public class GameController : MonoBehaviour
    {
        [SerializeField] LevelConfig config;

        public LevelConfig Config => config;
        public GameSimulation Simulation { get; private set; }

        void Awake()
        {
            Simulation = new GameSimulation(config);
        }

        void Update()
        {
            Simulation.Tick(Time.deltaTime);
        }
    }
}
