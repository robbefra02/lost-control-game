using System.Collections.Generic;
using LostControl.Model;
using UnityEngine;
using UnityEngine.UI;

namespace LostControl.View
{
    /// <summary>
    /// Shows one entry per arrived ticket, oldest at the top. Clicking an entry connects to its ticket.
    /// The header has a disconnect button.
    /// </summary>
    public class TicketListView : MonoBehaviour
    {
        [SerializeField] GameController game;
        [Tooltip("Parent of the entries; its layout group stacks them.")]
        [SerializeField] RectTransform entryParent;
        [SerializeField] TicketEntryView entryPrefab;

        [SerializeField] Button disconnectButton;
        [SerializeField] CanvasGroup disconnectGroup;
        [Range(0f, 1f)]
        [SerializeField] float disabledAlpha = 0.35f;

        readonly List<TicketEntryView> entries = new List<TicketEntryView>();

        void Awake()
        {
            disconnectButton.onClick.AddListener(() => game.Simulation.Disconnect());
        }

        void Start()
        {
            game.Simulation.TicketDamaged += OnTicketDamaged;
        }

        void LateUpdate()
        {
            var simulation = game.Simulation;
            var tickets = simulation.Tickets;
            while (entries.Count < tickets.Count)
            {
                var entry = Instantiate(entryPrefab, entryParent);
                entry.Bind(tickets[entries.Count], simulation.Connect);
                entries.Add(entry);
            }

            foreach (var entry in entries)
                entry.Refresh(simulation.IsStale(entry.Ticket), simulation.Connection);

            bool canDisconnect = simulation.Outcome == GameOutcome.Playing
                && simulation.Connection.State == ConnectionState.Connected;
            disconnectButton.interactable = canDisconnect;
            disconnectGroup.alpha = canDisconnect ? 1f : disabledAlpha;
        }

        // Hidden degradation cue: flash the row of a ticket the player cannot see.
        void OnTicketDamaged(TicketModel ticket)
        {
            if (!game.Config.hiddenDamageCue || !game.Simulation.IsHidden(ticket)) return;
            foreach (var entry in entries)
                if (entry.Ticket == ticket) entry.Flash();
        }
    }
}
