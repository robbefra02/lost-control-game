using TMPro;
using UnityEngine;

namespace LostControl.View
{
    /// <summary>Level-wide numbers in the game data column: level timer, resolved tickets and lost users.</summary>
    public class GameDataView : MonoBehaviour
    {
        [SerializeField] GameController game;
        [SerializeField] TMP_Text timeValue;
        [SerializeField] TMP_Text resolvedValue;
        [SerializeField] TMP_Text lostUsersValue;

        void LateUpdate()
        {
            var simulation = game.Simulation;
            timeValue.text = TimeText.Format(simulation.Time);
            resolvedValue.text = $"{simulation.ResolvedCount} / {simulation.TotalTickets}";
            lostUsersValue.text = $"{simulation.LostUsers} / {simulation.LostUsersToLose}";
        }
    }
}
