using LostControl.Model;
using TMPro;
using UnityEngine;

namespace LostControl.View
{
    /// <summary>The connected user's PC screen: session state, the ticket and the connection progress bar.</summary>
    public class PcScreenView : MonoBehaviour
    {
        [SerializeField] GameController game;
        [SerializeField] TMP_Text titleLabel;
        [SerializeField] TMP_Text infoLabel;
        [SerializeField] GameObject progressBar;
        [Tooltip("Fill of the progress bar; its right anchor follows the connection progress.")]
        [SerializeField] RectTransform progressFill;

        void LateUpdate()
        {
            var connection = game.Simulation.Connection;
            var ticket = connection.Target;
            progressBar.SetActive(connection.State == ConnectionState.Connecting);

            switch (connection.State)
            {
                case ConnectionState.Connecting:
                    titleLabel.text = $"Connecting to #{ticket.Id + 1}...";
                    infoLabel.text = ticket.Definition.description;
                    progressFill.anchorMax = new Vector2(connection.Progress, 1f);
                    break;
                case ConnectionState.Connected:
                    titleLabel.text = $"#{ticket.Id + 1}  {ticket.Definition.description}";
                    infoLabel.text = ticket.IsResolved
                        ? "Ticket resolved. Pick the next ticket."
                        : $"Connected - {DamageLevelText.Name(ticket.Damage)}";
                    break;
                default:
                    var kicked = connection.KickedFrom;
                    titleLabel.text = kicked != null ? $"#{kicked.Id + 1} blacked out" : "No remote session";
                    infoLabel.text = kicked != null ? "Session closed: this user is lost. Click another ticket." : "Click a ticket to connect.";
                    break;
            }
        }
    }
}
