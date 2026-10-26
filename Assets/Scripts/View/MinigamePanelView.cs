using LostControl.Model;
using TMPro;
using UnityEngine;

namespace LostControl.View
{
    /// <summary>
    /// The minigame panel: the remote session. The header shows the connected ticket, its damage, minigame, round
    /// and pressure meter; the board shows its minigame. Without a session (none yet, connecting, or closed because
    /// the ticket was resolved or blacked out) the board shows a session message instead, with the connection progress bar.
    /// </summary>
    public class MinigamePanelView : MonoBehaviour
    {
        [SerializeField] GameController game;
        [Tooltip("Panel title; the connected ticket's number and description.")]
        [SerializeField] TMP_Text headerLabel;
        [SerializeField] string emptyHeader = "Remote PC";
        [Tooltip("Board background, tinted by damage level.")]
        [SerializeField] UnityEngine.UI.Image boardImage;
        [Tooltip("Minigame name and round.")]
        [SerializeField] TMP_Text statusLabel;
        [SerializeField] TMP_Text damageLabel;
        [Tooltip("Pressure meter, shown while a connected ticket is open.")]
        [SerializeField] GameObject meter;
        [Tooltip("Fill of the pressure meter; its right anchor follows the ticket's pressure.")]
        [SerializeField] RectTransform meterFill;
        [Tooltip("One view per minigame type.")]
        [SerializeField] MinigameView[] minigameViews;

        [Header("Session message")]
        [Tooltip("Shown on the board whenever there is no minigame to play.")]
        [SerializeField] GameObject sessionMessage;
        [SerializeField] TMP_Text sessionTitle;
        [SerializeField] TMP_Text sessionInfo;
        [SerializeField] GameObject progressBar;
        [Tooltip("Fill of the progress bar; its right anchor follows the connection progress.")]
        [SerializeField] RectTransform progressFill;

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

            progressBar.SetActive(connection.State == ConnectionState.Connecting);
            sessionMessage.SetActive(Ticket == null);

            if (Ticket == null)
            {
                headerLabel.text = emptyHeader;
                statusLabel.text = "";
                damageLabel.text = "";
                meter.SetActive(false);
                boardImage.color = damageTints[0];
                ShowSessionMessage(connection);
                return;
            }

            headerLabel.text = $"#{Ticket.Id + 1}  {Ticket.Definition.description}";
            damageLabel.text = DamageLevelText.Name(Ticket.Damage);
            boardImage.color = damageTints[(int)Ticket.Damage];
            meter.SetActive(true);
            meterFill.anchorMax = new Vector2(Ticket.Pressure, 1f);
            statusLabel.text = $"{MinigameText.Name(Ticket.Minigame.Type)}   Round {Ticket.RoundsDone + 1} / {Ticket.RoundsNeeded}";
        }

        void ShowSessionMessage(ConnectionModel connection)
        {
            var target = connection.Target;
            if (connection.State == ConnectionState.Connecting)
            {
                sessionTitle.text = $"Connecting to #{target.Id + 1}...";
                sessionInfo.text = target.Definition.description;
                progressFill.anchorMax = new Vector2(connection.Progress, 1f);
                return;
            }

            var kicked = connection.KickedFrom;
            if (kicked == null)
            {
                sessionTitle.text = "No remote session";
                sessionInfo.text = "Click a ticket to connect.";
            }
            else if (kicked.IsBlackout)
            {
                sessionTitle.text = $"#{kicked.Id + 1} blacked out";
                sessionInfo.text = "Session closed: this user is lost. Click another ticket.";
            }
            else
            {
                sessionTitle.text = $"#{kicked.Id + 1} resolved";
                sessionInfo.text = "Session closed: this PC is fixed. Click another ticket.";
            }
        }
    }
}
