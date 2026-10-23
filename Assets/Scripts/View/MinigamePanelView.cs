using LostControl.Model;
using TMPro;
using UnityEngine;

namespace LostControl.View
{
    /// <summary>
    /// The minigame panel: shows the connected ticket's minigame, its round, pressure meter and damage state.
    /// Only a fully connected ticket is shown; the minigame views draw the game and handle its input.
    /// </summary>
    public class MinigamePanelView : MonoBehaviour
    {
        [SerializeField] GameController game;
        [Tooltip("Panel title; names the connected ticket's minigame.")]
        [SerializeField] TMP_Text headerLabel;
        [SerializeField] string emptyHeader = "Remote PC";
        [Tooltip("Board background, tinted by damage level.")]
        [SerializeField] UnityEngine.UI.Image boardImage;
        [SerializeField] TMP_Text statusLabel;
        [SerializeField] TMP_Text damageLabel;
        [Tooltip("Pressure meter, shown while connected.")]
        [SerializeField] GameObject meter;
        [Tooltip("Fill of the pressure meter; its right anchor follows the ticket's pressure.")]
        [SerializeField] RectTransform meterFill;
        [SerializeField] UnityEngine.UI.Image meterFillImage;
        [SerializeField] Color meterColor = new Color32(0xE0, 0xC3, 0x41, 0xFF);
        [Tooltip("Meter colour while full, i.e. while the damage countdown runs.")]
        [SerializeField] Color meterFullColor = new Color32(0xE5, 0x48, 0x4D, 0xFF);
        [Tooltip("One view per minigame type.")]
        [SerializeField] MinigameView[] minigameViews;

        [Tooltip("Board tint per damage level: Fine, Slightly broken, More broken, Blackout.")]
        [SerializeField] Color[] damageTints =
        {
            new Color32(0x25, 0x28, 0x2E, 0xFF),
            new Color32(0x3A, 0x36, 0x22, 0xFF),
            new Color32(0x45, 0x26, 0x1F, 0xFF),
            new Color32(0x08, 0x08, 0x0A, 0xFF),
        };

        /// <summary>The ticket in the panel, or null for an empty panel.</summary>
        public TicketModel Ticket { get; private set; }

        void Awake()
        {
            Show(null);
        }

        /// <summary>Switches the panel to another ticket (or null to empty it).</summary>
        public void Show(TicketModel ticket)
        {
            Ticket = ticket;
            foreach (var view in minigameViews)
            {
                bool shown = ticket != null && view.Type == ticket.Minigame.Type;
                view.gameObject.SetActive(shown);
                view.Show(shown ? ticket.Minigame : null);
            }
        }

        void LateUpdate()
        {
            // Only a fully connected ticket can be seen and worked on.
            var connection = game.Simulation.Connection;
            var connected = connection.State == ConnectionState.Connected ? connection.Target : null;
            if (connected != Ticket) Show(connected);

            if (Ticket == null)
            {
                headerLabel.text = emptyHeader;
                statusLabel.text = connection.State == ConnectionState.Connecting ? "Connecting..." : "";
                damageLabel.text = "";
                meter.SetActive(false);
                boardImage.color = damageTints[0];
                return;
            }

            headerLabel.text = MinigameText.Name(Ticket.Minigame.Type);
            statusLabel.text = Ticket.IsResolved ? "Resolved" : $"Round {Ticket.RoundsDone + 1} / {Ticket.RoundsNeeded}";
            damageLabel.text = DamageLevelText.Name(Ticket.Damage);
            meter.SetActive(true);
            meterFill.anchorMax = new Vector2(Ticket.Pressure, 1f);
            meterFillImage.color = Ticket.Pressure >= 1f ? meterFullColor : meterColor;
            boardImage.color = damageTints[(int)Ticket.Damage];
        }
    }
}
