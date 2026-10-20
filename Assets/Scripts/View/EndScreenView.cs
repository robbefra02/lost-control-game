using LostControl.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LostControl.View
{
    /// <summary>Covers the game with the result once the level is won or lost, with a restart button.</summary>
    public class EndScreenView : MonoBehaviour
    {
        [SerializeField] GameController game;
        [Tooltip("Overlay shown on game over; it also blocks clicks on the game behind it.")]
        [SerializeField] GameObject overlay;
        [SerializeField] TMP_Text titleLabel;
        [SerializeField] TMP_Text summaryLabel;
        [SerializeField] Button restartButton;

        void Awake()
        {
            overlay.SetActive(false);
            restartButton.onClick.AddListener(game.Restart);
        }

        void LateUpdate()
        {
            var simulation = game.Simulation;
            bool over = simulation.Outcome != GameOutcome.Playing;
            if (overlay.activeSelf != over) overlay.SetActive(over);
            if (!over) return;

            titleLabel.text = simulation.Outcome == GameOutcome.Won
                ? "All tickets resolved"
                : $"{simulation.LostUsers} users lost";
            summaryLabel.text =
                $"Resolved {simulation.ResolvedCount} / {simulation.TotalTickets}    " +
                $"Lost users {simulation.LostUsers} / {simulation.LostUsersToLose}    " +
                $"Time {TimeText.Format(simulation.Time)}";
        }
    }
}
