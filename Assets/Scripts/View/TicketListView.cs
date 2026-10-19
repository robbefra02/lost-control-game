using System.Collections.Generic;
using LostControl.Model;
using UnityEngine;

namespace LostControl.View
{
    /// <summary>Shows one entry per arrived ticket, oldest at the top. Clicking an entry connects to its ticket.</summary>
    public class TicketListView : MonoBehaviour
    {
        [SerializeField] GameController game;
        [Tooltip("Parent of the entries; its layout group stacks them.")]
        [SerializeField] RectTransform entryParent;
        [SerializeField] TicketEntryView entryPrefab;

        readonly List<TicketEntryView> entries = new List<TicketEntryView>();

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
        }

        // Hidden degradation cue: flash the row of a ticket the player cannot see.
        void OnTicketDamaged(TicketModel ticket)
        {
            if (!game.Config.hiddenDamageCue || !game.Simulation.IsStale(ticket)) return;
            foreach (var entry in entries)
                if (entry.Ticket == ticket) entry.Flash();
        }
    }
}
