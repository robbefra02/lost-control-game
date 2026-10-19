using System.Collections.Generic;
using LostControl.Model;
using TMPro;
using UnityEngine;

namespace LostControl.View
{
    /// <summary>
    /// Draws the ticket's virus dots and damage state on the motherboard panel and passes dot clicks to the model.
    /// It only mirrors model state: dots are added or removed here when the model's dot list changes.
    /// </summary>
    public class MotherboardView : MonoBehaviour
    {
        [SerializeField] GameController game;
        [Tooltip("Area that maps to normalized motherboard coordinates (0-1).")]
        [SerializeField] RectTransform board;
        [Tooltip("Board background, tinted by damage level.")]
        [SerializeField] UnityEngine.UI.Image boardImage;
        [Tooltip("Outline of the infected component, stretched to the virus area.")]
        [SerializeField] RectTransform componentOutline;
        [SerializeField] TMP_Text statusLabel;
        [SerializeField] TMP_Text damageLabel;
        [SerializeField] Sprite dotSprite;
        [SerializeField] Color dotColor = new Color(0.9f, 0.15f, 0.15f);

        [Tooltip("Board tint per damage level: Fine, Slightly broken, More broken, Blackout.")]
        [SerializeField] Color[] damageTints =
        {
            new Color32(0x25, 0x28, 0x2E, 0xFF),
            new Color32(0x3A, 0x36, 0x22, 0xFF),
            new Color32(0x45, 0x26, 0x1F, 0xFF),
            new Color32(0x08, 0x08, 0x0A, 0xFF),
        };

        readonly Dictionary<int, DotView> dotViews = new Dictionary<int, DotView>();
        readonly HashSet<int> liveIds = new HashSet<int>();
        readonly List<int> staleIds = new List<int>();

        void Start()
        {
            var area = game.Simulation.Ticket.Virus.Area;
            componentOutline.anchorMin = area.min;
            componentOutline.anchorMax = area.max;
            componentOutline.offsetMin = Vector2.zero;
            componentOutline.offsetMax = Vector2.zero;
        }

        void LateUpdate()
        {
            var ticket = game.Simulation.Ticket;
            var virus = ticket.Virus;
            SyncDots(virus);
            statusLabel.text = virus.IsCleared ? "Resolved" : $"Dots {virus.DotCount} / {virus.Cap}";
            damageLabel.text = DamageName(ticket.Damage);
            boardImage.color = damageTints[(int)ticket.Damage];
        }

        static string DamageName(DamageLevel level)
        {
            switch (level)
            {
                case DamageLevel.Fine: return "Fine";
                case DamageLevel.SlightlyBroken: return "Slightly broken";
                case DamageLevel.MoreBroken: return "More broken";
                default: return "Blackout";
            }
        }

        void SyncDots(VirusModel virus)
        {
            liveIds.Clear();
            foreach (var dot in virus.Dots)
            {
                liveIds.Add(dot.id);
                if (!dotViews.ContainsKey(dot.id))
                    dotViews.Add(dot.id, CreateDot(dot));
            }

            staleIds.Clear();
            foreach (var id in dotViews.Keys)
                if (!liveIds.Contains(id)) staleIds.Add(id);
            foreach (var id in staleIds)
            {
                Destroy(dotViews[id].gameObject);
                dotViews.Remove(id);
            }
        }

        DotView CreateDot(VirusModel.Dot dot)
        {
            var go = new GameObject("Dot " + dot.id, typeof(RectTransform), typeof(UnityEngine.UI.Image), typeof(DotView));
            go.layer = gameObject.layer;
            var rect = (RectTransform)go.transform;
            rect.SetParent(board, false);
            rect.anchorMin = dot.position;
            rect.anchorMax = dot.position;
            rect.anchoredPosition = Vector2.zero;
            float size = game.Config.dotDiameter;
            rect.sizeDelta = new Vector2(size, size);

            var image = go.GetComponent<UnityEngine.UI.Image>();
            image.sprite = dotSprite;
            image.color = dotColor;

            var view = go.GetComponent<DotView>();
            view.DotId = dot.id;
            view.Clicked += OnDotClicked;
            return view;
        }

        void OnDotClicked(int id)
        {
            game.Simulation.Ticket.Virus.RemoveDot(id);
        }
    }
}
