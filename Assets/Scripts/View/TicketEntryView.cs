using System;
using LostControl.Model;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LostControl.View
{
    /// <summary>
    /// One row in the ticket list. While its ticket is stale it keeps showing the last-known damage level
    /// and is dimmed; clicking it asks to connect to the ticket.
    /// </summary>
    public class TicketEntryView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] UnityEngine.UI.Image background;
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] TMP_Text titleLabel;
        [SerializeField] TMP_Text damageLabel;
        [SerializeField] TMP_Text statusLabel;

        [SerializeField] Color normalColor = new Color32(0x38, 0x3C, 0x44, 0xFF);
        [SerializeField] Color connectedColor = new Color32(0x34, 0x4E, 0x78, 0xFF);
        [Tooltip("Background colour at the start of the hidden-damage flash.")]
        [SerializeField] Color flashColor = new Color32(0x9A, 0x2C, 0x2C, 0xFF);
        [SerializeField] float flashDuration = 1f;
        [Range(0f, 1f)]
        [SerializeField] float staleAlpha = 0.55f;

        [Tooltip("Damage label colour per level: Fine, Slightly broken, More broken, Blackout.")]
        [SerializeField] Color[] damageColors =
        {
            new Color32(0x9A, 0xA3, 0xAF, 0xFF),
            new Color32(0xE0, 0xC3, 0x41, 0xFF),
            new Color32(0xE8, 0x83, 0x3A, 0xFF),
            new Color32(0xE5, 0x48, 0x4D, 0xFF),
        };

        Action<TicketModel> clicked;
        DamageLevel shownDamage;
        float flashTimer;

        public TicketModel Ticket { get; private set; }

        public void Bind(TicketModel ticket, Action<TicketModel> onClicked)
        {
            Ticket = ticket;
            clicked = onClicked;
            shownDamage = ticket.Damage;
            titleLabel.text = $"#{ticket.Id + 1}  {ticket.Definition.description}";
        }

        public void Flash()
        {
            flashTimer = flashDuration;
        }

        /// <summary>Updates the row. A stale row keeps its last-known damage level.</summary>
        public void Refresh(bool stale, ConnectionModel connection)
        {
            if (!stale) shownDamage = Ticket.Damage;
            damageLabel.text = DamageLevelText.Name(shownDamage);
            damageLabel.color = damageColors[(int)shownDamage];

            bool isTarget = connection.Target == Ticket;
            if (Ticket.IsResolved) statusLabel.text = "Resolved";
            else if (Ticket.IsBlackout) statusLabel.text = "Lost";
            else if (isTarget && connection.State == ConnectionState.Connecting) statusLabel.text = $"Connecting {Mathf.RoundToInt(connection.Progress * 100f)}%";
            else if (isTarget && connection.State == ConnectionState.Connected) statusLabel.text = "Connected";
            else statusLabel.text = stale ? "Stale" : "Open";

            Color color = isTarget && connection.State == ConnectionState.Connected ? connectedColor : normalColor;
            if (flashTimer > 0f)
            {
                flashTimer -= Time.deltaTime;
                color = Color.Lerp(color, flashColor, Mathf.Clamp01(flashTimer / flashDuration));
            }
            background.color = color;
            canvasGroup.alpha = stale && flashTimer <= 0f ? staleAlpha : 1f;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            clicked?.Invoke(Ticket);
        }
    }
}
