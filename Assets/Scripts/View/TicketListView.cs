using System.Collections.Generic;
using UnityEngine;

namespace LostControl.View
{
    /// <summary>Shows one entry per arrived ticket, oldest at the top.</summary>
    public class TicketListView : MonoBehaviour
    {
        [SerializeField] GameController game;
        [Tooltip("Parent of the entries; its layout group stacks them.")]
        [SerializeField] RectTransform entryParent;
        [SerializeField] TicketEntryView entryPrefab;

        readonly List<TicketEntryView> entries = new List<TicketEntryView>();

        void LateUpdate()
        {
            var tickets = game.Simulation.Tickets;
            while (entries.Count < tickets.Count)
            {
                var entry = Instantiate(entryPrefab, entryParent);
                entry.Bind(tickets[entries.Count]);
                entries.Add(entry);
            }

            foreach (var entry in entries) entry.Refresh();
        }
    }
}
