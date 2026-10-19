using LostControl.Model;
using TMPro;
using UnityEngine;

namespace LostControl.View
{
    /// <summary>One row in the ticket list: description, damage level and status of one ticket.</summary>
    public class TicketEntryView : MonoBehaviour
    {
        [SerializeField] TMP_Text titleLabel;
        [SerializeField] TMP_Text damageLabel;
        [SerializeField] TMP_Text statusLabel;

        [Tooltip("Damage label colour per level: Fine, Slightly broken, More broken, Blackout.")]
        [SerializeField] Color[] damageColors =
        {
            new Color32(0x9A, 0xA3, 0xAF, 0xFF),
            new Color32(0xE0, 0xC3, 0x41, 0xFF),
            new Color32(0xE8, 0x83, 0x3A, 0xFF),
            new Color32(0xE5, 0x48, 0x4D, 0xFF),
        };

        public TicketModel Ticket { get; private set; }

        public void Bind(TicketModel ticket)
        {
            Ticket = ticket;
            titleLabel.text = $"#{ticket.Id + 1}  {ticket.Definition.description}";
            Refresh();
        }

        public void Refresh()
        {
            damageLabel.text = DamageLevelText.Name(Ticket.Damage);
            damageLabel.color = damageColors[(int)Ticket.Damage];
            statusLabel.text = Ticket.IsResolved ? "Resolved" : "Open";
        }
    }
}
