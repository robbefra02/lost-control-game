using System;
using LostControl.Model;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LostControl.View
{
    /// <summary>
    /// One row in the ticket list. While its ticket is stale it keeps showing the last-known damage level
    /// and round, and is dimmed; clicking it asks to connect to the ticket. A closed ticket shrinks to one
    /// line with only its number, title and "Resolved" or "Blackout".
    /// </summary>
    [RequireComponent(typeof(UnityEngine.UI.LayoutElement))]
    public class TicketEntryView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] UnityEngine.UI.Image background;
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] TMP_Text titleLabel;
        [SerializeField] TMP_Text damageLabel;
        [SerializeField] TMP_Text statusLabel;
        [Tooltip("Row height once the ticket is closed (resolved or blacked out), in canvas pixels.")]
        [SerializeField] float resolvedHeight = 48f;

        [SerializeField] Color normalColor = new Color32(0x38, 0x3C, 0x44, 0xFF);
        [SerializeField] Color connectedColor = new Color32(0x34, 0x4E, 0x78, 0xFF);
        [Tooltip("Background colour at the start of the hidden-damage flash.")]
        [SerializeField] Color flashColor = new Color32(0x9A, 0x2C, 0x2C, 0xFF);
        [SerializeField] float flashDuration = 1f;
        [Range(0f, 1f)]
        [SerializeField] float staleAlpha = 0.2f;

        [Tooltip("Damage label colour per level: Fine, Slightly broken, More broken, Blackout.")]
        [SerializeField] Color[] damageColors =
        {
            new Color32(0x9A, 0xA3, 0xAF, 0xFF),
            new Color32(0xE0, 0xC3, 0x41, 0xFF),
            new Color32(0xE8, 0x83, 0x3A, 0xFF),
            new Color32(0xE5, 0x48, 0x4D, 0xFF),
        };
        [Tooltip("Colour of the round count behind the damage level.")]
        [SerializeField] Color roundColor = new Color32(0x9A, 0xA3, 0xAF, 0xFF);

        Action<TicketModel> clicked;
        DamageLevel shownDamage;
        int shownRoundsDone;
        float flashTimer;
        UnityEngine.UI.LayoutElement layoutElement;
        bool compact;
        float fullHeight;
        float titleY;
        float titleRight;
        float statusY;

        public TicketModel Ticket { get; private set; }

        void Awake()
        {
            layoutElement = GetComponent<UnityEngine.UI.LayoutElement>();
            fullHeight = layoutElement.preferredHeight;
            titleY = titleLabel.rectTransform.anchoredPosition.y;
            titleRight = titleLabel.rectTransform.offsetMax.x;
            statusY = statusLabel.rectTransform.anchoredPosition.y;
        }

        public void Bind(TicketModel ticket, Action<TicketModel> onClicked)
        {
            Ticket = ticket;
            clicked = onClicked;
            shownDamage = ticket.Damage;
            shownRoundsDone = ticket.RoundsDone;
            titleLabel.text = $"#{ticket.Id + 1}  {ticket.Definition.description}";
        }

        public void Flash()
        {
            flashTimer = flashDuration;
        }

        /// <summary>Takes the ticket's current damage level and round as the last-known state, even while stale.</summary>
        public void Snapshot()
        {
            shownDamage = Ticket.Damage;
            shownRoundsDone = Ticket.RoundsDone;
        }

        /// <summary>Updates the row. A stale row keeps its last-known damage level and round.</summary>
        public void Refresh(bool stale, ConnectionModel connection)
        {
            if (!stale) Snapshot();
            damageLabel.text = Ticket.IsClosed
                ? DamageLevelText.Name(shownDamage)
                : $"{DamageLevelText.Name(shownDamage)}  <color=#{ColorUtility.ToHtmlStringRGB(roundColor)}>Round {shownRoundsDone + 1}/{Ticket.RoundsNeeded}</color>";
            damageLabel.color = damageColors[(int)shownDamage];

            bool isTarget = connection.Target == Ticket;
            if (Ticket.IsResolved) statusLabel.text = "Resolved";
            else if (Ticket.IsBlackout) statusLabel.text = "Blackout";
            else if (isTarget && connection.State == ConnectionState.Connecting) statusLabel.text = $"Connecting {Mathf.RoundToInt(connection.Progress * 100f)}%";
            else if (isTarget && connection.State == ConnectionState.Connected) statusLabel.text = "Connected";
            else statusLabel.text = stale ? "Stale" : "Open";
            SetCompact(Ticket.IsClosed);

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

        // Compact: half height, no damage line, title and status side by side on one centred line. The title is
        // anchored to the top of the row and the status to the bottom; the title stops just before the status text.
        void SetCompact(bool value)
        {
            if (compact == value) return;
            compact = value;

            layoutElement.preferredHeight = value ? resolvedHeight : fullHeight;
            damageLabel.gameObject.SetActive(!value);

            var title = titleLabel.rectTransform;
            float statusWidth = statusLabel.GetPreferredValues(statusLabel.text).x;
            title.offsetMax = new Vector2(value ? titleRight - statusWidth - 12f : titleRight, title.offsetMax.y);
            title.anchoredPosition = new Vector2(title.anchoredPosition.x, value ? -resolvedHeight / 2f : titleY);
            var status = statusLabel.rectTransform;
            status.anchoredPosition = new Vector2(status.anchoredPosition.x, value ? resolvedHeight / 2f : statusY);
        }
    }
}
